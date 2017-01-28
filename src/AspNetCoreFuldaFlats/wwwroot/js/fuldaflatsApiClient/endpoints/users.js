/************************************************************
 * File:            users.js
 * Author:          Patrick Hasenauer, Jonas Kleinkauf
 * LastMod:         09.12.2016
 * Description:     Javascript api client endpoints for users.
 ************************************************************/
define(["jquery", 'knockout'], function($, ko) {

    function UsersEndpoint(usersEndpointUrls, offerTypes) {
        var self = this;
        var endpointUrls = usersEndpointUrls;

        self.getUserModel = function() {
            return {
                isAuthenticated: ko.observable(false),
                userData: ko.observable(
                    {
                        id: ko.observable(),
                        email: ko.observable(),
                        type: ko.observable(),
                        firstName: ko.observable(),
                        lastName: ko.observable(),
                        birthday: ko.observable(),
                        upgradeDate: ko.observable(),
                        creationDate: ko.observable(),
                        phoneNumber: ko.observable(),
                        zipCode: ko.observable(),
                        city: ko.observable(),
                        street: ko.observable(),
                        houseNumber: ko.observable(),
                        gender: ko.observable(),
                        officeAddress: ko.observable(),
                        averageRating: ko.observable(),
                        profilePicture: ko.observable(),
                        favorites: ko.observableArray(),
                        offers: ko.observableArray()
                    }
                )
            };
        }

        // sign up
        self.signUp = function(signUpData) {
            var defer = $.Deferred();
            if (signUpData) {
                $.ajax({
                    url: endpointUrls.users,
                    method: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: ko.toJSON(signUpData)
                }).done(function(data, textStatus, jqXHR) {
                    if (jqXHR.status === 201 && data) {
                        var user = self.getUserModel();
                        user.isAuthenticated = true,
                            user.userData = data;
                        defer.resolve(user);
                    } else {
                        console.error("Failed to sing up the current user. Invalid response data.");
                        defer.reject(jqXHR);
                    }
                }).fail(function(jqXHR, textStatus) {
                    console.error("Failed to sing up the current user. Sign Up request failed.");
                    defer.reject(jqXHR);
                });
            } else {
                defer.reject("Empty sign up data.");
            }

            return defer.promise();
        }

        // user sign in
        self.signIn = function(email, password, rememberMe) {
            var defer = $.Deferred();

            var rememberMeValue = false;
            if (ko.unwrap(rememberMe) === true) {
                rememberMeValue = true;
            }

            var userCredentials = {
                email: email,
                password: password,
                rememberMe: rememberMeValue
            };

            $.ajax({
                url: endpointUrls.auth,
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: ko.toJSON(userCredentials)
            }).done(function(data, textStatus, jqXHR) {
                if (jqXHR.status === 200 && data) {
                    var user = self.getUserModel();
                    user.isAuthenticated = true;
                    user.userData = data;
                    defer.resolve(user);
                    executeLoginCallbacks();
                } else {
                    console.error("Failed to sing in the current user. Invalid response data.");
                    defer.reject(jqXHR);
                }
            }).fail(function(jqXHR, textStatus) {
                if (jqXHR.status !== 403) {
                    console.error("Failed to sing in the current user. Sign In request failed.");
                }

                defer.reject(jqXHR);
            });

            return defer.promise();
        };

        // user sign out
        self.signOut = function() {
            var defer = $.Deferred();

            $.ajax({
                url: endpointUrls.auth,
                method: "DELETE",
            }).done(function(data, textStatus, jqXHR) {
                if (jqXHR.status === 204) {
                    var user = self.getUserModel();
                    user.isAuthenticated = false;
                    user.userData = undefined;
                    defer.resolve(user);
                    executeLoginCallbacks();
                    window.location = "/";
                } else {
                    console.error("Failed to sing out the current user. Invalid response data.");
                    defer.reject(jqXHR);
                }
            }).fail(function(jqXHR, textStatus) {
                console.error("Failed to sing out the current user. Sign Out request failed.");
                defer.reject(jqXHR);
            });

            return defer.promise();
        };

        // get current User
        self.getMe = function() {
            var defer = $.Deferred();

            $.ajax({
                url: endpointUrls.me,
                method: "GET"
            }).done(function(requestedUserResults) {
                var userResult = ko.observable();
                if (requestedUserResults) {
                    var user = self.getUserModel();
                    user.isAuthenticated = true;
                    user.userData = requestedUserResults;
                    defer.resolve(user);
                } else {
                    console.error("Failed to get the current user profile. Invalid response data.");
                    defer.reject();
                }
            }).fail(function(jqXHR, textStatus) {
                if (jqXHR.status === 403) {
                    var user = self.getUserModel();
                    user.isAuthenticated = false;
                    user.userData = undefined;
                    defer.resolve(user);
                } else {
                    console.error("Failed to get the current user. User profile request failed.");
                    defer.reject(jqXHR);
                }
            });

            return defer.promise();
        };
    }

    return UsersEndpoint;
});