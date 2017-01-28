/************************************************************
 * File:            fuldaflatsApiClient.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Javascript client for fuldaflats api.
 ************************************************************/
define(["jquery", 'knockout',
    "/js/fuldaflatsApiClient/endpoints/offers.js",
    "/js/fuldaflatsApiClient/endpoints/users.js",
    "/js/fuldaflatsApiClient/endpoints/mediaObjects.js"],
    function ($, ko, offersEndPoint, usersEndPoint, mediaObjectsEndpoint) {
        function FuldaFlatsApiClient() {
            var self = this;

            var relativeApiUrl = "/api/"
            var endpointUrls = {
                offers: {
                    offers: relativeApiUrl + "offers",
                    tags: relativeApiUrl + "tags",
                    search: relativeApiUrl + "offers/search",
                    recent: relativeApiUrl + "offers/recent",
                },
                users: {
                    auth: relativeApiUrl + "users/auth",
                    users: relativeApiUrl + "users",
                    me: relativeApiUrl + "users/me",
                },
                mediaObjects: {
                    getMediaObjectsByUserID: relativeApiUrl + "mediaObjects",
                    deleteMediaObjectById : relativeApiUrl + "mediaObjects"
                }

            }

            function setRelativeApiUrl(relativeApiUrl) {
                var unwrapRelativeApiUrl = ko.unwrap(relativeApiUrl);
                if (unwrapRelativeApiUrl) {
                    if (unwrapRelativeApiUrl.indexOf("/") !== 0) {
                        unwrapRelativeApiUrl = "/" + unwrapRelativeApiUrl;
                    }

                    if (unwrapRelativeApiUrl.lastIndexOf("/") !== unwrapRelativeApiUrl.lenght - 1) {
                        unwrapRelativeApiUrl = unwrapRelativeApiUrl + "/";
                    }

                    relativeApiUrl = unwrapRelativeApiUrl;
                }
            }

            self.initialize = function (relativeApiUrl, offerTypes) {
                setRelativeApiUrl(relativeApiUrl);

                self.offers = new offersEndPoint(endpointUrls.offers, offerTypes);
                self.users = new usersEndPoint(endpointUrls.users);

                self.mediaObjects = new mediaObjectsEndpoint(endpointUrls.mediaObjects);

            }
        }

        return new FuldaFlatsApiClient;
    });