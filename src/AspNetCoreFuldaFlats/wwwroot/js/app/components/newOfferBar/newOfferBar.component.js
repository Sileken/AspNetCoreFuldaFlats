/************************************************************
 * File:            newOfferBar.component.js
 * Author:          Patrick Hasenauer
 * LastMod:         13.12.2016
 * Description:     JS Component Handler for new offer bar.
 ************************************************************/
define(['text!./newOfferBar.component.html', 'css!./newOfferBar.component.css', 'app/components/fileUploaderModal/fileUploaderModal.component',
    'knockout', 'jquery', 'fuldaflatsApiClient', 'lightbox'],
    function (componentTemplate, componentCss, fileUploaderModalComponent, ko, $, api, lightbox) {
        function NewOfferModel(ko, $, api) {
            var self = this;
            // your model functions and variables
            self.tabsContainer = ko.observable();
            self.defaultSwitchOptions = { onText: 'Yes', onColor: 'primary', offColor: 'danger', offText: 'No', animate: true, size: 'small' };
            self.offerPriceTypes = ko.observableArray(["DAY", "MONTH", "QUARTER", "HALF YEAR", "SEMESTER", "YEAR"]);
            self.offerRentTypes = ko.observableArray(["COLD", "WARM"]);
            self.kitchenDescriptions = ko.observableArray(["Fridge & Oven", "Fridge & Stove", "Fridge & Stove & Oven"]);
            self.bathroomDescriptions = ko.observableArray(["Shower & WC", "Shower & Tub & WC", "Tub & WC"]);
            self.offerHeatingDescriptions = ko.observableArray(["Gas", "Oil", "Electricity"]);
            self.televisionDescriptions = ko.observableArray(["No", "Cable", "DSL", "SAT"]);
            self.offerTypes = ko.observable();
            self.offerTags = ko.observableArray()
            self.tempOfferStatus = ko.observable(false);
            self.offer = api.offers.getOfferModel();
            self.offerDetailsPageInfo = ko.observable();
            self.currentUser = ko.observable(
                {
                    isAuthenticated: false,
                    userData: undefined
                }
            );

            self.createdOffer = ko.observable(false);

            self.offerCreationError = ko.observable(false);
            self.offerLandlordIsNotCurrentUser = ko.observable(false);
            self.currentUserIsNotALandlord = ko.observable(false);
            self.offerLoadingError = ko.observable(false);
            self.offerTypesLoadingError = ko.observable(false);
            self.offerTagsLoadingError = ko.observable(false);
            self.offerUpdatingError = ko.observable(false);
            self.offerUpdatingErrorMessage = ko.observable("");

            self.finishedCreation = ko.observable(false);

            // Tab 1 possible invalid fields
            self.invalidOfferType = ko.observable(false);
            self.invalidTags = ko.observable(false);
            self.invalidTitle = ko.observable(false);
            self.invalidSize = ko.observable(false);
            self.invalidRooms = ko.observable(false);
            self.invalidRent = ko.observable(false);
            self.invalidRentType = ko.observable(false);
            self.invalidSideCosts = ko.observable(false);
            self.invalidPriceType = ko.observable(false);
            self.invalidDeposit = ko.observable(false);
            self.invalidCommission = ko.observable(false);

            // Tab 2 possible invalid fields
            self.invalidKitchenDescription = ko.observable(false);
            self.invalidBathroomNumbers = ko.observable(false);
            self.invalidBathroomDescription = ko.observable(false);
            self.invalidInternetSpeed = ko.observable(false);
            self.invalidHeatingDescription = ko.observable(false);
            self.invalidTelevision = ko.observable(false);

            // Tab 3 possible invalid fields
            self.invalidStreet = ko.observable(false);
            self.invalidHouseNumber = ko.observable(false);
            self.invalidFloor = ko.observable(false);
            self.invalidZipCode = ko.observable(false);
            self.invalidCity = ko.observable(false);

            // Tab 4 possible invalid fields
            self.invalidDescription = ko.observable();

            self.resetInvalidFields = function () {
                // Tab 1 possible invalid fields
                self.invalidOfferType(false);
                self.invalidTags(false);
                self.invalidTitle(false);
                self.invalidSize(false);
                self.invalidRooms(false);
                self.invalidRent(false);
                self.invalidRentType(false);
                self.invalidSideCosts(false);
                self.invalidPriceType(false);
                self.invalidDeposit(false);
                self.invalidCommission(false);

                // Tab 2 possible invalid fields
                self.invalidKitchenDescription(false);
                self.invalidBathroomNumbers(false);
                self.invalidBathroomDescription(false);
                self.invalidInternetSpeed(false);
                self.invalidHeatingDescription(false);
                self.invalidTelevision(false);

                // Tab 3 possible invalid fields
                self.invalidStreet(false);
                self.invalidHouseNumber(false);
                self.invalidFloor(false);
                self.invalidZipCode(false);
                self.invalidCity(false);

                // Tab 4 possible invalid fields
                self.invalidDescription(false);
            };

            self.validOfferType = ko.computed(function () {
                var isValid = false;
                if (self.offer().offerType() && self.offer().offerType().toString().trim().length > 0) {
                    isValid = true;
                }

                return isValid
            }).extend({ notify: 'always' });

            self.validTags = ko.computed(function () {
                return self.offer().tags() && self.offer().tags().length > 0;
            }).extend({ notify: 'always' });

            self.validTitle = ko.computed(function () {
                return self.offer().title() && self.offer().title().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validSize = ko.computed(function () {
                return self.offer().size() && self.offer().size().toString().trim().length > 0 && !isNaN(self.offer().size());
            }).extend({ notify: 'always' });

            self.validRooms = ko.computed(function () {
                return (self.offer().offerType() && (self.offer().offerType().toLowerCase() === 'COUCH'.toLowerCase() || self.offer().offerType().toLowerCase() === 'PARTY'.toLowerCase())) ||
                    (self.offer().rooms() && self.offer().rooms().toString().trim().length > 0 && !isNaN(self.offer().rooms()));
            }).extend({ notify: 'always' });

            self.validRent = ko.computed(function () {
                return self.offer().rent() && self.offer().rent().toString().trim().length > 0 && !isNaN(self.offer().rent());
            }).extend({ notify: 'always' });

            self.validRentType = ko.computed(function () {
                return (self.offer().offerType() && (self.offer().offerType().toLowerCase() === 'COUCH'.toLowerCase() || self.offer().offerType().toLowerCase() === 'PARTY'.toLowerCase())) ||
                    (self.offer().rentType() && self.offer().rentType().toString().trim().length > 0);
            }).extend({ notify: 'always' });

            self.validSideCosts = ko.computed(function () {
                return (self.offer().offerType() && (self.offer().offerType().toLowerCase() === 'COUCH'.toLowerCase() || self.offer().offerType().toLowerCase() === 'PARTY'.toLowerCase())) ||
                    (self.offer().sideCosts() && self.offer().sideCosts().toString().trim().length > 0 && !isNaN(self.offer().sideCosts()));
            }).extend({ notify: 'always' });

            self.validPriceType = ko.computed(function () {
                return (self.offer().offerType() && (self.offer().offerType().toLowerCase() === 'COUCH'.toLowerCase() || self.offer().offerType().toLowerCase() === 'PARTY'.toLowerCase())) ||
                    (self.offer().priceType() && self.offer().priceType().toString().trim().length > 0);
            }).extend({ notify: 'always' });

            self.validDeposit = ko.computed(function () {
                return self.offer().deposit() && self.offer().deposit().toString().trim().length > 0 && !isNaN(self.offer().deposit());
            }).extend({ notify: 'always' });

            self.validCommission = ko.computed(function () {
                return (self.offer().offerType() && (self.offer().offerType().toLowerCase() === 'COUCH'.toLowerCase() || self.offer().offerType().toLowerCase() === 'PARTY'.toLowerCase())) ||
                    (self.offer().commission() && self.offer().commission().toString().trim().length > 0 && !isNaN(self.offer().commission()));
            }).extend({ notify: 'always' });

            self.isTab1Invalid = ko.computed(function () {
                return self.invalidOfferType()
                    || self.invalidTags()
                    || self.invalidTitle()
                    || self.invalidSize()
                    || self.invalidRooms()
                    || self.invalidRent()
                    || self.invalidRentType()
                    || self.invalidSideCosts()
                    || self.invalidPriceType()
                    || self.invalidDeposit()
                    || self.invalidCommission();
            }).extend({ notify: 'always' });

            self.isTab1Valid = ko.computed(function () {
                var isValid = false

                if (self.validOfferType()
                    && self.validTags()
                    && self.validTitle()
                    && self.validSize()
                    && self.validRooms()
                    && self.validRent()
                    && self.validRentType()
                    && self.validSideCosts()
                    && self.validPriceType()
                    && self.validDeposit()
                    && self.validCommission()) {

                    isValid = true;
                    resetErrors();
                }
                return isValid;
            }).extend({ notify: 'always' });

            // Tab 2 valid functionn
            self.validKitchenDescription = ko.computed(function () {
                return self.offer().kitchenDescription() && self.offer().kitchenDescription().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validBathroomNumber = ko.computed(function () {
                return self.offer().bathroomNumber() && self.offer().bathroomNumber().toString().trim().length > 0 && !isNaN(self.offer().bathroomNumber());
            }).extend({ notify: 'always' });

            self.validBathroomDescription = ko.computed(function () {
                return self.offer().bathroomDescription() && self.offer().bathroomDescription().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validInternetSpeed = ko.computed(function () {
                return self.offer().internetSpeed() && self.offer().internetSpeed().toString().trim().length > 0 && !isNaN(self.offer().internetSpeed());
            }).extend({ notify: 'always' });

            self.validHeatingDescription = ko.computed(function () {
                return self.offer().heatingDescription() && self.offer().heatingDescription().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validTelevision = ko.computed(function () {
                return self.offer().television() && self.offer().television().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.isTab2Invalid = ko.computed(function () {
                return self.invalidKitchenDescription()
                    || self.invalidBathroomNumbers()
                    || self.invalidBathroomDescription()
                    || self.invalidInternetSpeed()
                    || self.invalidHeatingDescription()
                    || self.invalidTelevision();
            }).extend({ notify: 'always' });

            self.isTab2Valid = ko.computed(function () {
                var isValid = false

                if (self.validKitchenDescription()
                    && self.validBathroomNumber()
                    && self.validBathroomDescription()
                    && self.validInternetSpeed()
                    && self.validHeatingDescription()
                    && self.validTelevision()) {
                    isValid = true;
                    resetErrors();
                }
                return isValid;
            }).extend({ notify: 'always' });

            //Tab 3
            self.validStreet = ko.computed(function () {
                return self.offer().street() && self.offer().street().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validHouseNumber = ko.computed(function () {
                return self.offer().houseNumber() && self.offer().houseNumber().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validFloor = ko.computed(function () {
                return self.offer().floor() && self.offer().floor().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validZipCode = ko.computed(function () {
                return self.offer().zipCode() && self.offer().zipCode().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.validCity = ko.computed(function () {
                return self.offer().city() && self.offer().city().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.isTab3Invalid = ko.computed(function () {
                return self.invalidStreet()
                    || self.invalidHouseNumber()
                    || self.invalidFloor()
                    || self.invalidZipCode()
                    || self.invalidCity();
            }).extend({ notify: 'always' });

            self.isTab3Valid = ko.computed(function () {
                var isValid = false

                if (self.validStreet()
                    && self.validHouseNumber()
                    && self.validFloor()
                    && self.validZipCode()
                    && self.validCity()) {
                    isValid = true;
                    resetErrors();
                }
                return isValid;
            }).extend({ notify: 'always' });

            //Tab 4
            self.validDescription = ko.computed(function () {
                return self.offer().description() && self.offer().description().toString().trim().length > 0;
            }).extend({ notify: 'always' });

            self.isTab4Invalid = ko.computed(function () {
                return self.invalidDescription();
            }).extend({ notify: 'always' });

            self.isTab4Valid = ko.computed(function () {
                var isValid = false

                if (self.validDescription()) {
                    isValid = true;
                    resetErrors();
                }

                return isValid;
            }).extend({ notify: 'always' });

            function resetErrors() {
                self.offerCreationError(false);
                self.offerLandlordIsNotCurrentUser(false);
                self.currentUserIsNotALandlord(false);
                self.offerLoadingError(false);
                self.offerTypesLoadingError(false);
                self.offerTagsLoadingError(false);
                self.offerUpdatingError(false);
                self.offerUpdatingErrorMessage("");
                self.resetInvalidFields();
            };

            function getURLParameter(name) {
                return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
            };

            function clearNotAllowedFieldsForCouchOrParty() {
                if ((self.offer().offerType() && (self.offer().offerType().toLowerCase() === 'COUCH'.toLowerCase() || self.offer().offerType().toLowerCase() === 'PARTY'.toLowerCase()))) {
                    self.offer().rooms(undefined);
                    self.offer().rentType(undefined);
                    self.offer().priceType(undefined);
                    self.offer().sideCosts(undefined);
                    self.offer().commission(undefined);
                }
            };

            function activateTabNav(navElement) {
                if (navElement && typeof navElement.attr === "function") {
                    var parentListElement = navElement.parent("li");
                    if (parentListElement.length > 0) {
                        parentListElement.removeClass("disabled");
                        navElement.attr("data-toggle", "tab");
                    }
                }
            };

            function isCurrentUserEqualsLandlord(landlord) {
                var areEquals = false;

                var localLandlord = ko.unwrap(landlord);
                var localCurrentUser = ko.unwrap(self.currentUser);

                if (localLandlord && localCurrentUser &&
                    localCurrentUser.isAuthenticated && localCurrentUser.userData
                    && localLandlord.id === localCurrentUser.userData.id) {
                    areEquals = true;
                }

                return areEquals;
            };

            function isCurrentUserALandlord() {
                var isLandlord = false;

                var localCurrentUser = ko.unwrap(self.currentUser);

                if (localCurrentUser && localCurrentUser.isAuthenticated &&
                    localCurrentUser.userData && localCurrentUser.userData.type === 2) {
                    isLandlord = true;
                }

                return isLandlord;
            };

            function reloadOffer() {
                if (!isCurrentUserALandlord()) {
                    self.currentUserIsNotALandlord(true);
                } else if (self.offer() && !isNaN(self.offer().id())) {
                    api.offers.getOfferById(self.offer().id()).then(
                        function (requestedOffer) {
                            if (requestedOffer) {
                                if (!isCurrentUserALandlord()) {
                                    self.currentUserIsNotALandlord(true);
                                } else if (!isCurrentUserEqualsLandlord(requestedOffer.landlord)) {
                                    self.offerLandlordIsNotCurrentUser(true);
                                }
                                else {
                                    var mappedResult = api.offers.mapOfferResultToModel(requestedOffer);
                                    self.offer(mappedResult);
                                    resetErrors();
                                }
                            } else {
                                self.offerLoadingError(true);
                            }
                        },
                        function (xhr, statusText, error) {
                            self.offerLoadingError(true);
                        }
                    );
                }
            };

            function createOffer() {
                resetErrors();
                if (!isCurrentUserALandlord()) {
                    self.currentUserIsNotALandlord(true);
                } else {
                    api.offers.createOffer().then(
                        function (newOffer) {
                            if (!isCurrentUserEqualsLandlord(newOffer.landlord)) {
                                self.offerLandlordIsNotCurrentUser(true);
                            } else {
                                var mappedResult = api.offers.mapOfferResultToModel(newOffer);
                                self.offer(mappedResult);

                                self.offer().offerType.extend({ notify: 'always' });
                                self.offer().offerType.subscribe(function (newValue) {
                                    clearNotAllowedFieldsForCouchOrParty();
                                });

                                self.createdOffer(true);
                            }
                        },
                        function (xhr) {
                            self.offerCreationError(true);
                        }
                    );
                }
            };

            function loadOfferTypes() {
                api.offers.getOfferTypes().then(
                    function (offerTypes) {
                        self.offerTypes(offerTypes);
                    },
                    function (xhr) {
                        self.offerTypesLoadingError(true);
                    }
                );
            };

            function loadOfferTags() {
                api.offers.getTags().then(
                    function (offerTags) {
                        self.offerTags(offerTags)
                    },
                    function (xhr) {
                        self.offerTagsLoadingError(true);
                    }
                );
            };

            function onBeforeUnloadWindow() {
                if (!self.finishedCreation()) {
                    if (self.offer() && !isNaN(self.offer().id())) {
                        api.offers.deleteOffer(self.offer().id(), true);
                    }
                }
            };

            function processInvalidUpdateResponse(xhr) {
                if (xhr.responseJSON) {
                    var errorKey = Object.keys(xhr.responseJSON)[0];
                    var errorMessage = xhr.responseJSON[errorKey][0];
                    if (errorMessage) {
                        if (errorMessage.lastIndexOf(".") !== errorMessage.length - 1) {
                            errorMessage = errorMessage + ".";

                            switch (errorKey) {
                                case "offerType":
                                    self.invalidOfferType(true);
                                    break;
                                case "tags":
                                    self.invalidTags(true);
                                    break;
                                case "title":
                                    self.invalidTitle(true);
                                    break;
                                case "size":
                                    self.invalidSize(true);
                                    break;
                                case "rooms":
                                    self.invalidRooms(true);
                                    break;
                                case "rent":
                                    self.invalidRent(true);
                                    break;
                                case "rentType":
                                    self.invalidRentType(true);
                                    break;
                                case "sideCosts":
                                    self.invalidSideCosts(true);
                                    break;
                                case "priceType":
                                    self.invalidPriceType(true);
                                    break;
                                case "deposit":
                                    self.invalidDeposit(true);
                                    break;
                                case "commission":
                                    self.invalidCommission(true);
                                    break;
                                case "kitchenDescription":
                                    self.invalidKitchenDescription(true);
                                    break;
                                case "bathroomNumber":
                                    self.invalidBathroomNumbers(true);
                                    break;
                                case "bathroomDescription":
                                    self.invalidBathroomDescription(true);
                                    break;
                                case "internetSpeed":
                                    self.invalidInternetSpeed(true);
                                    break;
                                case "heatingDescription":
                                    self.invalidHeatingDescription(true);
                                    break;
                                case "television":
                                    self.invalidTelevision(true);
                                    break;
                                case "street":
                                    self.invalidStreet(true);
                                    break;
                                case "houseNumber":
                                    self.invalidHouseNumber(true);
                                    break;
                                case "floor":
                                    self.invalidFloor(true);
                                    break;
                                case "zipCode":
                                    self.invalidZipCode(true);
                                    break;
                                case "city":
                                    self.invalidCity(true);
                                    break;
                                case "description":
                                    self.invalidDescription(true);
                                    break;
                            }

                            self.offerUpdatingErrorMessage(errorMessage);
                            self.offerUpdatingError(true);
                        }
                    }

                    /*
                    var errors = Object.keys(xhr.responseJSON);
                    if (errors && errors.length > 0) {
                        $.each(errors, function(index, errorKey) {
                            var errorMessage = xhr.responseJSON[errorKey][0];
                            if (errorMessage) {
                                if (errorMessage.lastIndexOf(".") !== errorMessage.length - 1) {
                                    errorMessage = errorMessage + ".";

                                    switch (errorKey) {
                                        case "offerType":
                                            self.invalidOfferType(true);
                                            break;
                                        case "tags":
                                            self.invalidTags(true);
                                            break;
                                        case "title":
                                            self.invalidTitle(true);
                                            break;
                                        case "size":
                                            self.invalidSize(true);
                                            break;
                                        case "rooms":
                                            self.invalidRooms(true);
                                            break;
                                        case "rent":
                                            self.invalidRent(true);
                                            break;
                                        case "rentType":
                                            self.invalidRentType(true);
                                            break;
                                        case "sideCosts":
                                            self.invalidSideCosts(true);
                                            break;
                                        case "priceType":
                                            self.invalidPriceType(true);
                                            break;
                                        case "deposit":
                                            self.invalidDeposit(true);
                                            break;
                                        case "commission":
                                            self.invalidCommission(true);
                                            break;
                                        case "kitchenDescription":
                                            self.invalidKitchenDescription(true);
                                            break;
                                        case "bathroomNumber":
                                            self.invalidBathroomNumbers(true);
                                            break;
                                        case "bathroomDescription":
                                            self.invalidBathroomDescription(true);
                                            break;
                                        case "internetSpeed":
                                            self.invalidInternetSpeed(true);
                                            break;
                                        case "heatingDescription":
                                            self.invalidHeatingDescription(true);
                                            break;
                                        case "television":
                                            self.invalidTelevision(true);
                                            break;
                                        case "street":
                                            self.invalidStreet(true);
                                            break;
                                        case "houseNumber":
                                            self.invalidHouseNumber(true);
                                            break;
                                        case "floor":
                                            self.invalidFloor(true);
                                            break;
                                        case "zipCode":
                                            self.invalidZipCode(true);
                                            break;
                                        case "city":
                                            self.invalidCity(true);
                                            break;
                                        case "description":
                                            self.invalidDescription(true);
                                            break;
                                    }

                                    self.offerUpdatingErrorMessage(errorMessage); // todo: Array
                                }
                            }
                        });

                        if (self.offerUpdatingErrorMessage() && self.offerUpdatingErrorMessage().length > 0) {
                            self.offerUpdatingError(true);
                        }
                    } else {
                        self.offerUpdatingError(true);
                    }*/
                } else {
                    self.offerUpdatingError(true);
                }
            };

            self.offerFullPrice = ko.computed(function () {
                var fullPrice = 0;

                if (self.offer().rent() && !isNaN(self.offer().rent())) {
                    fullPrice += parseFloat(self.offer().rent());
                }
                if (self.offer().sideCosts() && !isNaN(self.offer().sideCosts())) {
                    fullPrice += parseFloat(self.offer().sideCosts());
                }

                self.offer().fullPrice(fullPrice);

                return fullPrice;
            });

            self.bindFileUploadModalEvents = function (model, event) {
                if (event && event.currentTarget) {
                    var dialogId = event.currentTarget.getAttribute("data-target");
                    var dialogContainer = $(dialogId);
                    if (dialogContainer.length > 0) {
                        dialogContainer.on('hidden.bs.modal', reloadOffer);
                    } else {
                        console.error("Failed to bind file upload dialog events.");
                    }
                }
            };

            self.goNextStep = function (model, event) {
                self.offer().status(0);

                api.offers.updatedOffer(self.offer).then(
                    function () {
                        if (self.tabsContainer() && event.currentTarget) {
                            var nextTabId = event.currentTarget.getAttribute("next-tab");
                            var nextTabNav = self.tabsContainer().find('.nav a[href="' + nextTabId + '"]');
                            if (nextTabNav.length > 0) {
                                nextTabNav.tab('show');
                                activateTabNav(nextTabNav);
                            }
                        }
                    },
                    function (xhr) {
                        processInvalidUpdateResponse(xhr);
                    }
                );
            };

            self.finishOfferCreation = function () {
                if (self.tempOfferStatus() === true) {
                    self.offer().status(1);
                } else {
                    self.offer().status(2);
                }

                api.offers.updatedOffer(self.offer).then(
                    function () {
                        if (self.offerDetailsPageInfo() && self.offerDetailsPageInfo().url) {
                            self.finishedCreation(true);
                            window.location.href = self.offerDetailsPageInfo().url + "?offerId=" + self.offer().id();
                        } else {
                            window.location.href = "/";
                        }
                    },
                    function (xhr) {
                        processInvalidUpdateResponse(xhr);
                    }
                );
            };

            self.cancelOfferCreation = function () {
                if (self.offer() && !isNaN(self.offer().id())) {
                    api.offers.deleteOffer(self.offer().id()).then(function () {
                        self.offer().id(undefined)
                        window.history.back();
                    }, function () {
                        window.history.back();
                    });;
                }
            };

            self.optionsAfterRender = function (option, item) {
                ko.applyBindingsToNode(option, {
                    disable: !item
                }, item);
            };

            self.initialize = function (params, tabsContainer) {
                self.tabsContainer(tabsContainer || "");

                if (params) {
                    self.offerDetailsPageInfo(ko.unwrap(params.offerDetailsPageInfo || ''));

                    if (params.currentUser && ko.isObservable(params.currentUser)) {
                        self.currentUser = params.currentUser;
                    }
                }

                self.currentUser.subscribe(function (currentUser) {
                    reloadOffer();

                    if (isCurrentUserALandlord() && isNaN(self.offer().id())) {
                        createOffer();
                    }
                });

                loadOfferTypes();
                loadOfferTags();
                createOffer();

                window.onbeforeunload = onBeforeUnloadWindow;
            };
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    ko.components.register("file-uploader", fileUploaderModalComponent);

                    var viewModel = null;
                    var templateRoot = $(componentInfo.element);
                    if (templateRoot.length > 0) {
                        var tabsContainer = templateRoot.find(".tabs");
                        if (tabsContainer.length > 0) {
                            var viewModel = new NewOfferModel(ko, $, api);
                            viewModel.initialize(params, tabsContainer);
                        }
                    }

                    return viewModel;
                }
            },
            template: componentTemplate
        };
    });