/************************************************************
 * File:            termsOfUseNar.component.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     JS Component Handler for terms of use bar.
 ************************************************************/
define(['text!./termsOfUseBar.component.html', 'css!./termsOfUseBar.component.css', 'knockout', 'jquery'],
    function (componentTemplate, componentCss, ko, $) {
        function TermsOfUseModel(params) {
            var self = this;
            // your model functions and variables
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    return new TermsOfUseModel(params);
                }
            },
            template: componentTemplate
        };
    });