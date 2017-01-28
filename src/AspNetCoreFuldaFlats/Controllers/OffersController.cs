using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreFuldaFlats.Controllers
{
    [Route("api/[controller]")]
    public class OffersController : Controller
    {
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public OffersController(WebApiDataContext webApiDataContext, ILogger<UsersController> logger)
        {
            _database = webApiDataContext;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOffer()
        {
            return BadRequest();
        }

        [HttpGet("{offerId}")]
        public async Task<IActionResult> GetOffer()
        {
            return BadRequest();
        }

        [Authorize]
        [HttpPut("{offerId}")]
        public async Task<IActionResult> UpdateOffer(string OfferId)
        {
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{offerId}")]
        public async Task<IActionResult> DeleteOffer(string OfferId)
        {
            return BadRequest();
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchOffers()
        {
            return BadRequest();
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetLastSearchResult()
        {
            return BadRequest();
        }

        [HttpGet("search/last")]
        public async Task<IActionResult> GetLastSearchParameter()
        {
            return BadRequest();
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentOffers()
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var offer = await 
                    _database.Offer.OrderByDescending(o => o.CreationDate)
                       // .Include(o => o.DatabaseLandlord)
                        //.Include(o => o.Mediaobjects)
                        .Take(10)
                        .ToListAsync();
                sendStatus = Ok(offer);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                sendStatus = StatusCode(500);
            }


            return sendStatus;
        }

        //CRUD Review und CRUD Favorite
    }
}