/************************************************************
 * File:            homeModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         09.12.2016
 * Description:     Home page module.
 ************************************************************/
define([
    'knockout', 'jquery',
    'app/components/searchBar/searchBar.component',
    'app/components/offerBarSlider/offerBarSlider.component',
    'app/components/tagCloudBar/tagCloudBar.component',
    'fuldaflatsApiClient'
], function (ko, $, searchBarComponent, offerBarSliderComponent, tagCloudBarComponent, api) {
    function HomePageModul(ko, $, searchBarComponent, offerBarSliderComponent, tagCloudBarComponent, api) {
        var self = this;

        var favoritesOffers = ko.observableArray();
        var recentOffers = ko.observableArray();
        var internationalOffers = ko.observableArray();

        ko.components.register("search", searchBarComponent);
        ko.components.register("offer-slider", offerBarSliderComponent);
        ko.components.register("tag-cloud", tagCloudBarComponent);

        function tryToSetFavoritesOffers(currentUser) {
            favoritesOffers.removeAll();
            var currentUserObject = ko.unwrap(currentUser);
            if (currentUserObject && currentUserObject.isAuthenticated && currentUserObject.userData
                && currentUserObject.userData.favorites && currentUserObject.userData.favorites.length > 0) {
                $.each(currentUserObject.userData.favorites, function (index, favorite) {
                    favoritesOffers.push(favorite);
                });
            }
        };

        self.searchByTags = function (tags) {
            var defer = $.Deferred();

            if (tags instanceof Array) {
                var queryParameter = api.offers.getSearchQueryParamters();
                if (queryParameter && queryParameter.tags && typeof queryParameter.tags === "function") {
                    queryParameter.tags(tags);
                    api.offers.searchOffer(queryParameter).then(function () {
                        api.offers.getOfferSearchResult().then(function (offerSearchResult) {
                            if (offerSearchResult) {
                                defer.resolve(offerSearchResult);
                            } else {
                                defer.reject("Invalid tags search result.");
                            }
                        });
                    });
                } else {
                    defer.reject("Invalid queryParameter from api client.");
                }
            } else {
                defer.reject("Invalid tags array.");
            }

            return defer.promise();
        };

        self.initialize = function (appModel) {
            if (appModel) {
                api.offers.getRecentOffers().then(function (recentOffersResult) {
                    recentOffers(recentOffersResult || [])
                });

                // Living International Offers 
                self.searchByTags(["english",
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
                    "chinese",]).then(function (offerSearchResult) {
                        internationalOffers(offerSearchResult || []);
                    });

                appModel.currentPage = appModel.pages.home;

                if (appModel.currentUser && ko.isObservable(appModel.currentUser)) {
                    appModel.currentUser.subscribe(function (currentUser) {
                        tryToSetFavoritesOffers(currentUser);
                    });
                    tryToSetFavoritesOffers(appModel.currentUser);
                }

                appModel.searchPanelBar = {
                    offerTypes: appModel.offerTypes,
                    searchPageInfo: appModel.pages.search
                };

                appModel.favoritesOfferBar = {
                    barTitle: "My Favorites",
                    offerDetailsPageInfo: appModel.pages.offerDetails,
                    offers: favoritesOffers,
                    owlCarouselOptions: {
                        items: 4,
                        itemsDesktopSmall: [990, 3],
                        itemsTablet: [768, 2],
                        itemsMobile: [480, 1]
                    }
                };

                appModel.recentBriefOfferBar = {
                    barTitle: "Recent Offers",
                    offerDetailsPageInfo: appModel.pages.offerDetails,
                    offers: recentOffers,
                    owlCarouselOptions: {
                        items: 4,
                        itemsDesktopSmall: [990, 3],
                        itemsTablet: [768, 2],
                        itemsMobile: [480, 1]
                    }
                };

                //todo: Query International offers
                appModel.livingInternationalBriefOfferBar = {
                    barTitle: "Living International",
                    offerDetailsPageInfo: appModel.pages.offerDetails,
                    offers: internationalOffers,
                    owlCarouselOptions: {
                        items: 4,
                        itemsDesktopSmall: [990, 3],
                        itemsTablet: [768, 2],
                        itemsMobile: [480, 1]
                    }
                };

                appModel.tagCloudBar = {
                    searchPageInfo: appModel.pages.search,
                    tagCloudOptions: {
                        height: 150
                    },
                }
            }
        }
    }

    return new HomePageModul(ko, $, searchBarComponent, offerBarSliderComponent, tagCloudBarComponent, api);
});


