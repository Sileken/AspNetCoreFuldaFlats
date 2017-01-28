/************************************************************
 * File:            errorModalDialog.component.js
 * Author:          Jonas Kleinkauf
 * LastMod:         10.12.2016
 * Description:     JS Component Handler for error modal dialog.
 ************************************************************/
define(['text!./errorModalDialog.component.html', 'css!./errorModalDialog.component.css', 'knockout', 'jquery'],
    function(componentTemplate, componentCss, ko, $) {
        function ErrorModel($, ko) {
            var self = this;

            self.errors = ko.observableArray([]);

            self.showErrors = function(_errors){
                self.errors.removeAll();
                for(var i in _errors){
                    if(typeof _errors == "string"){
                        self.errors.push(_errors);
                        break;;
                    }
                    if(typeof _errors[i] == "string"){
                        self.errors.push(_errors);
                        continue;
                    }
                    for(var k in _errors[i]){
                        self.errors.push(_errors[i][k]);
                    }
                }
                $('#errorModalDialog').modal();    
            }

            errorCallback = self.showErrors;

            self.initialize = function(params, dialogContainer) {
            };
        }

        return {
            viewModel: {
                createViewModel: function(params, componentInfo) {
                    return new ErrorModel($, ko);
                }
            },
            template: componentTemplate
        };
    });