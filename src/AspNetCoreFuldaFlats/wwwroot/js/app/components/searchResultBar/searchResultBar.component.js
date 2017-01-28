/************************************************************
 * File:            searchResultBar.component.js
 * Author:          Jonas Kleinkauf, Patrick Hasenauer
 * LastMod:         13.12.2016
 * Description:     JS Component Handler for seach results
 ************************************************************/

define(['text!./searchResultBar.component.html', 'css!./searchResultBar.component.css', 'knockout',
        'jquery', 'fuldaflatsApiClient', 'leaflet',
        'css!../offerBarSlider/offerBarSlider.component.css', 'moment'
    ],
    function (componentTemplate, componentCss, ko, $, api, L, offerBarCss, moment) {
        
        moment.locale('de');

        var resultMap;
        var locationMarkers = [];
        var iconBlue = L.icon({
            iconUrl: '/img/marker-icon-purple.png',
            iconSize: [25, 41],
            iconAnchor: [12.5, 41],
            popupAnchor: [0, -43],
        });

        var iconHS = L.icon({
            iconUrl: '/img/hs_marker.png',
            iconSize: [40, 40],
            iconAnchor: [20, 40],
            popupAnchor: [0, -43],
        });

        function SearchResultModel(componentTemplate, componentCss, ko, $, api, L, offerBarCss) {
            var self = this;

            self.offers = ko.observableArray([]);
            self.loadedResults = ko.observable()

            var forceNullObservable = function () {
                var obs = ko.observable();

                return ko.computed({
                    read: function () {
                        return obs();
                    },
                    write: function (val) {
                        if (val === '') {
                            val = null;
                        }
                        obs(val);
                    }
                });
            };

            self.sortCriteria = ko.observableArray([
                {
                    name: "Sort by creation date",
                    fn: function(left, right){
                        return left.creationDate == right.creationDate ? 0 : (left.creationDate > right.creationDate ? -1 : 1)
                    }
                },
                {
                    name: "Sort by price",
                    fn: function(left, right){
                        return left.rent == right.rent ? 0 : (left.rent < right.rent ? -1 : 1)
                    }
                },
                {
                    name: "Sort by distance",
                    fn: function(left, right){
                        return left.uniDistance == right.uniDistance ? 0 : (left.uniDistance < right.uniDistance ? -1 : 1)
                    }
                }
            ]);

            self.selectedSortCriteria = ko.observable();
            self.selectedSortCriteria(self.sortCriteria()[0].fn);
            self.sortOffers = function(){
                console.log("Sort Offers Called!");
                console.log(self.selectedSortCriteria());
                self.offers.sort(self.selectedSortCriteria().fn);
            }

            self.getSearchResults = function () {
                self.offers.removeAll();
                //Remove all Markers from the map
                for (var i in locationMarkers) {
                    resultMap.removeLayer(locationMarkers[i])
                }

                $.ajax({
                    url: "/api/offers/search",
                    success: function (data, status, req) {
                        for (var i in data) {
                            data[i].detailsUrl = '/pages/offerDetails?offerId=' + data[i].id;
                            data[i].isFavorite = ko.observable(data[i].isFavorite);
                            data[i].creationDateFormat = moment(data[i].creationDate).format('L');
                            self.offers.push(data[i]);
                        }
                        self.sortOffers();
                        placeMarkers(data);
                        self.loadedResults(true);
                    },
                    error: function (req, status, err) {
                        console.log(status);
                        console.log(err);
                        console.log(req);
                        self.loadedResults(true);
                    }
                });
            }
            searchCallback = self.getSearchResults;

            //Favorite Functions
            self.setFavorite = function(offer){
                offer.isFavorite(true);
                $.ajax({
                    url: "/api/offers/" + offer.id + "/favorite",
                    method: "PUT",
                    success: function(data, status, req){
                        console.log("Favorite Added!");
                    },
                    error: function(req, status, err){
                        errorCallback(JSON.parse(req.responseText));
                        offer.isFavorite(false);
                    } 
                });
            }

            self.unsetFavorite = function(offer){
                offer.isFavorite(false);
                $.ajax({
                    url: "/api/offers/" + offer.id + "/favorite",
                    method: "DELETE",
                    success: function(data, status, req){
                        console.log("Favorite Removed!");
                    },
                    error: function(req, status, err){
                        console.error(req);
                        errorCallback(err);
                        offer.isFavorite(true);
                    } 
                });
            }

            //Map Functions
            function initMap() {
                resultMap = L.map('resultMap').setView([50.5647986, 9.6828528], 14);

                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    maxZoom: 16,
                    minZoom: 5,
                    attribution: 'Map data © <a href="http://openstreetmap.org">OpenStreetMap</a> contributors',
                }).addTo(resultMap);

                var hs_latlng = ['50.5648258', '9.6842798'];
                var popup = '<div class="marker-popup"><img src="/img/logo_hs.png" alt="" class="img-responsive"></div>'
                L.marker(hs_latlng, {icon: iconHS}).addTo(resultMap).bindPopup(popup);
            }

            function placeMarkers(offers) {
                for (var i in offers) {
                    var latlng = [offers[i].latitude, offers[i].longitude];
                    var iconPopup = iconBlue;
                    var picture = '<div class="box-image-text">' +
                        '<div class="top">' +
                        '<div class="image thumbnail-container">' +
                        '<img src="' + offers[i].thumbnailUrl + '" alt="" class="img-responsive thumbnail-img">' +
                        '</div>' +
                        '<div class="bg"></div>' +
                        '<div class="text map-popup-viewtext">' +
                        '<p class="buttons">' +
                        '<a href="' + offers[i].detailsUrl + 
                        '" class="btn btn-template-transparent-primary map-popup-img"><i class="fa fa-link"></i>View Details</a>' +
                        '</p>' +
                        '</div>' +
                        '</div>' +
                        '</div>';

                    var caption = '<a href="' + offers[i].detailsUrl + '"><h4>' + offers[i].title + '</h4></a>';
                    var size = '<div class="map-popup-details col-md-6">' +
                        '<div class="offer-brief-detail-1-details-value"><strong>' + offers[i].size + ' m²</strong></div>' +
                        '<div class="offer-brief-detail-1-details-key"><small>Size</small></div></div>';
                    var price = '<div class="map-popup-details col-md-6">' +
                        '<div class="offer-brief-detail-1-details-value price-wrapper"><strong>' + offers[i].rent + ' €</strong>' +
                        '<div class="rate"><strong>/ ' + offers[i].rentType + '</strong></div></div>' +
                        '<div class="offer-brief-detail-1-details-key"><small>' + offers[i].offerType + '</small></div>';
                    var popup = '<div class="marker-popup row">' +
                        caption +
                        picture +
                        size +
                        price +
                        '</div>';
                    locationMarkers.push(L.marker(latlng, {
                        icon: iconPopup
                    }).addTo(resultMap).bindPopup(popup));
                }
            }

            //Initialization Function
            $(function () {
                self.getSearchResults();
                initMap();
            });

        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    var viewModel = new SearchResultModel(componentTemplate, componentCss, ko, $, api, L, offerBarCss);
                    return viewModel;
                }
            },
            template: componentTemplate
        };
    });