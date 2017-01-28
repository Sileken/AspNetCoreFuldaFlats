/************************************************************
 * File:            myProfileBar.component.js
 * Author:          Michelle Rothenbuecher, Patrick Hasenauer
 * LastMod:         10.12.2016
 * Description:     JS Component Handler for my profile bar.
 ************************************************************/
define(['text!./myProfileBar.component.html', 'css!./myProfileBar.component.css', 'knockout', 'jquery', 'moment'],
    function (componentTemplate, componentCss, ko, $, moment) {

        moment.locale('de');

        function MyProfileModel(params) {
            var self = this;
            // your model functions and variables
            self.currentUser = ko.observable();

            function removeFavorite(offerId) {
                  $.ajax({
                    url: "/api/offers/" + offerId + "/favorite",
                    method: "DELETE",
                    success: function(data, status, req){
                        self.getUserdata();

                    },
                    error: function(req, status, err){
                        console.error(req);
                        errorCallback(err);
                    } 
                });
            }

            self.getUserdata = function () {
                $.ajax({
                method: "GET",
                url: "/api/users/me",
                contentType: "application/json",
                    success: function (data, status, req) {
                        if (data.birthday) {
                            data.birthday = moment(data.birthday).format('L');
                        }
                        if (data.favorites) {
                            if (data.favorites.length > 0) {
                                for (var i in data.favorites) {
                                    data.favorites[i].detailsUrl = '/pages/offerDetails?offerId=' + data.favorites[i].id;
                                    data.favorites[i].creationDateFormat = moment(data.favorites[i].creationDate).format('L');
                                    data.favorites[i].removeFavorite = function () {
                                        removeFavorite(this.id);
                                    }
                                }
                            }
                        }
                        if (data.offers) {
                            for (var i in data.offers) {
                                data.offers[i].detailsUrl = '/pages/offerDetails?offerId=' + data.offers[i].id;
                                data.offers[i].editDetailsUrl = '/pages/editOfferDetails?offerId=' + data.offers[i].id;
                                data.offers[i].creationDateFormat = moment(data.offers[i].creationDate).format('L');
                            }
                        }

                        console.log(data);
                        self.currentUser(data);
                },
                error: function (req, status, err) {
                    window.location = "/";
                }
                });
            }

            self.getUserdata();

            self.showTab = function (scope, event) {
                event.preventDefault()
                console.log(event);
                $(event.currentTarget).tab('show')
            }

        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    window.m = new MyProfileModel(params);
                    return m;
                }
            },
            template: componentTemplate
        };
    });