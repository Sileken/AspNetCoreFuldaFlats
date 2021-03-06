/************************************************************
 * File:            becomeLandlordBar.component.js
 * Author:          Michelle Rothenbuecher, Patrick Hasenauer
 * LastMod:         10.12.2016
 * Description:     JS Component Handler for become landlord bar.
 ************************************************************/
define(['text!./becomeLandlordBar.component.html', 'css!./becomeLandlordBar.component.css', 'knockout', 'jquery', 'moment'],
    function (componentTemplate, componentCss, ko, $, moment) {

        function BecomeLandlordModel(params) {
            var self = this;
            self.currentUser = ko.observable();
            self.userChanges = ko.observable({
                birthday: ko.observable(new Date())
            });

            $.ajax({
                method: "GET",
                url: "/api/users/me",
                contentType: "application/json",
                success: function (data, status, req) {
                    if (data.birthday) {
                        data.birthday = moment(data.birthday).format('L');
                    }
                    self.currentUser(data);
                    self.userChanges().birthday(new Date(data.birthday));
                    if(data.type == 2)
                    {
                        window.location = "/pages/editProfileData";
                    }
                },
                error: function(req, status, err){
                    window.location = "/";
                }
            });

            self.showTab = function (scope, event) {
                event.preventDefault()
                $(event.currentTarget).tab('show')
            }

            self.upgrade = function () {
                self.userChanges().gender = self.currentUser().gender;
                var birthDayValue = ko.unwrap(self.userChanges().birthday);
                if (birthDayValue instanceof Date) {
                    birthDayValue = moment(birthDayValue).format();
                } else {
                    birthDayValue = "";
                }
                self.userChanges().birthday(birthDayValue);
                
                var _userChanges = ko.toJSON(self.userChanges);

                $("input").each(function () {
                    $(this).removeClass("errorField");
                })

                $.ajax({
                    method: "PUT",
                    url: "/api/users/upgrade",
                    dataType: "application/json",
                    contentType: "application/json",
                    data: _userChanges,
                    success: function (data, status, req) {
                        window.location = "/pages/myProfile";
                    },
                    error: function (req, status, error) {
                        if (req.status == 200) {
                            window.location = "/pages/myProfile";
                            return;
                        }
                        var errorBody = JSON.parse(req.responseText);

                        for (var singleError in errorBody) {
                            $("#" + singleError).addClass("errorField");
                        }
                        self.userChanges().birthday(new Date(self.userChanges().birthday()));
                        errorCallback(errorBody);
                    }
                });
            }
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    return new BecomeLandlordModel(params);
                }
            },
            template: componentTemplate
        };
    });