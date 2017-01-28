/************************************************************
 * File:            changePasswordBar.component.js
 * Author:          Plisam Ekpai-Laodema
 * LastMod:         08.12.2016
 * Description:     JS Component Handler for change password bar.
 ************************************************************/
define(['text!./changePasswordBar.component.html', 'css!./changePasswordBar.component.css', 'knockout', 'jquery'],
    function(componentTemplate, componentCss, ko, $, moment) {
        // your model functions and variables
        function ChangePasswordModel(params) {
            var self = this;
            self.passwordOld = ko.observable();
            self.passwordNew = ko.observable();
            self.passwordNew2 = ko.observable();


            self.accept = function(){
                var changePassword = {
                    	passwordOld : self.passwordOld,
                        passwordNew : self.passwordNew,
                };
                
                $.ajax({
                    method: "PUT",
                    url: "/api/users/changePassword",
                    dataType: "json",
                    contentType: "application/json",
                    data: ko.toJSON(changePassword),
                    success: function(data, status, req){
                        window.location = "/pages/myProfile";
                    },
                    error: function(req, status, error){
                        if(req.status == 200){
                            window.location ="/pages/changePassword";
                            return;
                        }
                        errorCallback(JSON.parse(req.responseText));
                    }
                });
            }

        }

        return {
            viewModel: {
                createViewModel: function(params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    return new ChangePasswordModel(params);
                }
            },
            template: componentTemplate
        };
    });