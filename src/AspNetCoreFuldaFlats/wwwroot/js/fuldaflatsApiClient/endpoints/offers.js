/************************************************************
 * File:            offer.js
 * Author:          Patrick Hasenauer, Jonas Kleinkauf
 * LastMod:         09.12.2016
 * Description:     Javascript api client endpoints for offers.
 ************************************************************/
define(["jquery", 'knockout'], function($, ko) {

    function OffersEndpoint(offersEndpointUrls, availableOfferTypes) {
        var self = this;
        var endpointUrls = offersEndpointUrls;

        self.mapOfferResultToModel = function(offerResult) {
            var offer = ko.unwrap(self.getOfferModel());
            var offerResultValue = ko.unwrap(offerResult);
            $.each(Object.keys(offerResultValue), function(index, propertyName) {
                if (offer.hasOwnProperty(propertyName)) {
                    var property = offer[propertyName];
                    var propertyValue = ko.unwrap(offerResultValue[propertyName]);
                    if (property && propertyValue) {
                        if (ko.isObservable(property) && typeof property.remove === "function" && Array.isArray(propertyValue)) {
                            property(propertyValue)
                        } else if (ko.isObservable(property)) {
                            property(propertyValue);
                        } else {
                            property = propertyValue;
                        }
                    }
                }
            });

            return offer;
        }

        self.getOfferModel = function() {
            return ko.observable({
                id: ko.observable(),
                title: ko.observable(),
                offerType: ko.observable(),
                description: ko.observable(),
                rent: ko.observable(),
                rentType: ko.observable(),
                rooms: ko.observable(),
                sideCosts: ko.observable(),
                fullPrice: ko.observable(),
                deposit: ko.observable(),
                commission: ko.observable(),
                priceType: ko.observable(),
                street: ko.observable(),
                zipCode: ko.observable(),
                houseNumber: ko.observable(),
                city: ko.observable(),
                floor: ko.observable(),
                size: ko.observable(),
                furnished: ko.observable(),
                pets: ko.observable(),
                bathroomNumber: ko.observable(),
                bathroomDescription: ko.observable(),
                kitchenDescription: ko.observable(),
                cellar: ko.observable(),
                parking: ko.observable(),
                elevator: ko.observable(),
                accessability: ko.observable(),
                wlan: ko.observable(),
                lan: ko.observable(),
                internetSpeed: ko.observable(),
                heatingDescription: ko.observable(),
                television: ko.observable(),
                dryer: ko.observable(),
                washingMachine: ko.observable(),
                telephone: ko.observable(),
                status: ko.observable(0),
                creationDate: ko.observable(),
                lastModified: ko.observable(),
                longitude: ko.observable(),
                latitude: ko.observable(),
                uniDistance: ko.observable(),
                landlord: ko.observable(),
                thumbnailUrl: ko.observable(),
                mediaObjects: ko.observableArray(),
                reviews: ko.observableArray(),
                tags: ko.observableArray(),
                favorite: ko.observableArray()
            });
        };

        // Offer Serach
        var forceNullObservable = function() {
            var obs = ko.observable(null);
            return ko.pureComputed({
                read: function() {
                    return obs();
                },
                write: function(value) {
                    if (value === '') {
                        value = null;
                    }
                    obs(value);
                },
                owner: this
            });
        };

        self.getSearchQueryParamters = function() {
            return {
                offerType: forceNullObservable(),
                uniDistance: { lte: forceNullObservable() },
                rent: { lte: forceNullObservable() },
                size: { gte: forceNullObservable() },
                tags: forceNullObservable()
            }
        }

        self.searchOffer = function(queryParameters, redirectSearchPageUrl) {
            var defer = $.Deferred();

            var searchQuery = ko.toJSON(queryParameters, function(key, value) {
                if (key !== "offerType" && key !== "tag" && value == null) {
                    return;
                }
                else {
                    return value;
                }
            });

            $.ajax({
                url: endpointUrls.search,
                method: "POST",
                contentType: "application/json",
                data: searchQuery,
            }).done(function() {
                if (redirectSearchPageUrl) {
                    document.location.href = redirectSearchPageUrl;
                } else {
                    defer.resolve();
                }
            }).fail(function(jqXHR, textStatus) {
                console.error("Failed to post search query to search api.");
                defer.reject(jqXHR);
            });

            return defer.promise();
        };

        self.getOfferSearchResult = function() {
            var defer = $.Deferred();

            $.ajax({
                url: endpointUrls.search,
                method: "GET",
                dataType: "json"
            }).done(function(requestSearchResults) {
                if (Array.isArray(requestSearchResults)) {
                    defer.resolve(requestSearchResults);
                } else {
                    defer.resolve([]);
                }
            }).fail(function(jqXHR, textStatus) {
                console.error("Failed to get offer search result.");
                defer.reject(jqXHR);
            });

            return defer.promise();
        }

        // Get Offer Types
        var offerTypes = [];
        var unwrapOfferTypes = ko.unwrap(availableOfferTypes);
        if (Array.isArray(unwrapOfferTypes)) {
            $.each(unwrapOfferTypes, function(index, offerType) {
                offerTypes.push(offerType)
            });
        }

        self.getOfferTypes = function() {
            var defer = $.Deferred();
            defer.resolve(offerTypes);
            return defer.promise();
        }

        // Get Tags
        var getTagsDefer = undefined;
        var cachedTags = [];

        self.getTags = function(force) {
            if (getTagsDefer == undefined) {
                getTagsDefer = $.Deferred();

                if (force || cachedTags.length == 0) {
                    $.ajax({
                        url: endpointUrls.tags,
                        method: "GET",
                        dataType: "json"
                    }).done(function(tags) {
                        if (Array.isArray(tags) && tags.length > 0) {
                            cachedTags = tags;
                        }

                        getTagsDefer.resolve(cachedTags);
                        getTagsDefer = undefined;
                    }).fail(function(jqXHR, textStatus) {
                        var errorMsg = "Failed to load tags \n" + jqXHR.statusCode().status + ": " + jqXHR.statusCode().statusText;
                        console.error(errorMsg);
                        defer.reject(jqXHR)
                        getTagsDefer = undefined;
                    });
                } else {
                    getTagsDefer.resolve(cachedTags);
                }
            }

            return getTagsDefer.promise();
        }

        // Recent offers
        self.getRecentOffers = function() {
            var defer = $.Deferred();

            $.ajax({
                url: endpointUrls.recent,
                method: "GET",
                dataType: "json"
            }).done(function(requestedOfferResults) {
                if (Array.isArray(requestedOfferResults) && requestedOfferResults.length > 0) {
                    defer.resolve(requestedOfferResults);
                } else {
                    defer.resolve([]);
                }
            }).fail(function(jqXHR, textStatus) {
                console.error("Failed to get recent offers.");
                defer.reject(jqXHR);
            });

            return defer.promise();
        }

        // Get Offer By Id
        self.getOfferById = function(offerId) {
            var defer = $.Deferred();

            $.ajax({
                url: endpointUrls.offers + "/" + offerId,
                method: "GET",
                dataType: "json"
            }).done(function(requestedOffer) {
                if (requestedOffer) {
                    defer.resolve(requestedOffer);
                } else {
                    console.error("Failed to get offer by id " + offerId + ". Unexpected  server response.");
                    defer.reject(jqXHR);
                }
            }).fail(function(jqXHR, textStatus) {
                console.error("Failed to get offer by id " + offerId);
                defer.reject(jqXHR);
            });

            return defer.promise();
        };

        // Update Offer
        self.updatedOffer = function(offer) {
            var defer = $.Deferred();

            var localOffer = ko.unwrap(offer);

            $.ajax({
                url: endpointUrls.offers + "/" + ko.unwrap(localOffer.id),
                method: "PUT",
                contentType: "application/json",
                dataType: "text",
                data: ko.toJSON(localOffer)
            }).done(function(updatedOffer) {
                defer.resolve();
            }).fail(function(jqXHR, textStatus) {
                console.error("Failed to update offer:");
                console.error(localOffer);
                defer.reject(jqXHR);
            });

            return defer.promise();
        };

        // Create Offer
        self.createOffer = function() {
            var defer = $.Deferred();

            $.ajax({
                url: endpointUrls.offers,
                method: "POST",
                dataType: "json",
            }).done(function(newOffer) {
                if (newOffer) {
                    defer.resolve(newOffer);
                } else {
                    console.error("Failed to create offer. Unexpected  server response.");
                    defer.reject(jqXHR);
                }
            }).fail(function(jqXHR, textStatus) {
                console.error("Failed to create offer");
                defer.reject(jqXHR);
            });

            return defer.promise();
        };

        // Delete offer
        self.deleteOffer = function(offerId, synchron) {
            var defer = $.Deferred();

            var localOfferId = ko.unwrap(offerId)

            if (!isNaN(localOfferId)) {
                $.ajax({
                    url: endpointUrls.offers + "/" + localOfferId,
                    method: "DELETE",
                    async: synchron !== true
                }).done(function(deletedOffer) {
                    console.log("Delete Offer with id: " + localOfferId)
                    defer.resolve();
                }).fail(function(jqXHR, textStatus) {
                    console.error("Failed to delete offer with id:" + localOfferId);
                    defer.reject(jqXHR);
                });
            } else {
                defer.resolve();
            }

            return defer.promise();
        };
    }

    return OffersEndpoint;
});