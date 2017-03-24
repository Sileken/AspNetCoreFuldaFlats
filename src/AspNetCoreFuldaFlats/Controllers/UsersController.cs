using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCoreFuldaFlats.Controllers
{
    /// <summary>
    ///     Endpoints for user functions.
    /// </summary>
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public UsersController(IOptions<AppSettings> appSettingsOptions, WebApiDataContext webApiDataContext,
            ILogger<UsersController> logger)
        {
            _appSettings = appSettingsOptions.Value;
            _database = webApiDataContext;
            _logger = logger;
        }

        /// <summary>
        ///     Create a new User (Registration functionality) with given JSON data in the body.
        /// </summary>
        /// <param name="signUpInfo"></param>
        /// <returns></returns>
        [Produces(typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(SignUpError))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] User signUpInfo)
        {
            IActionResult response = BadRequest();

            try
            {
                var signUpError = await ValidateSignUpInfo(signUpInfo);
                if (signUpError.HasError)
                {
                    response = BadRequest(signUpError);
                }
                else
                {
                    var user = new User
                    {
                        Password = GetPasswordHash(signUpInfo.ReadPassword),
                        CreationDate = DateTime.Now,
                        Type = UserConstants.UserTypes.Normal,
                        AverageRating = 0,
                        IsLocked = false
                    };

                    UpdateNormalUserProperties(user, signUpInfo);
                    await PersistUser(user);
                    await LoadUserRelationships(user);
                    await SignInUser(user);
                    response = StatusCode((int) HttpStatusCode.Created, user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Authenticate current Session with login data.
        /// </summary>
        /// <param name="signInData"></param>
        /// <returns></returns>
        [Produces(typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse(423)]
        [SwaggerResponse((int) HttpStatusCode.Forbidden)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [HttpPost("auth")]
        public async Task<IActionResult> SigIn([FromBody] SignInInfo signInData)
        {
            IActionResult response = StatusCode((int) HttpStatusCode.Forbidden);

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

                        if ((user.IsLocked != true) && (user.Password == passwordHash))
                        {
                            await SignInUser(user);
                            await LoadUserRelationships(user);
                            user.LoginAttempts = 0;
                            response = Ok(user);
                        }
                        else if (user.IsLocked != true)
                        {
                            user.LoginAttempts = user.LoginAttempts ?? 0;
                            if (user.LoginAttempts + 1 < _appSettings.MaxSignInAttempts)
                            {
                                user.LoginAttempts++;
                            }
                            else
                            {
                                user.LoginAttempts++;
                                user.IsLocked = true;
                                response = StatusCode(423); // 423 Locked
                            }
                        }

                        await UpdateUser(user);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode((int) HttpStatusCode.InternalServerError);
                }
            }

            return response;
        }

        /// <summary>
        ///     Sign out (delete authentication in current session).
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [HttpDelete("auth")]
        public async Task<IActionResult> SignOut()
        {
            IActionResult response = NoContent();

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
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }


            return response;
        }

        /// <summary>
        ///     Retrieve own user data (when sign in).
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            IActionResult response = BadRequest();

            try
            {
                var user = await GetCurrentUserFromDatabase(true);

                if (user == null)
                {
                    response = NotFound();
                }
                else
                {
                    response = Ok(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Manipulate own user data (when sign in) with given JSON data in the body.
        /// </summary>
        /// <param name="updateInfo"></param>
        /// <returns></returns>
        [Produces(typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(NormalUserUpdateError))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
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
                    var normalUserUpdateError = await ValidateNormalUserInfo(user, updateInfo, landlordValidationInfo);

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

                        await UpdateUser(user);
                        await LoadUserRelationships(user);

                        await SignInUser(user);

                        response = Ok(user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Upgrade the current user to a landlord.
        /// </summary>
        /// <param name="upgradeInfo"></param>
        /// <returns></returns>
        [Produces(typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(NormalUserUpdateError))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
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
                            Upgrade = new List<string> {"Unable to upgrade this user account."}
                        });
                    }
                    else
                    {
                        var landlordValidationInfo = await ValidateUpgradeToLandlord(user, upgradeInfo);
                        var normalUserUpdateError =
                            await ValidateNormalUserInfo(user, upgradeInfo, landlordValidationInfo);
                        if (normalUserUpdateError.HasError)
                        {
                            response = BadRequest(normalUserUpdateError);
                        }
                        else
                        {
                            UpdateNormalUserProperties(user, upgradeInfo);
                            UpdateLandlordProperties(user, upgradeInfo);
                            UpgradeUser(user, upgradeInfo);
                            await UpdateUser(user);
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
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Change the password from the current user.
        /// </summary>
        /// <param name="changePasswordInfo"></param>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(ChangePasswordError))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, typeof(ChangePasswordError))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [Authorize]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInfo changePasswordInfo)
        {
            IActionResult response = BadRequest();

            if (string.IsNullOrWhiteSpace(changePasswordInfo.PasswordNew) ||
                (changePasswordInfo.PasswordNew.Length < _appSettings.MinPasswordLength))
            {
                response = BadRequest(new ChangePasswordError
                {
                    Password = new List<string> {"Invalid Password (please use at least 5 characters)."}
                });
            }
            else
            {
                try
                {
                    var oldPasswordHash = GetPasswordHash(changePasswordInfo.PasswordOld);
                    var newPasswordHash = GetPasswordHash(changePasswordInfo.PasswordNew);

                    var user = await _database.User.SingleOrDefaultAsync(
                        u => (u.Email == HttpContext.User.Identity.Name) && (u.Password == oldPasswordHash));

                    if (user != null)
                    {
                        user.Password = newPasswordHash;
                        await UpdateUser(user);
                        response = NoContent();
                    }
                    else
                    {
                        response = NotFound(new ChangePasswordError
                        {
                            Password = new List<string> {"Invalid password."}
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode((int) HttpStatusCode.InternalServerError);
                }
            }

            return response;
        }

        /// <summary>
        ///     Set the profile picture from the current user.
        /// </summary>
        /// <param name="profilePictureData"></param>
        /// <returns></returns>
        [Produces(typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.OK, typeof(User))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
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
                    else
                    {
                        user.ProfilePicture = profilePictureData.Img;
                        await UpdateUser(user);
                        response = Ok(user);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode((int) HttpStatusCode.InternalServerError);
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
                    .Include(u => u.DatabaseFavorites).ThenInclude(f => f.Offer).ThenInclude(o => o.MediaObjects)
                    .Include(u => u.DatabaseFavorites).ThenInclude(f => f.Offer).ThenInclude(o => o.Tags)
                    .Include(u => u.Offers).ThenInclude(o => o.MediaObjects)
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
                .Include(f => f.Offer).ThenInclude(o => o.MediaObjects)
                .Include(f => f.Offer).ThenInclude(o => o.Tags)
                .LoadAsync();

            await _database.Entry(user).Collection(u => u.Offers)
                .Query()
                .Include(o => o.MediaObjects)
                .Include(o => o.Tags)
                .LoadAsync();
        }

        private async Task PersistUser(User user)
        {
            await _database.User.AddAsync(user);
            await _database.SaveChangesAsync();
        }

        private async Task UpdateUser(User user)
        {
            _database.User.Update(user);
            await _database.SaveChangesAsync();
        }

        private async Task<LandlordUpdateError> ValidateLandlordInfo(User currentUser, User userInfo,
            LandlordUpdateError prefilledErrorInfo = null)
        {
            var updateError = prefilledErrorInfo ?? new LandlordUpdateError();

            if (!string.IsNullOrWhiteSpace(userInfo.Email) &&
                (new EmailAddressAttribute().IsValid(userInfo.Email) == false))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail."};
                updateError.HasError = true;
            }
            else if (!string.IsNullOrWhiteSpace(userInfo.Email) && (userInfo.Email.ToLower() != currentUser.Email.ToLower()) &&
                     await _database.User.AnyAsync(u => u.Email == userInfo.Email.ToLower()))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail. This eMail is not unique."};
                updateError.HasError = true;
            }

            return updateError;
        }

        private async Task<LandlordUpdateError> ValidateUpgradeToLandlord(User currentUser, User userInfo,
            LandlordUpdateError prefilledErrorInfo = null)
        {
            var updateError = prefilledErrorInfo ?? new LandlordUpdateError();

            if (string.IsNullOrWhiteSpace(userInfo.PhoneNumber))
            {
                updateError.PhoneNumber = new List<string> {"Please enter a phone number."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.ZipCode))
            {
                updateError.ZipCode = new List<string> {"Please enter a zip code."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.City))
            {
                updateError.City = new List<string> {"Please enter a city."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.Street))
            {
                updateError.Street = new List<string> {"Please enter a street."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.HouseNumber))
            {
                updateError.HouseNumber = new List<string> {"Please enter a house number."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.Email) ||
                (new EmailAddressAttribute().IsValid(userInfo.Email) == false))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail."};
                updateError.HasError = true;
            }
            else if (!string.IsNullOrWhiteSpace(userInfo.Email) && (userInfo.Email.ToLower() != currentUser.Email.ToLower()) &&
                     await _database.User.AnyAsync(u => u.Email == userInfo.Email.ToLower()))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail. This eMail is not unique."};
                updateError.HasError = true;
            }

            return updateError;
        }

        private async Task<NormalUserUpdateError> ValidateNormalUserInfo(User currentUser, User userInfo,
            NormalUserUpdateError prefilledErrorInfo = null)
        {
            var updateError = prefilledErrorInfo ?? new NormalUserUpdateError();

            UserConstants.Genders gender;
            if (!string.IsNullOrWhiteSpace(userInfo.Gender) && !Enum.TryParse(userInfo.Gender, true, out gender))
            {
                updateError.Gender = new List<string>
                {
                    $"Please enter a valid gender. {string.Join(",", Enum.GetNames(typeof(UserConstants.Genders)))}."
                };
                updateError.HasError = true;
            }

            if (!string.IsNullOrWhiteSpace(userInfo.Email) &&
                (new EmailAddressAttribute().IsValid(userInfo.Email) == false))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail."};
                updateError.HasError = true;
            }
            else if (!string.IsNullOrWhiteSpace(userInfo.Email) && (userInfo.Email.ToLower() != currentUser.Email.ToLower()) &&
                     await _database.User.AnyAsync(u => u.Email == userInfo.Email.ToLower()))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail. This eMail is not unique."};
                updateError.HasError = true;
            }

            return updateError;
        }

        private async Task<SignUpError> ValidateSignUpInfo(User userInfo)
        {
            var updateError = new SignUpError();

            if (string.IsNullOrWhiteSpace(userInfo.FirstName))
            {
                updateError.FirstName = new List<string> {"Please enter a first name."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.LastName))
            {
                updateError.LastName = new List<string> {"Please enter a last name."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.Email) ||
                (new EmailAddressAttribute().IsValid(userInfo.Email) == false))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail."};
                updateError.HasError = true;
            }
            else if (!string.IsNullOrWhiteSpace(userInfo.Email) &&
                     await _database.User.AnyAsync(u => u.Email == userInfo.Email.ToLower()))
            {
                updateError.Email = new List<string> {"Please enter a valid eMail. This eMail is not unique."};
                updateError.HasError = true;
            }

            if (string.IsNullOrWhiteSpace(userInfo.ReadPassword) ||
                (userInfo.ReadPassword.Length < _appSettings.MinPasswordLength))
            {
                updateError.Password = new List<string> {"Password is too short."};
                updateError.HasError = true;
            }

            if (!userInfo.Birthday.HasValue)
            {
                updateError.Birthday = new List<string> {"Please enter a birthday."};
                updateError.HasError = true;
            }

            UserConstants.Genders gender;
            if (string.IsNullOrWhiteSpace(userInfo.Gender) || !Enum.TryParse(userInfo.Gender, true, out gender))
            {
                updateError.Gender = new List<string>
                {
                    $"Please enter a valid gender. {string.Join(",", Enum.GetNames(typeof(UserConstants.Genders)))}."
                };
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

            if (!string.IsNullOrWhiteSpace(updateInfo.LastName))
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
                        Encoding.UTF8.GetBytes(_appSettings.PasswordSalt + plainPassword)));
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