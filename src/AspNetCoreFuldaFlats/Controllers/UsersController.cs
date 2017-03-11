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
        public async Task<IActionResult> UpdateMe()
        {
            return BadRequest();
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
                        response = BadRequest(new UpgradeError
                        {
                            Upgrade = {[0] = "Unable to upgrade this user account."}
                        });
                    }
                    else
                    {
                        UpgradeError validationInfo = ValidateUpgradeInfo(upgradeInfo);
                        if (validationInfo.HasError)
                        {
                            response = BadRequest(validationInfo);
                        }
                        else
                        {
                            UpgradeUser(user, upgradeInfo);
                            await LoadUserRelationships(user);
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

        private UpgradeError ValidateUpgradeInfo(User upgradeInfo)
        {
            UpgradeError upgradeError = new UpgradeError();

            if (string.IsNullOrWhiteSpace(upgradeInfo.PhoneNumber))
            {
                upgradeError.PhoneNumber[0] = "Please enter a phone number.";
                upgradeError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(upgradeInfo.ZipCode))
            {
                upgradeError.ZipCode[0] = "Please enter a zip code.";
                upgradeError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(upgradeInfo.City))
            {
                upgradeError.City[0] = "Please enter a city.";
                upgradeError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(upgradeInfo.Street))
            {
                upgradeError.Street[0] = "Please enter a street.";
                upgradeError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(upgradeInfo.HouseNumber))
            {
                upgradeError.HouseNumber[0] = "Please enter a house number.";
                upgradeError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(upgradeInfo.Email) && new EmailAddressAttribute().IsValid(upgradeInfo.Email) == false)
            {
                upgradeError.Email[0] = "Please enter a valid eMail.";
                upgradeError.HasError = true;
            }

            return upgradeError;
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
            currentUser.PhoneNumber = upgradeInfo.PhoneNumber;
            currentUser.ZipCode = upgradeInfo.ZipCode;
            currentUser.City = upgradeInfo.City;
            currentUser.Street = upgradeInfo.Street;
            currentUser.HouseNumber = upgradeInfo.HouseNumber;
            currentUser.Email = upgradeInfo.Email;
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