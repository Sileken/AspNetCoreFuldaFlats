using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using AspNetCoreFuldaFlats.Extensions;
using AspNetCoreFuldaFlats.Models;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCoreFuldaFlats.Controllers
{
    /// <summary>
    ///     Endpoints offer functions.
    /// </summary>
    [Route("api/[controller]")]
    public class OffersController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly WebApiDataContext _database;
        private readonly ILogger _logger;

        public OffersController(IOptions<AppSettings> appSettingsOptions, WebApiDataContext webApiDataContext,
            ILogger<OffersController> logger)
        {
            _appSettings = appSettingsOptions.Value;
            _database = webApiDataContext;
            _logger = logger;
        }

        #region Recent Offers 

        /// <summary>
        ///     Get list of recent offers.
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(List<Offer>))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Offer>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentOffers()
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = await
                    _database.Offer
                        .Where(o => o.Status == (int) GlobalConstants.OfferStatus.Active)
                        .OrderByDescending(o => o.LastModified)
                        .Include(o => o.DatabaseLandlord)
                        .Include(o => o.MediaObjects)
                        .Include(o => o.Tags)
                        .Take(10)
                        .ToListAsync();
                response = Ok(offer);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        #endregion

        #region CRUD Offer

        /// <summary>
        ///     Create a new offer with given JSON data in the body.Requires the account to be upgraded to landlord.
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(Offer))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(Offer))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOffer()
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = new Offer
                {
                    Landlord = HttpContext.User.GetUserId(),
                    Status = (int) GlobalConstants.OfferStatus.Inactive,
                    CreationDate = DateTime.Now,
                    LastModified = DateTime.Now
                };
                await PersistOffer(offer);

                var reloadedOffer = await _database.Offer
                    .Include(o => o.DatabaseLandlord)
                    .Include(o => o.MediaObjects)
                    .Include(o => o.Tags)
                    .SingleOrDefaultAsync(o => o.Id == offer.Id);

                response = Ok(reloadedOffer);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Get detailed information on the offer with given :id. (More information when session is authenticated).
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [Produces(typeof(Offer))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(Offer))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [HttpGet("{offerId}")]
        public async Task<IActionResult> GetOffer(int offerId)
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = await _database.Offer
                    .Include(o => o.DatabaseLandlord)
                    .Include(o => o.MediaObjects)
                    .Include(o => o.Tags)
                    .SingleOrDefaultAsync(o => o.Id == offerId);

                if (offer != null)
                {
                    if ((offer.Status != (int) GlobalConstants.OfferStatus.Active) &&
                        (offer.Landlord != HttpContext.User.GetUserId()))
                    {
                        response = Unauthorized();
                    }
                    else
                    {
                        //hot fix for client offer details page
                        if ((offer.MediaObjects == null) || (offer.MediaObjects.Count == 0))
                        {
                            offer.MediaObjects = new List<Mediaobject>
                            {
                                new Mediaobject
                                {
                                    MainUrl = _appSettings.DefaultThumbnailUrl,
                                    ThumbnailUrl = _appSettings.DefaultThumbnailUrl
                                }
                            };
                        }

                        if (HttpContext.User.Identity.IsAuthenticated)
                        {
                            offer.Favorite = await _database.Favorite.Where(
                                f => (f.UserId == HttpContext.User.GetUserId()) && (f.OfferId == offer.Id)).ToListAsync();
                        }

                        response = Ok(offer);
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
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Manipulate the data of the offer with given :id with given JSON data in the body under the prerequisite that the
        ///     offer is owned by the currently sign in user.
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="offerUpdateInfo"></param>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(OfferUpdateError))]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [Authorize]
        [HttpPut("{offerId}")]
        public async Task<IActionResult> UpdateOffer(int offerId, [FromBody] OfferUpdateInfo offerUpdateInfo)
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = await _database.Offer
                    .SingleOrDefaultAsync(o => o.Id == offerId);

                if (offer != null)
                {
                    if (offer.Landlord != HttpContext.User.GetUserId())
                    {
                        response = Unauthorized();
                    }
                    else
                    {
                        var offerUpdateInfoError = ValidateOfferUpdateInfo(offer, offerUpdateInfo);
                        if (offerUpdateInfoError.HasError)
                        {
                            response = BadRequest(offerUpdateInfoError);
                        }
                        else
                        {
                            await UpdateOfferProperties(offer, offerUpdateInfo);
                            await UpdateOffer(offer);
                            await CreateOffereRelatedTags(offer, offerUpdateInfo);
                            response = Ok();
                        }
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
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Delete the offer with given id und the prerequisite that the offer is owned by the currently sign in user.
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(DeleteOfferError))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized, Type = typeof(OfferUpdateError))]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [Authorize]
        [HttpDelete("{offerId}")]
        public async Task<IActionResult> DeleteOffer(int offerId)
        {
            IActionResult response = BadRequest();

            try
            {
                var offer = await _database.Offer.SingleOrDefaultAsync(o => o.Id == offerId);
                if (offer != null)
                {
                    if (offer.Landlord != HttpContext.User.GetUserId())
                    {
                        response = StatusCode(401,
                            new DeleteOfferError
                            {
                                Offer = new List<string> {"You can only delete your own offers."}
                            });
                    }
                    else
                    {
                        _database.Remove(offer);
                        await _database.SaveChangesAsync();
                        response = NoContent();
                    }
                }
                else
                {
                    response = NotFound(
                        new DeleteOfferError
                        {
                            Offer = new List<string> {"The offer was not found."}
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        #endregion

        #region Search

        /// <summary>
        ///     Send a offer search query.
        /// </summary>
        /// <param name="searchParamaters"></param>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(string))]
        [HttpPost("search")]
        public IActionResult SearchOffers([FromBody] SearchParamaters searchParamaters)
        {
            IActionResult response = BadRequest();

            try
            {
                if (searchParamaters == null)
                {
                    BadRequest("Invalid search parameters");
                }
                else
                {
                    HttpContext.Session.SetString(GlobalConstants.SearchParamtersSessionkey,
                        JsonConvert.SerializeObject(searchParamaters));
                    response = NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Get search result from last sended offer search query.
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(List<Offer>))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Offer>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [HttpGet("search")]
        public async Task<IActionResult> GetLastSearchResult()
        {
            IActionResult response = BadRequest();

            try
            {
                var lastSearchParameterString = HttpContext.Session.GetString(GlobalConstants.SearchParamtersSessionkey);
                if (string.IsNullOrWhiteSpace(lastSearchParameterString))
                {
                    response = NotFound();
                }
                else
                {
                    var lastSearchParameters =
                        JsonConvert.DeserializeObject<SearchParamaters>(lastSearchParameterString);

                    var offersQuery = await BuildOfferSearchQuery(lastSearchParameters);
                    List<Offer> offerResult;
                    if (offersQuery != null)
                    {
                        offerResult = await offersQuery
                            .OrderByDescending(o => o.LastModified)
                            .Include(o => o.DatabaseLandlord)
                            .Include(o => o.MediaObjects)
                            .Include(o => o.Tags)
                            .ToListAsync();
                    }
                    else
                    {
                        offerResult = new List<Offer>();
                    }

                    response = Ok(offerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Get last sended offer search query.
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [HttpGet("search/last")]
        public IActionResult GetLastSearchParameter()
        {
            IActionResult response = BadRequest();

            try
            {
                var lastSearchParameterString = HttpContext.Session.GetString(GlobalConstants.SearchParamtersSessionkey);
                if (string.IsNullOrWhiteSpace(lastSearchParameterString))
                {
                    response = NotFound();
                }
                else
                {
                    var lastSearchParamaters =
                        JsonConvert.DeserializeObject<SearchParamaters>(lastSearchParameterString);
                    response = Ok(lastSearchParamaters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        #endregion

        #region Offer Reviews

        /// <summary>
        ///     Get offer reviews.
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [Produces(typeof(List<Review>))]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Review>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
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
                    if ((offer.Status != 1) && (offer.Landlord != HttpContext.User.GetUserId()))
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
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Post a review for the offer with the given id when a user is sign in.
        /// </summary>
        /// <param name="review"></param>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.Created)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, typeof(ReviewError))]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [Authorize]
        [HttpPost("{offerId}/review")]
        public async Task<IActionResult> CreateOfferReview([FromBody] Review review, [FromRoute] int offerId)
        {
            IActionResult response = BadRequest();

            var userId = HttpContext.User.GetUserId();

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
                        response = StatusCode((int) HttpStatusCode.Created);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(null, ex, "Unexpected Issue.");
                    response = StatusCode((int) HttpStatusCode.InternalServerError);
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

        /// <summary>
        ///     Set offer as my favorite.
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [SwaggerResponse((int) HttpStatusCode.Created)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
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
                            f => (f.UserId == HttpContext.User.GetUserId()) && (f.OfferId == offerId)))
                {
                    _database.Favorite.Add(new Favorite
                    {
                        OfferId = offerId,
                        UserId = HttpContext.User.GetUserId()
                    });
                    await _database.SaveChangesAsync();
                    response = StatusCode((int) HttpStatusCode.Created);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        /// <summary>
        ///     Delete a offer favorite.
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [Produces(typeof(List<Review>))]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError)]
        [SwaggerResponse((int) HttpStatusCode.Unauthorized)]
        [Authorize]
        [HttpDelete("{offerId}/favorite")]
        public async Task<IActionResult> DeleteOfferFromFavorite(int offerId)
        {
            IActionResult response = BadRequest();

            try
            {
                var favorite = await
                    _database.Favorite.SingleOrDefaultAsync(
                        f => (f.UserId == HttpContext.User.GetUserId()) && (f.OfferId == offerId));

                if (favorite != null)
                {
                    _database.Favorite.Remove(favorite);
                    await _database.SaveChangesAsync();
                }

                response = Ok();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(null, ex, "Unexpected Issue.");
                response = StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return response;
        }

        #endregion

        #region Helper Functions

        private async Task UpdateOffer(Offer offer)
        {
            _database.Offer.Update(offer);
            await _database.SaveChangesAsync();
        }

        private async Task PersistOffer(Offer offer)
        {
            await _database.Offer.AddAsync(offer);
            await _database.SaveChangesAsync();
        }

        private OfferUpdateError ValidateOfferUpdateInfo(Offer currentOffer, OfferUpdateInfo offerUpdateInfo)
        {
            var offerUpdateError = new OfferUpdateError();

            if (offerUpdateInfo == null)
            {
                offerUpdateError.Id = new List<string> {"Offer update data are invalid."};
                offerUpdateError.HasError = true;
            }

            // Client does not send always the id
            //else
            //{
            //if (currentOffer.Id != offerUpdateInfo.Id)
            //{
            //    offerUpdateError.Id = new List<string>
            //    {
            //        "Offer id in the url query and post object are not equivalent."
            //    };
            //    offerUpdateError.HasError = true;
            //}
            //}

            return offerUpdateError;
        }

        private async Task UpdateOfferProperties(Offer currentOffer, OfferUpdateInfo offerUpdateInfo)
        {
            if ((offerUpdateInfo.Accessability != null) || (currentOffer.Accessability == null))
            {
                currentOffer.Accessability = offerUpdateInfo.Accessability == true;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.BathroomDescription))
            {
                currentOffer.BathroomDescription = offerUpdateInfo.BathroomDescription;
            }

            if ((offerUpdateInfo.BathroomNumber != null) || (currentOffer.BathroomNumber == null))
            {
                currentOffer.BathroomNumber = offerUpdateInfo.BathroomNumber ?? 0;
            }

            if ((offerUpdateInfo.Cellar != null) || (currentOffer.Cellar == null))
            {
                currentOffer.Cellar = offerUpdateInfo.Cellar == true;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.City))
            {
                currentOffer.City = offerUpdateInfo.City;
            }

            if ((offerUpdateInfo.Commission != null) || (currentOffer.Commission == null))
            {
                currentOffer.Commission = offerUpdateInfo.Commission ?? 0;
            }

            if ((offerUpdateInfo.Deposit != null) || (currentOffer.Deposit == null))
            {
                currentOffer.Deposit = offerUpdateInfo.Deposit ?? 0;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.Description))
            {
                currentOffer.Description = offerUpdateInfo.Description;
            }

            if ((offerUpdateInfo.Dryer != null) || (currentOffer.Dryer == null))
            {
                currentOffer.Dryer = offerUpdateInfo.Dryer == true;
            }

            if ((offerUpdateInfo.Elevator != null) || (currentOffer.Elevator == null))
            {
                currentOffer.Elevator = offerUpdateInfo.Elevator == true;
            }

            if ((offerUpdateInfo.Floor != null) || (currentOffer.Floor == null))
            {
                currentOffer.Floor = offerUpdateInfo.Floor ?? 0;
            }

            if ((offerUpdateInfo.Furnished != null) || (currentOffer.Furnished == null))
            {
                currentOffer.Furnished = offerUpdateInfo.Furnished == true;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.HeatingDescription))
            {
                currentOffer.HeatingDescription = offerUpdateInfo.HeatingDescription;
            }

            if (offerUpdateInfo.HouseNumber != null)
            {
                currentOffer.HouseNumber = offerUpdateInfo.HouseNumber ?? 0;
            }

            if ((offerUpdateInfo.InternetSpeed != null) || (currentOffer.InternetSpeed == null))
            {
                currentOffer.InternetSpeed = offerUpdateInfo.InternetSpeed ?? 0;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.KitchenDescription))
            {
                currentOffer.KitchenDescription = offerUpdateInfo.KitchenDescription;
            }

            if ((offerUpdateInfo.Lan != null) || (currentOffer.Lan == null))
            {
                currentOffer.Lan = offerUpdateInfo.Lan == true;
            }

            currentOffer.LastModified = DateTime.Now;


            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.OfferType))
            {
                currentOffer.OfferType = offerUpdateInfo.OfferType;
            }

            if ((offerUpdateInfo.Parking != null) || (currentOffer.Parking == null))
            {
                currentOffer.Parking = offerUpdateInfo.Parking == true;
            }

            if ((offerUpdateInfo.Pets != null) || (currentOffer.Pets == null))
            {
                currentOffer.Pets = offerUpdateInfo.Pets == true;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.PriceType))
            {
                currentOffer.PriceType = offerUpdateInfo.PriceType;
            }

            if ((offerUpdateInfo.Rent != null) || (currentOffer.Rent == null))
            {
                currentOffer.Rent = offerUpdateInfo.Rent ?? 0;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.RentType))
            {
                currentOffer.RentType = offerUpdateInfo.RentType;
            }

            if ((offerUpdateInfo.Rooms != null) || (currentOffer.Rooms == null))
            {
                currentOffer.Rooms = offerUpdateInfo.Rooms ?? 0;
            }

            if ((offerUpdateInfo.SideCosts != null) || (currentOffer.SideCosts == null))
            {
                currentOffer.SideCosts = offerUpdateInfo.SideCosts ?? 0;
            }

            if ((offerUpdateInfo.Size != null) || (currentOffer.Size == null))
            {
                currentOffer.Size = offerUpdateInfo.Size ?? 0;
            }

            if ((offerUpdateInfo.Status != null) || (currentOffer.Status == null))
            {
                currentOffer.Status = offerUpdateInfo.Status ?? 0;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.Street))
            {
                currentOffer.Street = offerUpdateInfo.Street;
            }

            if ((offerUpdateInfo.Telephone != null) || (currentOffer.Telephone == null))
            {
                currentOffer.Telephone = offerUpdateInfo.Telephone == true;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.Television))
            {
                currentOffer.Television = offerUpdateInfo.Television;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.Title))
            {
                currentOffer.Title = offerUpdateInfo.Title;
            }

            if ((offerUpdateInfo.WashingMachine != null) || (currentOffer.WashingMachine == null))
            {
                currentOffer.WashingMachine = offerUpdateInfo.WashingMachine == true;
            }

            if ((offerUpdateInfo.Wlan != null) || (currentOffer.Wlan == null))
            {
                currentOffer.Wlan = offerUpdateInfo.Wlan == true;
            }

            if (!string.IsNullOrWhiteSpace(offerUpdateInfo.ZipCode))
            {
                currentOffer.ZipCode = offerUpdateInfo.ZipCode;
            }

            if ((offerUpdateInfo.Status != null) &&
                ((int[]) Enum.GetValues(typeof(GlobalConstants.OfferStatus))).Any(i => i == offerUpdateInfo.Status))
            {
                currentOffer.Status = offerUpdateInfo.Status;
            }

            if ((offerUpdateInfo.FullPrice != null) || (currentOffer.FullPrice == null))
            {
                var fullPrice = 0;

                if (offerUpdateInfo.Rent != null)
                {
                    fullPrice += (int) offerUpdateInfo.Rent;
                }
                if (offerUpdateInfo.SideCosts != null)
                {
                    fullPrice += (int) offerUpdateInfo.SideCosts;
                }

                currentOffer.FullPrice = fullPrice;
            }

            if (!string.IsNullOrWhiteSpace(currentOffer.Street) && (currentOffer.HouseNumber != null) &&
                !string.IsNullOrWhiteSpace(currentOffer.ZipCode) && !string.IsNullOrWhiteSpace(currentOffer.City) &&
                (currentOffer.UniDistance == null))
            {
                try
                {
                    var coordinate = await GetOfferGeoCoordinates(currentOffer);
                    if (coordinate != null)
                    {
                        currentOffer.Latitude = coordinate.Latitude;
                        currentOffer.Longitude = coordinate.Longitude;
                        currentOffer.UniDistance =
                            Math.Round(coordinate.GetDistanceTo(_appSettings.HsFuldaCoordinate)/1000*100)/100;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error while requesting offer geo coordinate", ex);
                }
            }
        }

        private async Task CreateOffereRelatedTags(Offer currentOffer, OfferUpdateInfo offerUpdateInfo)
        {
            var addedTag = false;

            foreach (var tag in offerUpdateInfo.Tags)
            {
                if (!await _database.Tag.AnyAsync(t => (t.OfferId == currentOffer.Id) && (t.Title == tag)))
                {
                    await _database.Tag.AddAsync(new Tag {OfferId = currentOffer.Id, Title = tag});
                    addedTag = true;
                }
            }

            if (addedTag)
            {
                await _database.SaveChangesAsync();
            }
        }

        private async Task<GeoCoordinate> GetOfferGeoCoordinates(Offer offer)
        {
            GeoCoordinate offerCoordinate = null;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestUrl =
                $"{_appSettings.OpenStreetMapSearchApi.TrimEnd('/')}?street={UrlEncoder.Default.Encode($"{offer.Street} {offer.HouseNumber}")}&postalcode={UrlEncoder.Default.Encode(offer.ZipCode)}&city={UrlEncoder.Default.Encode(offer.City)}&format=json";

            var response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var openStreetMapSearchResults =
                    JsonConvert.DeserializeObject<List<OpenStreetMapSearchResult>>(responseBody);
                if ((openStreetMapSearchResults != null) && (openStreetMapSearchResults.Count > 0))
                {
                    var openStreetMapSearchResult = openStreetMapSearchResults.First();
                    offerCoordinate = new GeoCoordinate(openStreetMapSearchResult.Lat, openStreetMapSearchResult.Lon);
                }
            }

            return offerCoordinate;
        }

        private async Task<IQueryable<Offer>> BuildOfferSearchQuery(SearchParamaters lastSearchParameters)
        {
            var offerQuery = _database.Offer
                .Where(o => o.Status == (int) GlobalConstants.OfferStatus.Active);

            if (!string.IsNullOrWhiteSpace(lastSearchParameters.OfferType))
            {
                offerQuery = offerQuery.Where(o => o.OfferType == lastSearchParameters.OfferType);
            }

            if (lastSearchParameters.Furnished)
            {
                offerQuery = offerQuery.Where(o => o.Furnished == true);
            }

            if (lastSearchParameters.Pets)
            {
                offerQuery = offerQuery.Where(o => o.Pets == true);
            }

            if (lastSearchParameters.Cellar)
            {
                offerQuery = offerQuery.Where(o => o.Cellar == true);
            }

            if (lastSearchParameters.Parking)
            {
                offerQuery = offerQuery.Where(o => o.Parking == true);
            }

            if (lastSearchParameters.Elevator)
            {
                offerQuery = offerQuery.Where(o => o.Elevator == true);
            }

            if (lastSearchParameters.Accessibility)
            {
                offerQuery = offerQuery.Where(o => o.Accessability == true);
            }

            if (lastSearchParameters.Dryer)
            {
                offerQuery = offerQuery.Where(o => o.Dryer == true);
            }

            if (lastSearchParameters.Washingmachine)
            {
                offerQuery = offerQuery.Where(o => o.WashingMachine == true);
            }

            if (lastSearchParameters.Television)
            {
                offerQuery =
                    offerQuery.Where(o => (o.Television != "No") && (o.Television != null) && (o.Television != ""));
            }

            if (lastSearchParameters.Wlan)
            {
                offerQuery = offerQuery.Where(o => o.Wlan == true);
            }

            if (lastSearchParameters.Lan)
            {
                offerQuery = offerQuery.Where(o => o.Lan == true);
            }

            if (lastSearchParameters.Telephone)
            {
                offerQuery = offerQuery.Where(o => o.Telephone == true);
            }

            if (lastSearchParameters.UniDistance != null)
            {
                if (lastSearchParameters.UniDistance.Gte != null)
                {
                    offerQuery = offerQuery.Where(o => o.UniDistance > lastSearchParameters.UniDistance.Gte);
                }

                if (lastSearchParameters.UniDistance.Lte != null)
                {
                    offerQuery = offerQuery.Where(o => o.UniDistance < lastSearchParameters.UniDistance.Lte);
                }
            }

            if (lastSearchParameters.Rent != null)
            {
                if (lastSearchParameters.Rent.Gte != null)
                {
                    offerQuery = offerQuery.Where(o => o.Rent > lastSearchParameters.Rent.Gte);
                }

                if (lastSearchParameters.Rent.Lte != null)
                {
                    offerQuery = offerQuery.Where(o => o.Rent < lastSearchParameters.Rent.Lte);
                }
            }

            if (lastSearchParameters.Size != null)
            {
                if (lastSearchParameters.Size.Gte != null)
                {
                    offerQuery = offerQuery.Where(o => o.Size > lastSearchParameters.Size.Gte);
                }

                if (lastSearchParameters.Size.Lte != null)
                {
                    offerQuery = offerQuery.Where(o => o.Size < lastSearchParameters.Size.Lte);
                }
            }

            if (lastSearchParameters.Rooms != null)
            {
                if (lastSearchParameters.Rooms.Gte != null)
                {
                    offerQuery = offerQuery.Where(o => o.Rooms > lastSearchParameters.Rooms.Gte);
                }

                if (lastSearchParameters.Rooms.Lte != null)
                {
                    offerQuery = offerQuery.Where(o => o.Rooms < lastSearchParameters.Rooms.Lte);
                }
            }

            if (lastSearchParameters.Internetspeed != null)
            {
                if (lastSearchParameters.Internetspeed.Gte != null)
                {
                    offerQuery = offerQuery.Where(o => o.InternetSpeed > lastSearchParameters.Internetspeed.Gte);
                }

                if (lastSearchParameters.Internetspeed.Lte != null)
                {
                    offerQuery = offerQuery.Where(o => o.InternetSpeed < lastSearchParameters.Internetspeed.Lte);
                }
            }

            if ((lastSearchParameters.Tags != null) && (lastSearchParameters.Tags.Count > 0))
            {
                //var tags =
                //    await
                //        _database.Tag.Where(t => lastSearchParameters.Tags.Contains(t.Title) && (t.OfferId != null))
                //            .ToListAsync();

                //var offerTagMapping = new Dictionary<int, List<string>>();
                //foreach (var tag in tags)
                //{
                //    if (tag.OfferId != null)
                //    {
                //        if (offerTagMapping.ContainsKey((int) tag.OfferId))
                //        {
                //            offerTagMapping[(int) tag.OfferId].Add(tag.Title);
                //        }
                //        else
                //        {
                //            offerTagMapping.Add((int) tag.OfferId, new List<string> {tag.Title});
                //        }
                //    }
                //}

                //IEnumerable<int> offerIds =
                //    offerTagMapping.Where(
                //            m =>
                //            {
                //                return (lastSearchParameters.Tags.Count() <= m.Value.Count()) &&
                //                       lastSearchParameters.Tags.All(t => m.Value.Contains(t));
                //            })
                //        .Select(m => m.Key)
                //        .ToList();

                //offerQuery = offerIds.Any() ? offerQuery.Where(o => offerIds.Contains(o.Id)) : null;

                var offerIds =
                    await
                        _database.Tag.Where(t => lastSearchParameters.Tags.Contains(t.Title) && (t.OfferId != null))
                            .Select(t => (int) t.OfferId)
                            .ToListAsync();
                offerQuery = offerIds.Any() ? offerQuery.Where(o => offerIds.Contains(o.Id)) : null;
            }

            return offerQuery;
        }

        #endregion
    }
}