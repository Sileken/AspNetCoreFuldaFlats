/************************************************************
 * File:            navigationBar.component.js
 * Author:          Patrick Hasenauer
 * LastMod:         09.12.2016
 * Description:     JS Component Handler for navigation bar.
 ************************************************************/
define(['text!./navigationBar.component.html', 'css!./navigationBar.component.css',
    'knockout', 'jquery', 'fuldaflatsApiClient',
    'app/components/signInModalDialog/signInModalDialog.component',
    'app/components/signUpModalDialog/signUpModalDialog.component',
    'app/components/errorModalDialog/errorModalDialog.component',
    'app/components/contactModalDialog/contactModalDialog.component'],
    function (componentTemplate, componentCss, ko, $, api, signInModalDialogComponent, signUpModalDialogComponent, errorModalDialogComponent, contactModalDialogComponent) {
        function NavigationModel($, ko, api) {
            var self = this;

            self.userTypes = {
                normal: 1,
                landlord: 2,
            }

            self.domain = ko.observable();
            self.logoUrl = ko.observable();
            self.smallLogoUrl = ko.observable();

            self.homePageInfo = ko.observable();
            self.myProfilePageInfo = ko.observable();
            self.becomeLandlordPageInfo = ko.observable();
            self.newOfferPageInfo = ko.observable();
            self.legalDisclosurePageInfo = ko.observable();
            self.termsOfUsePageInfo = ko.observable();

            self.currentUser = ko.observable({
                isAuthenticated: false,
                userData: undefined
            });

            self.signInDialogParameter = {
                currentUser: self.currentUser
            }

            self.signUpDialogParameter = {
                currentUser: self.currentUser
            }

            self.contactDialogParamter = undefined;

            self.signOut = function () {
                api.users.signOut().then(function () {
                    console.log("User signed out");
                });
            };

            self.initialize = function (params) {
                if (params) {
                    self.domain(ko.unwrap(params.domain) || 'FuldaFlats.de');
                    self.logoUrl(ko.unwrap(params.logoUrl) || '');
                    self.smallLogoUrl(ko.unwrap(params.smallLogoUrl) || '');

                    self.homePageInfo(ko.unwrap(params.homePageInfo) || '');
                    self.myProfilePageInfo(ko.unwrap(params.myProfilePageInfo) || '');
                    self.becomeLandlordPageInfo(ko.unwrap(params.becomeLandlordPageInfo) || '');
                    self.newOfferPageInfo(ko.unwrap(params.newOfferPageInfo) || '');
                    self.legalDisclosurePageInfo(ko.unwrap(params.legalDisclosurePageInfo) || '');
                    self.termsOfUsePageInfo(ko.unwrap(params.termsOfUsePageInfo) || '');

                    if (params.currentUser && ko.isObservable(params.currentUser)) {
                        self.currentUser = params.currentUser;

                        self.signInDialogParameter = {
                            currentUser: self.currentUser
                        }

                        self.signUpDialogParameter = {
                            currentUser: self.currentUser,
                            domain: self.domain,
                            termsOfUsePageInfo: self.termsOfUsePageInfo,
                            myProfilePageInfo: params.myProfilePageInfo
                        }
                    }

                    if (params.contactData) {
                        self.contactDialogParamter = ko.unwrap(params.contactData);
                    }
                }
            };
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template

                    ko.components.register("sign-in", signInModalDialogComponent);
                    ko.components.register("sign-up", signUpModalDialogComponent);
                    ko.components.register("error", errorModalDialogComponent);
                    ko.components.register("contact", contactModalDialogComponent);

                    var navigation = new NavigationModel($, ko, api);
                    navigation.initialize(params);
                    return navigation;
                }
            },
            template: componentTemplate
        };
    });