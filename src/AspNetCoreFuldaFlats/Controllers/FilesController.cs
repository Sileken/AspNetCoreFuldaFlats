﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using AspNetCoreFuldaFlats.Extensions;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        public async Task<IActionResult> UploadOfferFile([FromRoute] int offerId, [FromBody] IFormFile file)
        {
            IActionResult response = BadRequest();

            try
            {
                if (file == null)
                {
                    response = NotFound(new UploadError {Error = "File was not specified"});
                }
                else if (file.Length > _appSettings.MaxUploadFileSizeInBytes)
                {
                    response = BadRequest(new UploadError {Error = "This image exceeds the size limit of 5 MB!"});
                }
                else if (!_appSettings.SupportedImageContentTypes.Contains(file.ContentType))
                {
                    response =
                        BadRequest(new UploadError
                        {
                            Error =
                                $"Unsupported picture format. Supported formats are {string.Join(",", _appSettings.SupportedImageContentTypes)}."
                        });
                }
                else
                {
                    var offer = await _database.Offer
                        .SingleOrDefaultAsync(o => o.Id == offerId);

                    if (offer == null)
                    {
                        response = NotFound();
                    }
                    else if (offer.Landlord != HttpContext.User.GetUserId())
                    {
                        response = StatusCode((int)HttpStatusCode.Unauthorized,
                            new UploadError {Error = "Only the landlord can upload images for this offer"});
                    }
                    else
                    {
                        var newMediaObject = new Mediaobject
                        {
                            Type = (int) GlobalConstants.MediaObjectTypes.Image,
                            CreatedByUserId = HttpContext.User.GetUserId(),
                            OfferId = offer.Id
                        };

                        await _database.Mediaobject.AddAsync(newMediaObject);
                        await _database.SaveChangesAsync();

                        var fileName =
                            $"offer_{offer.Id}_media_{newMediaObject.Id}_{GlobalConstants.MediaObjectTypes.Image}" +
                            file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase));
                        var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", fileName);
                        var mediaObjectRelativeUrl = "/uploads/" + fileName;

                        using (var fileStream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        newMediaObject.MainUrl = mediaObjectRelativeUrl;
                        newMediaObject.ThumbnailUrl = mediaObjectRelativeUrl;

                        _database.Update(newMediaObject);
                        await _database.SaveChangesAsync();
                        response = StatusCode((int)HttpStatusCode.Created, newMediaObject);
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

        [Authorize]
        [HttpPost("profilePicture")]
        public async Task<IActionResult> UploadProfilePicutre(IFormFile file)
        {
            IActionResult response = BadRequest();

            try
            {
                if (file == null)
                {
                    response = NotFound(new UploadError {Error = "File was not specified"});
                }
                else if (file.Length > _appSettings.MaxUploadFileSizeInBytes)
                {
                    response = BadRequest(new UploadError {Error = "This image exceeds the size limit of 5 MB!"});
                }
                else if (!_appSettings.SupportedImageContentTypes.Contains(file.ContentType))
                {
                    response =
                        BadRequest(new UploadError
                        {
                            Error =
                                $"Unsupported picture format. Supported formats are {string.Join(",", _appSettings.SupportedImageContentTypes)}."
                        });
                }
                else
                {
                    var user = await _database.User.SingleOrDefaultAsync(u => u.Email == HttpContext.User.Identity.Name);

                    var fileName = $"user_{user.Id}_profile_picture" +
                                   file.FileName.Substring(file.FileName.LastIndexOf(".",
                                       StringComparison.OrdinalIgnoreCase));
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
                response = StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return response;
        }
    }
}
