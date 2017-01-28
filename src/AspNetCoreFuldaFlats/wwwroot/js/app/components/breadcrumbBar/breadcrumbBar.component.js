/************************************************************
 * File:            breadcrumbBar.component.js
 * Author:          Patrick Hasenauer
 * LastMod:         13.12.2016
 * Description:     JS Component Handler for breadcrumb bar.
 ************************************************************/
define(['text!./breadcrumbBar.component.html', 'css!./breadcrumbBar.component.css', 'knockout'], function (componentTemplate,componentCss, ko) {
    function BreadcrumbModel(params) {
        var self = this;

        self.homePageInfo = ko.observable()
        self.currentPageInfo = ko.observable();

        if (params) {
            self.homePageInfo(ko.unwrap(params.homePageInfo) || '/');
            self.currentPageInfo(ko.unwrap(params.currentPageInfo) || '');
        }
    }

    return {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                // componentInfo contains for example the root element from the component template
                return new BreadcrumbModel(params);
            }
        },
        template: componentTemplate
    };
});