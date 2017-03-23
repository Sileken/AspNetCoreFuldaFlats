using System.Collections.Generic;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreFuldaFlats.Controllers
{
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

        [HttpGet]
        public List<string> GetTags()
        {
            return _appSettings.SupportedTags;
        }
    }
}
