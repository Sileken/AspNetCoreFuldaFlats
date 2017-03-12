using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreFuldaFlats.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class TagsController : Controller
    {
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public TagsController(WebApiDataContext webApiDataContext, ILogger<TagsController> logger)
        {
            _database = webApiDataContext;
            _logger = logger;
        }

        [HttpGet]
        public string[] GetTags()
        {
            return new string[]
            {
                //Languages
                "english",
                "german",
                "french",
                "spanish",
                "italian",
                "portuguese",
                "turkish",
                "russian",
                "ukrainian",
                "persian",
                "arabic",
                "japanese",
                "chinese",

                //Faculties
                "computer science",
                "electrical engineering",
                "food technology",
                "nutritional sciences",
                "nursing and health sciences",
                "social and cultural sciences",
                "social work",
                "business economics",
                "accounting, finance, controlling",
                "dietetics",
                "nutritional psychology",
                "food processing",
                "health management",
                "global software development",
                "intercultural communication and european studies",
                "international food business and consumer studies",
                "international management",
                "oecotrophology",
                "public health",
                "public health nutrition",
                "supply chain management",
                "sustainable food systems",

                //Interests
                "football",
                "table tennis",
                "music",
                "computer games",
                "party",
                "beerpong",
                "bowling",
                "darts",
                "drone racing",
                "cooking",
                "travel",
                "art",
                "dancing",
                "free-climbing",
                "bodybuilding",
                "yoga",
                "photography",

                //Sport
                "basketball",
                "soccer",
                "handball",
                "extreme sports",
                "esports",

                //Mitbewohner
                "only men",
                "only woman",
                "under 25",
                "under 30",
                "over 30",
                "singles",

                //Sexuality
                "heterosexual",
                "homosexual",
                "bisexual",

                //Religions
                "catholic",
                "evangelical",
                "orthodox",
                "atheist",
                "islam",
                "hinduism",
                "buddhism",
                "judaism",
                "bahai"
            };
        }     
    }
}
