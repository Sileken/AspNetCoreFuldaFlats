using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCoreFuldaFlats.Controllers
{
    /// <summary>
    ///     Endpoints for media object functions.
    /// </summary>
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

        /// <summary>
        ///     Get a offer media object.
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [Produces(typeof(List<Mediaobject>))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Mediaobject>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
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
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Delete a media object.
        /// </summary>
        /// <param name="mediaObjectId"></param>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized, Type = typeof(DeleteOfferError))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(DeleteOfferError))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [Authorize]
        [HttpDelete("{mediaObjectId}")]
        public async Task<IActionResult> DeleteMediaObject(int mediaObjectId)
        {
            IActionResult response = BadRequest();

            try
            {
                var mediaobject =
                    await _database.Mediaobject.Include(m => m.Offer).SingleOrDefaultAsync(m => m.Id == mediaObjectId);
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
                        response = StatusCode((int) HttpStatusCode.Unauthorized,
                            new DeleteOfferError
                            {
                                Offer = new List<string> {"You can only delete your own offer media objects."}
                            });
                    }
                    else
                    {
                        var fileInfo = _environment.ContentRootFileProvider.GetFileInfo(mediaobject.MainUrl);
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
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }
    }
}
