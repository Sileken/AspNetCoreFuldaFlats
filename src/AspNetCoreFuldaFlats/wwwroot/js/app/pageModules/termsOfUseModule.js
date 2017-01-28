/************************************************************
 * File:            termsOfUseModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Terms of use page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/termsOfUseBar/termsOfUseBar.component'
], function (ko, breadcrumbBarComponent, termsOfUseBarComponent) {
    function TermsOfUseModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("terms-of-use", termsOfUseBarComponent);

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.termsOfUse;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }
            }
        };
    };

    return new TermsOfUseModule();
});


