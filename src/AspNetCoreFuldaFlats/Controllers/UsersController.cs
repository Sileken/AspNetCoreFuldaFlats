using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Database;
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

        [HttpPost()]
        public void Register()
        {
        }

        [HttpPost("auth")]
        public async Task<IActionResult> SigIn([FromBody] SignInData signInData)
        {
            IActionResult sendStatus = StatusCode(403); //sollte eigentlich 401 sein ! => Unauthorized();

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
                        var passwordHash =
                            Convert.ToBase64String(
                                SHA512.Create()
                                    .ComputeHash(
                                        Encoding.UTF8.GetBytes(GlobalConstants.PasswordSalt + password)));

                        if ((user.IsLocked != 1) && (user.Password == passwordHash))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.Email, ClaimValueTypes.Email,
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

                            user.LoginAttempts = 0;

                            _database.Entry(user)
                                .Collection(u => u.DatabaseFavorites)
                                .Query()
                                .Include(f => f.Offer);

                            sendStatus = Ok(user);
                        }
                        else if (user.IsLocked != 1)
                        {
                            user.LoginAttempts = user.LoginAttempts ?? 0;
                            if (user.LoginAttempts + 1 < GlobalConstants.MaxSignInAttempts)
                            {
                                user.LoginAttempts++;
                            }
                            else
                            {
                                user.LoginAttempts++;
                                user.IsLocked = 1;
                                sendStatus = StatusCode(423);
                            }
                        }

                        _database.User.Update(user);
                        await _database.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    sendStatus = StatusCode(500);
                }
            }

            return sendStatus;
        }

        [HttpDelete("auth")]
        public async Task<IActionResult> SignUp()
        {
            IActionResult sendStatus = StatusCode(204);

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
                sendStatus = StatusCode(500);
            }


            return sendStatus;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            IActionResult sendStatus = StatusCode(403);

            try
            {
                var user = await
                    _database.User.Include(u => u.DatabaseFavorites)
                        .ThenInclude(f => f.Offer)
                        .SingleOrDefaultAsync(u => u.Email == HttpContext.User.Identity.Name);

                if (user != null)
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            IActionResult sendStatus = StatusCode(403);

            try
            {
                var user = await
                    _database.User.Include(u => u.DatabaseFavorites)
                        .ThenInclude(f => f.Offer)
                        .SingleOrDefaultAsync(u => u.Id == id);

                if (user != null)
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

        [HttpPut("upgrade")]
        public void UpgradeUser()
        {
        }

        [HttpPut("/changePassword")]
        public void ChangePassword()
        {
        }

        [Authorize]
        [HttpPut("standardPicture")]
        public async Task<IActionResult> SetProfilePicture([FromBody] SetProfilePictureData profilePictureData)
        {
            IActionResult sendStatus = BadRequest();

            if (!string.IsNullOrWhiteSpace(profilePictureData?.Img))
            {
                try
                {
                    var user = await _database.User.SingleOrDefaultAsync(u => u.Email == HttpContext.User.Identity.Name);

                    if (user != null)
                    {
                        user.ProfilePicture = profilePictureData.Img;

                        _database.User.Update(user);
                        await _database.SaveChangesAsync();

                        sendStatus = Ok(user);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    sendStatus = StatusCode(500);
                }
            }

            return sendStatus;
        }
    }
}