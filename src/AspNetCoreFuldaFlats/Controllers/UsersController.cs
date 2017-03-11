using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreFuldaFlats.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public UsersController(WebApiDataContext webApiDataContext, ILogger<UsersController> logger)
        {
            _database = webApiDataContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp()
        {
            return BadRequest();
        }


        [HttpPost("auth")]
        public async Task<IActionResult> SigIn([FromBody] SignInData signInData)
        {
            IActionResult response = StatusCode(403);

            var email = signInData.Email;
            var password = signInData.Password;

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                email = email.ToLower();

                try
                {
                    var user = await
                        _database.User.SingleOrDefaultAsync(u => u.Email == email);

                    if (user == null)
                    {
                        _logger.LogDebug($"User sign in - invalid email: {email}");
                    }
                    else
                    {
                        var passwordHash = GetPasswordHash(password);

                        if ((user.IsLocked != 1) && (user.Password == passwordHash))
                        {
                            await SignInUser(user);
                            await LoadUserRelationships(user);
                            user.LoginAttempts = 0;
                            response = Ok(user);
                        }
                        else if (user.IsLocked != 1)
                        {
                            user.LoginAttempts = user.LoginAttempts ?? 0;
                            if (user.LoginAttempts + 1 < UserConstants.MaxSignInAttempts)
                            {
                                user.LoginAttempts++;
                            }
                            else
                            {
                                user.LoginAttempts++;
                                user.IsLocked = 1;
                                response = StatusCode(423);
                            }
                        }

                        await PersistUser(user);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode(500);
                }
            }

            return response;
        }
        
        [HttpDelete("auth")]
        public async Task<IActionResult> SignOut()
        {
            IActionResult response = StatusCode(204);

            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    await HttpContext.Authentication.SignOutAsync(GlobalConstants.CookieAuthenticationSchema);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }


            return response;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var user = await GetCurrentUserFromDatabase(true);

                if (user == null)
                {
                    sendStatus = NotFound();
                }
                else
                {
                    sendStatus = Ok(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                sendStatus = StatusCode(500);
            }

            return sendStatus;
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] User updateInfo)
        {
            IActionResult response = BadRequest();

            try
            {
                var user = await GetCurrentUserFromDatabase();

                if (user == null)
                {
                    response = NotFound();
                }
                else
                {
                    LandlordUpdateError landlordValidationInfo = null;
                    if (user.Type == UserConstants.UserTypes.Landlord)
                    {
                        landlordValidationInfo = await ValidateLandlordInfo(user, updateInfo);
                    }
                    NormalUserUpdateError normalUserUpdateError = await ValidateNormalUserInfo(user, updateInfo, landlordValidationInfo);

                    if (normalUserUpdateError.HasError)
                    {
                        response = BadRequest(normalUserUpdateError);
                    }
                    else
                    {
                        UpdateNormalUserProperties(user, updateInfo);
                        if (user.Type == UserConstants.UserTypes.Landlord)
                        {
                            UpdateLandlordProperties(user, updateInfo);
                        }

                        await PersistUser(user);
                        await LoadUserRelationships(user);

                        await SignInUser(user);

                        response = Ok(user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }

        [Authorize]
        [HttpPut("upgrade")]
        public async Task<IActionResult> UpgradeUser([FromBody] User upgradeInfo)
        {
            IActionResult response = BadRequest();
            
            try
            {
                var user = await GetCurrentUserFromDatabase();

                if (user == null)
                {
                    response = NotFound();
                }
                else
                {
                    if (user.Type != UserConstants.UserTypes.Normal)
                    {
                        response = BadRequest(new LandlordUpdateError
                        {
                            Upgrade = {[0] = "Unable to upgrade this user account."}
                        });
                    }
                    else
                    {
                        LandlordUpdateError landlordValidationInfo = await ValidateUpgradeToLandlord(user, upgradeInfo);
                        NormalUserUpdateError normalUserUpdateError = await ValidateNormalUserInfo(user, upgradeInfo, landlordValidationInfo);
                        if (normalUserUpdateError.HasError)
                        {
                            response = BadRequest(normalUserUpdateError);
                        }
                        else
                        {
                            UpdateNormalUserProperties(user, upgradeInfo);
                            UpdateLandlordProperties(user, upgradeInfo);
                            UpgradeUser(user, upgradeInfo);
                            await PersistUser(user);
                            await LoadUserRelationships(user);

                            await SignInUser(user);

                            response = Ok(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }

        [Authorize]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInfo changePasswordInfo)
        {
            IActionResult response = BadRequest();

            if (string.IsNullOrWhiteSpace(changePasswordInfo.PasswordNew) || (changePasswordInfo.PasswordNew.Length < UserConstants.MinPasswordLength))
            {
                response = BadRequest(new ChangePasswordError
                {
                    Password = {[0] = "Invalid Password (please use at least 5 characters)."}
                });
            }
            else
            {
                try
                {
                    var oldPasswordHash = GetPasswordHash(changePasswordInfo.PasswordOld);
                    var newPasswordHash = GetPasswordHash(changePasswordInfo.PasswordNew);

                    var user = await _database.User.SingleOrDefaultAsync(
                        u => u.Email == HttpContext.User.Identity.Name && u.Password == oldPasswordHash);

                    if (user != null)
                    {
                        user.Password = newPasswordHash;
                        await PersistUser(user);
                        response = StatusCode(204);
                    }
                    else
                    {
                        response = StatusCode(404, new ChangePasswordError
                        {
                            Password = { [0] = "Invalid password." }
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode(500);
                }
            }

            return response;
        }

        [Authorize]
        [HttpPut("standardPicture")]
        public async Task<IActionResult> SetProfilePicture([FromBody] SetProfilePictureData profilePictureData)
        {
            IActionResult response = BadRequest();

            if (!string.IsNullOrWhiteSpace(profilePictureData?.Img))
            {
                try
                {
                    var user = await GetCurrentUserFromDatabase();

                    if (user == null)
                    {
                        response = NotFound();
                    }
                    else {
                        user.ProfilePicture = profilePictureData.Img;
                        await PersistUser(user);
                        response = Ok(user);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode(500);
                }
            }

            return response;
        }

        #region Helper Functions
        
        private async Task<User> GetCurrentUserFromDatabase(bool withRelationships = false)
        {
            User user;
            if (withRelationships)
            {
                user = await _database.User
                    .Include(u => u.DatabaseFavorites).ThenInclude(f => f.Offer).ThenInclude(o => o.Mediaobjects)
                    .Include(u => u.DatabaseFavorites).ThenInclude(f => f.Offer).ThenInclude(o => o.Tags)
                    .Include(u => u.Offers).ThenInclude(o => o.Mediaobjects)
                    .Include(u => u.Offers).ThenInclude(o => o.Tags)
                    .SingleOrDefaultAsync(u => u.Email == HttpContext.User.Identity.Name);
            }
            else
            {
                user = await _database.User.SingleOrDefaultAsync(u => u.Email == HttpContext.User.Identity.Name);
            }
             
            return user;
        }

        private async Task LoadUserRelationships(User user)
        {
            await _database.Entry(user)
                              .Collection(u => u.DatabaseFavorites)
                              .Query()
                              .Include(f => f.Offer).ThenInclude(o => o.Mediaobjects)
                              .Include(f => f.Offer).ThenInclude(o => o.Tags)
                              .LoadAsync();

            await _database.Entry(user).Collection(u => u.Offers)
                .Query()
                .Include(o => o.Mediaobjects)
                .Include(o => o.Tags)
                .LoadAsync();
        }

        private async Task PersistUser(User user)
        {
            _database.User.Update(user);
            await _database.SaveChangesAsync();
        }

        private async Task<LandlordUpdateError> ValidateLandlordInfo(User currentUser, User userInfo, LandlordUpdateError prefilledErrorInfo = null)
        {
            LandlordUpdateError updateError = prefilledErrorInfo as LandlordUpdateError ?? new LandlordUpdateError();

            if (!string.IsNullOrWhiteSpace(userInfo.Email) && new EmailAddressAttribute().IsValid(userInfo.Email) == false)
            {
                updateError.Email[0] = "Please enter a valid eMail.";
                updateError.HasError = true;
            }
            else if (!string.IsNullOrWhiteSpace(userInfo.Email) && userInfo.Email.ToLower() != currentUser.Email.ToLower() &&
                      await _database.User.AnyAsync(u => u.Email == userInfo.Email.ToLower()))
            {
                updateError.Email[0] = "Please enter a valid eMail. This eMail is not unique.";
                updateError.HasError = true;
            }

            return updateError;
        }

        private async Task<LandlordUpdateError> ValidateUpgradeToLandlord(User currentUser, User userInfo, LandlordUpdateError prefilledErrorInfo = null)
        {
            LandlordUpdateError updateError = prefilledErrorInfo as LandlordUpdateError ?? new LandlordUpdateError();

            if (string.IsNullOrWhiteSpace(userInfo.PhoneNumber))
            {
                updateError.PhoneNumber[0] = "Please enter a phone number.";
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.ZipCode))
            {
                updateError.ZipCode[0] = "Please enter a zip code.";
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.City))
            {
                updateError.City[0] = "Please enter a city.";
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.Street))
            {
                updateError.Street[0] = "Please enter a street.";
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.HouseNumber))
            {
                updateError.HouseNumber[0] = "Please enter a house number.";
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.Email) || new EmailAddressAttribute().IsValid(userInfo.Email) == false)
            {
                updateError.Email[0] = "Please enter a valid eMail.";
                updateError.HasError = true;
            }
            else if (!string.IsNullOrWhiteSpace(userInfo.Email) && userInfo.Email.ToLower() != currentUser.Email.ToLower() &&
                      await _database.User.AnyAsync(u => u.Email == userInfo.Email.ToLower()))
            {
                updateError.Email[0] = "Please enter a valid eMail. This eMail is not unique.";
                updateError.HasError = true;
            }

            return updateError;
        }

        private async Task<NormalUserUpdateError> ValidateNormalUserInfo(User currentUser, User userInfo, NormalUserUpdateError prefilledErrorInfo = null)
        {
            NormalUserUpdateError updateError = prefilledErrorInfo ?? new NormalUserUpdateError();
            
            UserConstants.Genders gender;
            if (!string.IsNullOrWhiteSpace(userInfo.Gender) && !Enum.TryParse(userInfo.Gender, true, out gender))
            {
                updateError.Gender[0] = $"Please enter a valid gender. {String.Join(",", Enum.GetNames(typeof(UserConstants.Genders)))}.";
                updateError.HasError = true;
            }

            if (!string.IsNullOrWhiteSpace(userInfo.Email) && new EmailAddressAttribute().IsValid(userInfo.Email) == false)
            {
                updateError.Email[0] = "Please enter a valid eMail.";
                updateError.HasError = true;
            }
            else if (!string.IsNullOrWhiteSpace(userInfo.Email) && userInfo.Email.ToLower() != currentUser.Email.ToLower() &&
                      await _database.User.AnyAsync(u => u.Email == userInfo.Email.ToLower()))
            {
                updateError.Email[0] = "Please enter a valid eMail. This eMail is not unique.";
                updateError.HasError = true;
            }

            return updateError;
        }

        private string MergeOfficeAddress(User user)
        {
            var officeAddresse = string.Empty;

            if (string.IsNullOrWhiteSpace(user.ZipCode))
            {
                officeAddresse += user.ZipCode;
            }
            if (string.IsNullOrWhiteSpace(user.City))
            {
                if (string.IsNullOrWhiteSpace(user.ZipCode))
                {
                    officeAddresse += " ";
                }
                officeAddresse += user.City;
            }
            if (string.IsNullOrWhiteSpace(user.Street))
            {
                if (string.IsNullOrWhiteSpace(officeAddresse))
                {
                    officeAddresse += ", ";
                }
                officeAddresse += user.Street;

                if (string.IsNullOrWhiteSpace(user.HouseNumber))
                {
                    officeAddresse += " ";
                    officeAddresse += user.HouseNumber;
                }
            }

            return officeAddresse;
        }

        private void UpgradeUser(User currentUser, User upgradeInfo)
        {
            currentUser.Type = UserConstants.UserTypes.Landlord;
            currentUser.UpgradeDate = DateTime.Now;
            currentUser.AverageRating = 1;          
        }

        private void UpdateNormalUserProperties(User currentUser, User updateInfo)
        {
            if (!string.IsNullOrWhiteSpace(updateInfo.FirstName))
            {
                currentUser.FirstName = updateInfo.FirstName;
            }
            
            if(!string.IsNullOrWhiteSpace(updateInfo.LastName))
            {
                currentUser.LastName = updateInfo.LastName;
            }

            if (updateInfo.Birthday.HasValue)
            {
                currentUser.Birthday = updateInfo.Birthday;
            }

            UserConstants.Genders gender;
            if (Enum.TryParse(updateInfo.Gender, true, out gender))
            {
                currentUser.Gender = Enum.GetName(typeof(UserConstants.Genders), gender).ToLower();
            }

            if (!string.IsNullOrWhiteSpace(updateInfo.Email))
            {
                currentUser.Email = updateInfo.Email.ToLower();
            }
        }

        private void UpdateLandlordProperties(User currentUser, User updateInfo)
        {
            if (!string.IsNullOrWhiteSpace(updateInfo.PhoneNumber))
            {
                currentUser.PhoneNumber = updateInfo.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(updateInfo.ZipCode))
            {
                currentUser.ZipCode = updateInfo.ZipCode;
            }

            if (!string.IsNullOrWhiteSpace(updateInfo.City))
            {
                currentUser.City = updateInfo.City;
            }

            if (!string.IsNullOrWhiteSpace(updateInfo.Street))
            {
                currentUser.Street = updateInfo.Street;
            }

            if (!string.IsNullOrWhiteSpace(updateInfo.HouseNumber))
            {
                currentUser.HouseNumber = updateInfo.HouseNumber;
            }

            if (!string.IsNullOrWhiteSpace(updateInfo.Email))
            {
                currentUser.Email = updateInfo.Email.ToLower();
            }

            currentUser.OfficeAddress = MergeOfficeAddress(currentUser);
        }
        
        private string GetPasswordHash(string plainPassword)
        {
            return Convert.ToBase64String(
                            SHA512.Create()
                                .ComputeHash(
                                    Encoding.UTF8.GetBytes(UserConstants.PasswordSalt + plainPassword)));
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email, ClaimValueTypes.Email,
                    HttpContext.Request.Host.Host),
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString(), ClaimValueTypes.String,
                    HttpContext.Request.Host.Host),
                new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email,
                    HttpContext.Request.Host.Host),
                new Claim(ClaimTypes.GivenName, user.FirstName, ClaimValueTypes.String,
                    HttpContext.Request.Host.Host),
                new Claim(ClaimTypes.Surname, user.LastName, ClaimValueTypes.String,
                    HttpContext.Request.Host.Host)
            };

            var claimsIdentity = new ClaimsIdentity(claims,
                GlobalConstants.IdentityAuthenticationSchema);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.Authentication.SignInAsync(GlobalConstants.CookieAuthenticationSchema,
                claimsPrincipal);
            HttpContext.User = claimsPrincipal;
        }
        #endregion

    }
}