using System.Collections.Generic;
using System.Net;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCoreFuldaFlats.Controllers
{
    /// <summary>
    ///     Endpoints for tag functions.
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class TagsController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public TagsController(IOptions<AppSettings> appSettingsOptions, WebApiDataContext webApiDataContext,
            ILogger<TagsController> logger)
        {
            _appSettings = appSettingsOptions.Value;
            _database = webApiDataContext;
            _logger = logger;
        }

        /// <summary>
        ///     Get a list of all available tags for filtering.
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(List<string>))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<string>))]
        [HttpGet]
        public IActionResult GetTags()
        {
            return Ok(_appSettings.SupportedTags);
        }
    }
}
