/************************************************************
 * File:            signinModalDialog.component.js
 * Author:          Patrick Hasenauer
 * LastMod:         09.12.2016
 * Description:     JS Component Handler for sign in dialog.
 ************************************************************/
define(['text!./signInModalDialog.component.html', 'css!./signInModalDialog.component.css', 'knockout', 'jquery', 'fuldaflatsApiClient'],
    function (componentTemplate, componentCss, ko, $, api) {
        function SignInModel($, ko, api) {
            var self = this;

            self.currentUser = ko.observable({
                isAuthenticated: false,
                userData: undefined
            });

            self.eMail = ko.observable();
            self.password = ko.observable();
            self.rememberMe = ko.observable(false);
            self.modalDialogContainer = ko.observable();

            self.internalError = ko.observable(false);
            self.invalidCredentials = ko.observable(false);
            self.accountIsLocked = ko.observable(false);

            function focusInput() {
                self.modalDialogContainer().find("[autofocus]:first").focus();
            };

            function resetErrors() {
                self.internalError(false);
                self.invalidCredentials(false);
                self.accountIsLocked(false);
            };

            self.isValidPassword = ko.computed(function () {
                var isValidPassword = false;

                if (self.password()) {
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

            self.signIn = function (mode, event) {
                if (self.isValidEmail() && self.isValidPassword()) {
                    resetErrors();

                    api.users.signIn(self.eMail(), self.password(), self.rememberMe()).then(
                        function (userObject) {
                            if (userObject) {
                                self.currentUser(userObject);
                                if (self.modalDialogContainer()) {
                                    self.modalDialogContainer().modal("hide");
                                }
                            } else {
                                self.internalError(true);
                            }
                        },
                        function (xhr) {
                            if (xhr.status === 403 || xhr.status === 400) {
                                self.invalidCredentials(true);
                            } else if (xhr.status === 423) {
                                resetErrors();
                                self.accountIsLocked(true);
                            } else {
                                resetErrors()
                                self.internalError(true);
                            }
                        });
                }
            };

            self.resetDialog = function () {
                self.eMail("");
                self.password("");
                self.rememberMe(false);
                resetErrors();
            };

            self.initialize = function (params, dialogContainer) {
                if (dialogContainer) {
                    dialogContainer.on('shown.bs.modal', focusInput);
                    dialogContainer.on('show.bs.modal', self.resetDialog)
                    dialogContainer.on('hidden.bs.modal', self.resetDialog)
                    self.modalDialogContainer(dialogContainer);
                }

                if (params) {
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
                        var signInDialog = templateRoot.find("#signInModalDialog");
                        if (signInDialog.length > 0) {
                            var signIn = new SignInModel($, ko, api);
                            signIn.initialize(params, signInDialog);
                        }
                    }

                    return signIn;
                }
            },
            template: componentTemplate
        };
    });