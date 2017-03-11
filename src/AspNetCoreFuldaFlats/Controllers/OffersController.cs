using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using AspNetCoreFuldaFlats.Extensions;
using AspNetCoreFuldaFlats.Models;
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
        public async Task<IActionResult> GetOffer(int offerId)
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var offer = await _database.Offer.Include(o => o.DatabaseLandlord).FirstOrDefaultAsync(o => o.Id == offerId);
                if (offer != null)
                {
                    if (offer.Status != 1 && offer.Landlord != int.Parse(HttpContext.User.GetUserId()))
                    {
                        sendStatus = Unauthorized();
                    }
                    else
                    {
                        sendStatus = Ok(offer);
                    }
                }
                else
                {
                    sendStatus = NotFound();
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

        #region Recent Favorites 

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentOffers()
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var offer = await
                    _database.Offer.OrderByDescending(o => o.CreationDate)
                        .Include(o => o.DatabaseLandlord)
                        .Include(o => o.Mediaobjects)
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

        #endregion

        #region Offer Reviews

        [Authorize]
        [HttpGet("{offerId}/review")]
        public async Task<IActionResult> GetOfferReviews(int offerId)
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var offer = await _database.Offer.FirstOrDefaultAsync(o => o.Id == offerId);
                if (offer != null)
                {
                    if (offer.Status != 1 && offer.Landlord != int.Parse(HttpContext.User.GetUserId()))
                    {
                        sendStatus = Unauthorized();
                    }
                    else
                    {
                        Review[] reviews = await _database.Review.Include(r => r.User).Where(r => r.OfferId == offerId).ToArrayAsync();
                        sendStatus = Ok(reviews);
                    }
                }
                else
                {
                    sendStatus = NotFound();
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
        [HttpPost("{offerId}/review")]
        public async Task<IActionResult> CreateOfferReview([FromBody] Review review, [FromRoute] int offerId)
        {
            IActionResult sendStatus = BadRequest();

            int userId = int.Parse(HttpContext.User.GetUserId());

            bool isError = false;
            ReviewError reviewError = new ReviewError();

            if (string.IsNullOrWhiteSpace(review.Title))
            {
                reviewError.Title[0] = "Please enter a valid title.";
                isError = true;
            }

            if(string.IsNullOrWhiteSpace(review.Comment))
            {
                reviewError.Rating[0] = "Please enter a valid rating.";
                isError = true;
            }

            if (!isError)
            {
                try
                {
                    Offer offer = _database.Offer.FirstOrDefault(f => f.Id == offerId);
                    if (offer == null)
                    {
                        sendStatus = NotFound();
                    }
                    else if(offer.OfferType == "FLAT" || offer.OfferType == "SHARE")
                    {
                        reviewError.OfferType[0] = "You can not post reviews for offer with type FLAT or SHARE";
                        sendStatus = BadRequest(reviewError);
                    }
                    else if (offer.Landlord == userId)
                    {
                        reviewError.OfferType[0] = "You can not post reviews your own offer.";
                        sendStatus = BadRequest(reviewError);
                    }
                    else
                    {
                        review.OfferId = offerId;
                        review.UserId = userId;
                        review.CreationDate = DateTime.Now;
                        await _database.Review.AddAsync(review);

                        var newAverageRating = 0;
                        await _database.Review.Where(r => r.UserId == userId).ForEachAsync((rev) =>
                        {
                            newAverageRating += rev.Rating ?? 0;
                        });

                        _database.User.Update(new User()
                        {
                            Id = userId,
                            AverageRating = newAverageRating
                        });

                        await _database.SaveChangesAsync();
                        sendStatus = StatusCode(201);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    sendStatus = StatusCode(500);
                }
            }
            else
            {
                sendStatus = BadRequest(reviewError);
            }
        

            return sendStatus;
        }
        
        #endregion

        #region Offer Favorites 

        [Authorize]
        [HttpPut("{offerId}/favorite")]
        public async Task<IActionResult> SetOfferAsFavorite(int offerId)
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                if (
                    !await
                        _database.Favorite.AnyAsync(
                            f => (f.UserId.ToString() == HttpContext.User.GetUserId()) && (f.OfferId == offerId)))
                {
                    _database.Favorite.Add(new Favorite
                    {
                        OfferId = offerId,
                        UserId = int.Parse(HttpContext.User.GetUserId())
                    });
                    await _database.SaveChangesAsync();
                    sendStatus = StatusCode(201);
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
        [HttpDelete("{offerId}/favorite")]
        public async Task<IActionResult> DeleteOfferFromFavorite(int offerId)
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var favorite = await
                    _database.Favorite.FirstOrDefaultAsync(
                        f => (f.UserId.ToString() == HttpContext.User.GetUserId()) && (f.OfferId == offerId));

                if (favorite != null)
                {
                    _database.Favorite.Remove(favorite);
                    await _database.SaveChangesAsync();
                    sendStatus = Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                sendStatus = StatusCode(500);
            }

            return sendStatus;
        }

        #endregion
    }
}