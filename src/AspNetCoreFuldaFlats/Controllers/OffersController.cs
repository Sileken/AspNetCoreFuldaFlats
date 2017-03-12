using System;
using System.Collections.Generic;
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

        #region CRUD Offer
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOffer()
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = new Offer();
                offer.Landlord = int.Parse(HttpContext.User.GetUserId());
                offer.Status = 0;
                await _database.Offer.AddAsync(offer);
                await _database.SaveChangesAsync();
                response = Ok(offer);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }

        [HttpGet("{offerId}")]
        public async Task<IActionResult> GetOffer(int offerId)
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var offer = await _database.Offer
                    .Include(o => o.DatabaseLandlord)
                    .Include(o => o.MediaObjects)
                    .SingleOrDefaultAsync(o => o.Id == offerId);

                if (offer != null)
                {
                    if ((offer.Status != 1) && (offer.Landlord != int.Parse(HttpContext.User.GetUserId())))
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
        public async Task<IActionResult> UpdateOffer(int offerId)
        {
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{offerId}")]
        public async Task<IActionResult> DeleteOffer(int offerId)
        {
            IActionResult sendStatus = BadRequest();

            try
            {
                var offer = await _database.Offer.SingleOrDefaultAsync(o => o.Id == offerId);
                if (offer != null)
                {
                    if (offer.Landlord != int.Parse(HttpContext.User.GetUserId()))
                    {
                        sendStatus = StatusCode(401,
                        new DeleteOfferError
                        {
                            Offer = new List<string> { "You can only delete your own offers." }
                        });
                    }
                    else
                    {
                        _database.Remove(offer);
                        await _database.SaveChangesAsync();
                        sendStatus = StatusCode(204);
                    }
                }
                else
                {
                    sendStatus = NotFound(
                        new DeleteOfferError
                        {
                            Offer = new List<string> {"The offer was not found."}
                        });
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

        #region Search

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

        #endregion

        #region Recent Favorites 

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentOffers()
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = await
                    _database.Offer.OrderByDescending(o => o.CreationDate)
                        .Include(o => o.DatabaseLandlord)
                        .Include(o => o.MediaObjects)
                        .Take(10)
                        .ToListAsync();
                response = Ok(offer);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }

        #endregion

        #region Offer Reviews

        [Authorize]
        [HttpGet("{offerId}/review")]
        public async Task<IActionResult> GetOfferReviews(int offerId)
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = await _database.Offer.SingleOrDefaultAsync(o => o.Id == offerId);
                if (offer != null)
                {
                    if ((offer.Status != 1) && (offer.Landlord != int.Parse(HttpContext.User.GetUserId())))
                    {
                        response = Unauthorized();
                    }
                    else
                    {
                        var reviews =
                            await _database.Review.Include(r => r.User).Where(r => r.OfferId == offerId).ToArrayAsync();
                        response = Ok(reviews);
                    }
                }
                else
                {
                    response = NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }

        [Authorize]
        [HttpPost("{offerId}/review")]
        public async Task<IActionResult> CreateOfferReview([FromBody] Review review, [FromRoute] int offerId)
        {
            IActionResult response = BadRequest();

            var userId = int.Parse(HttpContext.User.GetUserId());

            var isError = false;
            var reviewError = new ReviewError();

            if (await _database.Review.AnyAsync(r => (r.UserId == userId) && (r.OfferId == offerId)))
            {
                reviewError.Review = new List<string> {"You can only post one review per offer"};
                isError = true;
            }

            if (string.IsNullOrWhiteSpace(review.Title))
            {
                reviewError.Title = new List<string> {"Please enter a valid title."};
                isError = true;
            }

            if (review.Rating == null)
            {
                reviewError.Rating = new List<string> {"Please enter a valid rating."};
                isError = true;
            }

            if (!isError)
            {
                try
                {
                    var offer =
                        await
                            _database.Offer.Include(o => o.DatabaseLandlord)
                                .SingleOrDefaultAsync(f => f.Id == offerId);
                    if (offer == null)
                    {
                        response = NotFound();
                    }
                    else if ((offer.OfferType == "FLAT") || (offer.OfferType == "SHARE"))
                    {
                        reviewError.OfferType = new List<string>
                        {
                            "You can not post reviews for offer with type FLAT or SHARE"
                        };
                        response = BadRequest(reviewError);
                    }
                    else if (offer.Landlord == userId)
                    {
                        reviewError.OfferType = new List<string> {"You can not post reviews your own offer."};
                        response = BadRequest(reviewError);
                    }
                    else
                    {
                        review.OfferId = offerId;
                        review.UserId = userId;
                        review.CreationDate = DateTime.Now;
                        await _database.Review.AddAsync(review);
                        await _database.SaveChangesAsync();

                        var newAverageRating = 0;
                        var reviews = 0;
                        foreach (
                            var offer2 in
                            await _database.Offer.Where(o => o.Landlord == offer.Landlord).ToListAsync())
                        {
                            await _database.Review.Where(r => r.OfferId == offer2.Id)
                                .ForEachAsync(rev =>
                                {
                                    newAverageRating += rev.Rating ?? 0;
                                    reviews++;
                                });
                        }

                        var offerLandlord = offer.DatabaseLandlord;
                        offerLandlord.AverageRating = reviews == 0 ? 0 : newAverageRating/reviews;
                        _database.User.Update(offerLandlord);

                        await _database.SaveChangesAsync();
                        response = StatusCode(201);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode(500);
                }
            }
            else
            {
                response = BadRequest(reviewError);
            }

            return response;
        }

        #endregion

        #region Offer Favorites 

        [Authorize]
        [HttpPut("{offerId}/favorite")]
        public async Task<IActionResult> SetOfferAsFavorite(int offerId)
        {
            IActionResult response = BadRequest();

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
                    response = StatusCode(201);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }


        [Authorize]
        [HttpDelete("{offerId}/favorite")]
        public async Task<IActionResult> DeleteOfferFromFavorite(int offerId)
        {
            IActionResult response = BadRequest();

            try
            {
                var favorite = await
                    _database.Favorite.SingleOrDefaultAsync(
                        f => (f.UserId.ToString() == HttpContext.User.GetUserId()) && (f.OfferId == offerId));

                if (favorite != null)
                {
                    _database.Favorite.Remove(favorite);
                    await _database.SaveChangesAsync();
                    response = Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode(500);
            }

            return response;
        }

        #endregion
    }
}