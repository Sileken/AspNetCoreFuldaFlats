using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreFuldaFlats.Controllers
{
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public FilesController(IOptions<AppSettings> appSettingsOptions, WebApiDataContext webApiDataContext,
            ILogger<FilesController> logger, IHostingEnvironment environment)
        {
            _appSettings = appSettingsOptions.Value;
            _database = webApiDataContext;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("offer/{offerId}")]
        public async Task<IActionResult> UploadOfferFile(int offerId)
        {
            IActionResult response = BadRequest();

            try
            {
                //var userId = int.Parse(HttpContext.User.GetUserId());
                //Mediaobject mediaobject = await
                //    _database.Mediaobject.SingleOrDefaultAsync(m => m.UserId == userId);

                //if (mediaobject == null)
                //{
                //    mediaobject = new Mediaobject
                //    {
                //        CreatedByUserId = userId,
                //        CreationDate = DateTime.Now,

                //    };
                //}
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }

        [Authorize]
        [HttpPost("profilePicture")]
        public async Task<IActionResult> UploadProfilePicutre(IFormFile file)
        {
            IActionResult response = BadRequest();

            try
            {
                if (file == null)
                {
                    response = NotFound(new UploadError() { Error = "File was not specified" });
                }
                else if (file.Length > _appSettings.MaxUploadFileSizeInBytes)
                {
                    response = BadRequest(new UploadError() {Error = "This image exceeds the size limit of 5 MB!"});
                }
                else if (_appSettings.SupportedImageContentTypes.Contains(file.ContentType))
                {
                       response = BadRequest(new UploadError() { Error = $"Unsupported picture format. Supported formats are {String.Join(",",_appSettings.SupportedImageContentTypes)}." });
                }
                else
                {
                    var user = await _database.User.SingleOrDefaultAsync(u => u.Email == HttpContext.User.Identity.Name);

                    var fileName = $"user_{user.Id}_profile_picture" + file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase));
                    var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", fileName);
                    using (var fileStream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    user.ProfilePicture = "/uploads/" + fileName;
                    _database.Update(user);
                    await _database.SaveChangesAsync();
                    response = Redirect("/api/users/me");
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }
    }
}
