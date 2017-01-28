/************************************************************
 * File:            signupModalDialog.component.js
 * Author:          Patrick Hasenauer
 * LastMod:         09.12.2016
 * Description:     JS Component Handler for sign up dialog.
 ************************************************************/
define(['text!./signUpModalDialog.component.html', 'css!./signUpModalDialog.component.css',
    'knockout', 'jquery', 'fuldaflatsApiClient', "moment"],
    function (componentTemplate, componentCss, ko, $, api, moment) {
        function SignUpModel($, ko, api) {
            var self = this;
            // your model functions and variables

            self.domain = ko.observable("FuldaFlats.de");
            self.termsOfUsePageInfo = ko.observable({});
            self.myProfilePageInfo = ko.observable();

            self.modalDialogContainer = ko.observable();

            self.firstName = ko.observable();
            self.lastName = ko.observable();
            self.eMail = ko.observable();
            self.password = ko.observable();
            self.confirmPassword = ko.observable();
            self.genders = ko.observableArray(["female", "male"]);
            self.birthDay = ko.observable();
            self.selectedGender = ko.observable();
            self.termsOfUseAgreement = ko.observable(false);

            self.invalidFirstName = ko.observable(false);
            self.invalidLastName = ko.observable(false);
            self.invalidEmail = ko.observable(false);
            self.invalidPassword = ko.observable(false);
            self.invalidBirthday = ko.observable(false);
            self.invalidGender = ko.observable(false);

            self.internalError = ko.observable(false);
            self.isSignUpDataError = ko.observable(false);
            self.signUpDataErrorMessage = ko.observable("");

            self.resetInvalidFields = function () {
                self.invalidFirstName(false);
                self.invalidLastName(false);
                self.invalidEmail(false);
                self.invalidPassword(false);
                self.invalidBirthday(false);
                self.invalidGender(false);
            };

            function focusInput() {
                self.modalDialogContainer().find("[autofocus]:first").focus();
            };

            function resetErrors() {
                self.internalError(false);
                self.isSignUpDataError(false);
                self.signUpDataErrorMessage("");
                self.resetInvalidFields();
            };

            function processResponseErrorMessage(xhr) {
                var field = Object.keys(xhr.responseJSON)[0];
                var errorMessage = xhr.responseJSON[field][0];
                if (errorMessage) {
                    if (errorMessage.lastIndexOf(".") !== errorMessage.length - 1) {
                        errorMessage = errorMessage + ".";

                        switch (field) {
                            case "firstName":
                                self.invalidFirstName(true);
                                break;
                            case "lastName":
                                self.invalidLastName(true);
                                break;
                            case "email":
                                self.invalidEmail(true);
                                break;
                            case "password":
                                self.invalidPassword(true);
                                break;
                            case "birthday":
                                self.invalidBirthday(true);
                                break;
                            case "gender":
                                self.invalidGender(true);
                                break;
                        }
                    }

                    self.isSignUpDataError(true);
                    self.signUpDataErrorMessage(errorMessage);
                } else {
                    throw Error("Unable to extract error message from server response.")
                }
            };

            self.signUp = function () {
                if (self.formIsValid()) {
                    resetErrors();

                    var birthDayValue = ko.unwrap(self.birthDay);
                    if (birthDayValue instanceof Date) {
                        birthDayValue = moment(birthDayValue).format();
                    } else {
                        birthDayValue = "";
                    }
                    var signUpData = {
                        firstName: self.firstName,
                        lastName: self.lastName,
                        email: self.eMail,
                        password: self.password,
                        birthday: birthDayValue,
                        gender: self.selectedGender,
                        type: 1
                    }

                    api.users.signUp(signUpData).then(
                        function (userObject) {
                            if (userObject) {
                                if (self.myProfilePageInfo() && self.myProfilePageInfo().url) {
                                    window.location.href = self.myProfilePageInfo().url;
                                } else {
                                    self.currentUser(userObject);
                                    if (self.modalDialogContainer()) {
                                        self.modalDialogContainer().modal("hide");
                                    }
                                }
                            } else {
                                self.internalError(true);
                            }
                        },
                        function (xhr) {
                            if (xhr && xhr.status === 400 && xhr.responseJSON
                                && (xhr.responseJSON.lastName || xhr.responseJSON.firstName || xhr.responseJSON.email
                                    || xhr.responseJSON.password || xhr.responseJSON.gender || xhr.responseJSON.birthday)) {
                                var errorMessage = "";
                                try {
                                    processResponseErrorMessage(xhr);
                                } catch (ex) {
                                    console.error("Error while extracting sign up error from server response.")
                                    self.internalError(true);
                                }
                            } else {
                                self.internalError(true);
                            }
                        });
                } else {
                    self.isSignUpDataError(true);
                    self.signUpDataErrorMessage("Please fill all fields.");
                }
            };

            self.isValidPassword = ko.computed(function () {
                var isValidPassword = false;

                if (self.password() && self.password().length >= 5 &&
                    self.confirmPassword() && self.password() === self.confirmPassword()) {
                    isValidPassword = true;
                    resetErrors();
                }

                return isValidPassword;
            });

            self.isValidEmail = ko.computed(function () {
                var isValidEmail = false;
                var regEmail = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                if (self.eMail() && regEmail.test(self.eMail())) {
                    isValidEmail = true;
                    resetErrors();
                }

                return isValidEmail;
            });

            self.isValidBirthday = ko.computed(function () {
                var isValidBirthday = false;

                if (self.birthDay() instanceof Date && self.birthDay().toDateString() !== "Invalid Date") {
                    isValidBirthday = true;
                }

                return isValidBirthday;
            });

            self.formIsValid = ko.computed(function () {
                var isValid = false;

                if (self.firstName() && self.lastName() && self.isValidEmail() && self.isValidPassword() && self.isValidBirthday() && self.selectedGender()
                    && self.termsOfUseAgreement()) {
                    resetErrors();
                    isValid = true;
                }

                return isValid;
            });

            self.resetDialog = function () {
                self.firstName("");
                self.lastName("");
                self.eMail("");
                self.birthDay(undefined);
                self.password("");
                self.confirmPassword("");
                self.selectedGender("");
                self.termsOfUseAgreement(false);
                resetErrors();
            };

            self.optionsAfterRender = function (option, item) {
                ko.applyBindingsToNode(option, {
                    disable: !item
                }, item);
            };

            self.initialize = function (params, dialogContainer) {
                if (dialogContainer) {
                    dialogContainer.on('shown.bs.modal', focusInput);
                    dialogContainer.on('show.bs.modal', self.resetDialog);
                    dialogContainer.on('hidden.bs.modal', self.resetDialog);
                    self.modalDialogContainer(dialogContainer);
                }

                if (params) {
                    self.domain(ko.unwrap(params.domain) || 'FuldaFlats.de');
                    self.termsOfUsePageInfo(ko.unwrap(params.termsOfUsePageInfo) || {});
                    self.myProfilePageInfo(ko.unwrap(params.myProfilePageInfo) || {});

                    if (params.currentUser && ko.isObservable(params.currentUser)) {
                        self.currentUser = params.currentUser;
                    }
                }
            };
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    var viewModel = null;

                    var templateRoot = $(componentInfo.element);
                    if (templateRoot.length > 0) {
                        var signUpDialog = templateRoot.find("#signUpModalDialog");
                        if (signUpDialog.length > 0) {
                            viewModel = new SignUpModel($, ko, api);
                            viewModel.initialize(params, signUpDialog);
                        }
                    }

                    return viewModel;
                }
            },
            template: componentTemplate
        };
    });