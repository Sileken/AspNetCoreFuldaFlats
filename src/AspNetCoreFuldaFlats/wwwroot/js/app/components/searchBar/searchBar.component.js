/************************************************************
 * File:            searchBar.component.js
 * Author:          Jonas Kleinkauf, Patrick Hasenauer
 * LastMod:         09.12.2016
 * Description:     JS Component Handler for search bar.
 ************************************************************/
define(['text!./searchBar.component.html', 'css!./searchBar.component.css',
    'knockout', 'fuldaflatsApiClient', 'bootstrapMultiselect'],
    function (componentTemplate, componentCss, ko, api, multiselect) {

        var forceNullObservable = function (_val) {
            var obs = ko.observable(_val);

            return ko.computed({
                read: function () {
                    return obs();
                },
                write: function (val) {
                    if (val == '') {
                        val = null;
                    }
                    obs(val);
                }
            });
        };

        function SearchPanelModel(params) {
            var self = this;
            var searchCookieName = "lastSearchQuery";

            self.offerTypes = ko.observableArray();
            self.searchPageInfo = ko.observable();

            if (params) {
                self.searchPageInfo(ko.unwrap(params.searchPageInfo) || '');

                var paramsOfferTypes = ko.unwrap(params.offerTypes)
                if (paramsOfferTypes && paramsOfferTypes.length > 0) {
                    self.offerTypes(paramsOfferTypes);
                }
            }

            self.availableTags = ko.observableArray();
            api.offers.getTags().then(function (tags) {
                self.availableTags(tags);
                self.queryParameter(self.queryParameter())
            });

            self.offerTypes = ko.observableArray(['FLAT', 'SHARE', 'SUBLET', 'COUCH', 'PARTY']);

            //Recursively crawl the last search query and set knockout observables in leaf nodes
            function createRecursiveNotNullObservable(object) {
                for (var c1 in object) {
                    if (typeof object[c1] != "object") {
                        object[c1] = forceNullObservable(object[c1]);
                        continue;
                    } else if (Array.isArray(object[c1])) {
                        object[c1] = forceNullObservable(object[c1]);
                    }
                    object[c1] = createRecursiveNotNullObservable(object[c1]);
                }
                return object;
            }

            var queryParamaterEmpty = {
                offerType: forceNullObservable(),
                uniDistance: { gte: forceNullObservable(), lte: forceNullObservable() },
                rent: { gte: forceNullObservable(), lte: forceNullObservable() },
                size: { gte: forceNullObservable(), lte: forceNullObservable() },
                tags: forceNullObservable(),
                //Extended Search Parameters
                rooms: { gte: forceNullObservable(), lte: forceNullObservable() },
                furnished: forceNullObservable(),
                pets: forceNullObservable(),
                cellar: forceNullObservable(),
                parking: forceNullObservable(),
                elevator: forceNullObservable(),
                accessibility: forceNullObservable(),
                dryer: forceNullObservable(),
                washingmachine: forceNullObservable(),
                television: forceNullObservable(),
                wlan: forceNullObservable(),
                lan: forceNullObservable(),
                telephone: forceNullObservable(),
                internetspeed: { gte: forceNullObservable() }
            };

            self.queryParameter = ko.observable(queryParamaterEmpty);

            function getQueryParameter() {
                $.ajax({
                    url: "/api/offers/search/last",
                    dataType: "json",
                    type: "get",
                    contentType: "application/json",
                    success: function (data, status, req) {
                        var oldTagsObs = self.queryParameter().tags;
                        self.queryParameter(createRecursiveNotNullObservable(data));
                        oldTagsObs(self.queryParameter().tags());
                        self.queryParameter().tags = oldTagsObs;
                    },
                    error: function (req, status, err) {
                        self.queryParameter(queryParamaterEmpty);
                    }
                });
            };

            //only get query parameter if on search results (temporary bugfix)
            if (!isIndex) {
                getQueryParameter();
            }

            if (isIndex) {
                searchCallback = function () {
                    $.cookie(searchCookieName, ko.toJSON(self.queryParameter), { expires: 1, path: '/' });
                    if (self.searchPageInfo() && self.searchPageInfo().url) {
                        window.document.location.href = self.searchPageInfo().url
                    }
                }
            }

            // Function Bindings
            self.search = function () {
                var searchQuery = ko.toJSON(self.queryParameter);
                console.log(searchQuery);
                $.ajax({
                    url: "/api/offers/search",
                    type: "post",
                    dataType: "application/json",
                    contentType: "application/json",
                    data: searchQuery,
                    success: function (data, status, req) {
                        searchCallback();
                    },
                    error: function (req, status, err) {
                        console.log("Error!");
                        console.log(err);
                    }
                });
            };

            //Extended Search Bar
            self.showExtendedSearchButton = ko.observable();
            self.showExtendedSearch = ko.observable();
            self.showExtendedSearch(false);
            self.showSimpleSearch = ko.observable();
            self.showSimpleSearch(true);

            //Check if I may show simple search
            self.checkLogin = function () {
                $.ajax({
                    url: "/api/users/me",
                    type: "get",
                    contentType: "application/json",
                    success: function (data, status, req) {
                        self.showExtendedSearchButton(true);
                    },
                    error: function (req, status, err) {
                        self.showExtendedSearchButton(false);
                    }
                });
            }

            self.checkLogin();
            loginCallbacks.push(self.checkLogin);

            self.openExtendedSearch = function () {
                self.showExtendedSearch(true);
                self.showSimpleSearch(false);
            }
            self.hideExtendedSearch = function () {
                self.showExtendedSearch(false);
                self.showSimpleSearch(true);
            }

            self.optionsAfterRender = function (option, item) {
                ko.applyBindingsToNode(option, {
                    disable: !item
                }, item);
            };
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    var viewModel = new SearchPanelModel(params);
                    window.m = viewModel;
                    return viewModel;
                }
            },
            template: componentTemplate
        };
    });