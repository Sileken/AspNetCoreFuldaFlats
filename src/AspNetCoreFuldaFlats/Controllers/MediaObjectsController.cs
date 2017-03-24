using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using AspNetCoreFuldaFlats.Extensions;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreFuldaFlats.Controllers
{
    [Route("api/[controller]")]
    public class MediaObjectsController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public MediaObjectsController(IOptions<AppSettings> appSettingsOptions, WebApiDataContext webApiDataContext,
            ILogger<MediaObjectsController> logger, IHostingEnvironment environment)
        {
            _appSettings = appSettingsOptions.Value;
            _database = webApiDataContext;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet("{offerId}")]
        public async Task<IActionResult> GetMediaObjects(int offerId)
        {
            IActionResult response = BadRequest();

            try
            {
                var mediaobjects = await _database.Mediaobject.Where(m => m.OfferId == offerId).ToListAsync();
                return Ok(mediaobjects);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [Authorize]
        [HttpDelete("{mediaObjectId}")]
        public async Task<IActionResult> DeleteMediaObject(int mediaObjectId)
        {
            IActionResult response = BadRequest();

            try
            {
                var mediaobject = await _database.Mediaobject.SingleOrDefaultAsync(m => m.Id == mediaObjectId);
                if (mediaobject?.Offer == null)
                {
                    response = NotFound(
                        new DeleteOfferError
                        {
                            Offer = new List<string> {"The media object was not found."}
                        });
                }
                else
                {
                    if (mediaobject.CreatedByUserId != HttpContext.User.GetUserId())
                    {
                        response = StatusCode((int)HttpStatusCode.Unauthorized,
                            new DeleteOfferError
                            {
                                Offer = new List<string> {"You can only delete your own offer media objects."}
                            });
                    }
                    else
                    {
                        IFileInfo fileInfo = _environment.ContentRootFileProvider.GetFileInfo(mediaobject.MainUrl);
                        if (fileInfo.Exists)
                        {
                            System.IO.File.Delete(fileInfo.PhysicalPath);
                        }
                        _database.Remove(mediaobject);
                        await _database.SaveChangesAsync();
                        response = NoContent();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return response;
        }
    }
}
