/************************************************************
 * File:            editOfferDetailsBar.component.js
 * Author:          Martin Herbener, Patrick Hasenauer
 * LastMod:         11.12.2016
 * Description:     JS Component Handler for edit offer details bar.
 ************************************************************/
define(['text!./editOfferDetailsBar.component.html',
    'css!./editOfferDetailsBar.component.css', 'css!../offerDetailsBar/offerDetailsBar.component.css',
    'app/components/fileUploaderModal/fileUploaderModal.component',
    'knockout', 'jquery', 'fuldaflatsApiClient'],
    function (componentTemplate, componentCss, detailsCss, fileUploaderModalComponent, ko, $, api) {
        function EditOfferDetailsModel(params) {

            // your model functions and variables
            var self = this;

            //  Main Observables
            self.offer = ko.observable({});
            self.offerId = ko.observable();
            self.offerChanges = ko.observable({});

            // User Observables
            self.isAuthenticated = ko.observable(false);
            self.landlord = ko.observable({});
            self.currentUser = ko.observable(
                {
                    isAuthenticated: false,
                    userData: undefined
                }
            );

            // Checkbox Observables
            self.status = ko.observable(0);
            self.cellar = ko.observable(0);
            self.parking = ko.observable(0);
            self.elevator = ko.observable(0);
            self.dryer = ko.observable(0);
            self.washingMachine = ko.observable(0);
            self.telephone = ko.observable(0);
            self.furnished = ko.observable(0);
            self.pets = ko.observable(0);
            self.wlan = ko.observable(0);
            self.lan = ko.observable(0);
            self.accessability = ko.observable(0);

            // Select Observables
            self.television = ko.observable();
            self.heatingDescription = ko.observable();
            self.bathroomDescription = ko.observable();
            self.kitchenDescription = ko.observable();
            self.priceType = ko.observable();
            self.rentType = ko.observable();
            self.offerPriceTypes = ko.observableArray(["DAY", "MONTH", "QUARTER", "HALF YEAR", "SEMESTER", "YEAR"]);
            self.offerRentTypes = ko.observableArray(["COLD", "WARM"]);
            self.kitchenDescriptions = ko.observableArray(["Fridge & Oven", "Fridge & Stove", "Fridge & Stove & Oven"]);
            self.bathroomDescriptions = ko.observableArray(["Shower & WC", "Shower & Tub & WC", "Tub & WC"]);
            self.offerHeatingDescriptions = ko.observableArray(["Gas", "Oil", "Electricity"]);
            self.televisionDescriptions = ko.observableArray(["No", "Cable", "DSL", "SAT"]);

            // Tags Observables
            self.allTags = ko.observable({});
            self.selectedTags = ko.observableArray();

            self.offerType = ko.observable();
            self.showField = ko.observable(true);

            // Get URL Data
            function getURLParameter(name) {
                return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
            };

            //Check Login
            self.checkLogin = function () {
                $.ajax({
                    method: "GET",
                    url: "/api/users/me",
                    contentType: "application/json",
                    success: function (data, status, req) {
                        console.log("User-Data:");
                        console.log(data);
                        self.currentUser(data);
                        self.isAuthenticated(true);
                    },
                    error: function (req, status, error) {
                        self.currentUser({});
                        self.isAuthenticated(false);
                    }
                });
            }
            loginCallbacks.push(self.checkLogin);
            self.checkLogin();

            //Get Offer Data
            self.offerId(getURLParameter("offerId") || "");
            if (self.offerId()) {
                loadPage();
            }

            // Reload Data after Media upload
            function loadPage() {
                $.getJSON({
                    url: '/api/offers/' + self.offerId(),
                    success: function (offerData, status, req) {
                        if (offerData) {
                            for (var i in offerData.mediaObjects) {
                                offerData.mediaObjects[i].carouselIndex = i;
                                offerData.mediaObjects[i].carouselActive = false;
                            }
                            offerData.mediaObjects[0].carouselActive = true;
                            console.log("Offer-Data:");
                            console.log(offerData);
                            self.offer(offerData);
                            if (offerData.status == 1) {
                                self.status(true);
                            } else {
                                self.status(false);
                            }
                            self.cellar(offerData.cellar);
                            self.parking(offerData.parking);
                            self.elevator(offerData.elevator);
                            self.dryer(offerData.dryer);
                            self.washingMachine(offerData.washingMachine);
                            self.telephone(offerData.telephone);
                            self.furnished(offerData.furnished);
                            self.pets(offerData.pets);
                            self.wlan(offerData.wlan);
                            self.lan(offerData.lan);
                            self.accessability(offerData.accessability);
                            self.television(offerData.television);
                            self.heatingDescription(offerData.heatingDescription);
                            self.bathroomDescription(offerData.bathroomDescription);
                            self.kitchenDescription(offerData.kitchenDescription);
                            self.priceType(offerData.priceType);
                            self.rentType(offerData.rentType);
                            self.offerType(offerData.offerType);
                            if (offerData.offerType) {
                                if (offerData.offerType == "COUCH" || offerData.offerType == "PARTY") {
                                    self.showField(false);
                                }
                                else {
                                    self.showField(true);
                                }
                            }
                            if (offerData.landlord) {
                                self.landlord(offerData.landlord);
                            }
                        }
                        if (offerData.tags) {
                            if (offerData.tags.length > 0) {
                                for (var j in offerData.tags) {
                                    self.selectedTags.push(offerData.tags[j].title);
                                }
                            }
                        }
                    }
                })
            };

            // Get Tags
            self.loadTags = function () {
                $.ajax({
                    method: "GET",
                    url: "/api/tags",
                    contentType: "application/json",
                    success: function (tagsData, status, req) {
                        self.allTags(tagsData);
                    },
                    error: function (req, status, error) {

                    }
                });
            }
            self.loadTags();

            // File Upload Modal
            self.bindFileUploadModalEvents = function (model, event) {
                if (event && event.currentTarget) {
                    var dialogId = event.currentTarget.getAttribute("data-target");
                    var dialogContainer = $(dialogId);
                    if (dialogContainer.length > 0) {
                        dialogContainer.on('hide.bs.modal', loadPage);
                    } else {
                        console.error("Failed to bind file upload dialog events.");
                    }
                }
            };

            // Cancel Button
            self.cancelEditOffer = function () {
                console.log("Cancel gedrückt");
                window.history.back();
            };

             // Delete Offer

             self.openDeleteOfferModal = function(){
                 $('#confirmDeleteModal').modal(); 
             }

            self.deleteOffer = function () {
                $.ajax({
                    method: "DELETE",
                    url: '/api/offers/' + self.offerId(),
                    success: function (data, status, req) {
                        window.location = "/pages/myProfile";
                    },
                    error: function (req, status, error) {
                        if (req.status == 200) {
                            window.location = "/pages/myProfile";
                            return;
                        }
                        errorCallback(error);
                    }
                });
            };

            // Accept Button
            self.updateOffer = function () {
                if (self.status() === true) {
                    self.offerChanges().status = 1
                } else {
                    self.offerChanges().status = 2
                }
                self.offerChanges().cellar = self.cellar();
                self.offerChanges().parking = self.parking();
                self.offerChanges().elevator = self.elevator();
                self.offerChanges().dryer = self.dryer();
                self.offerChanges().washingMachine = self.washingMachine();
                self.offerChanges().telephone = self.telephone();
                self.offerChanges().furnished = self.furnished();
                self.offerChanges().pets = self.pets();
                self.offerChanges().wlan = self.wlan();
                self.offerChanges().lan = self.lan();
                self.offerChanges().accessability = self.accessability();
                self.offerChanges().television = self.television();
                self.offerChanges().heatingDescription = self.heatingDescription();
                self.offerChanges().bathroomDescription = self.bathroomDescription();
                self.offerChanges().kitchenDescription = self.kitchenDescription();
                self.offerChanges().priceType = self.priceType();
                self.offerChanges().rentType = self.rentType();
                self.offerChanges().tags = self.selectedTags();
                var _offerChanges = ko.toJSON(self.offerChanges);
                $.ajax({
                    method: "PUT",
                    url: '/api/offers/' + self.offerId(),
                    dataType: "text",
                    contentType: "application/json",
                    data: _offerChanges,
                    success: function (data, status, req) {
                        window.location = "/pages/offerDetails?offerId=" + self.offerId();
                    },
                    error: function (req, status, error) {
                        if (req.status == 200) {
                            window.location = "/pages/offerDetails?offerId=" + self.offerId();
                            return;
                        }
                        errorCallback(error);
                    }
                });
                console.log("Update gedrückt");
            }
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    ko.components.register("file-uploader", fileUploaderModalComponent);
                    var viewModel = new EditOfferDetailsModel(params);
                    window.model = viewModel;
                    return viewModel;
                }
            },
            template: componentTemplate
        };
    });
