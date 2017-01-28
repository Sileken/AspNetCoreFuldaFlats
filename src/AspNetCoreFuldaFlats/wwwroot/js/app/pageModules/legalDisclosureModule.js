/************************************************************
 * File:            legalDisclosureModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         07.12.2016
 * Description:     legal disclosure page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/legalDisclosureBar/legalDisclosureBar.component'
], function (ko, breadcrumbBarComponent, legalDisclosureBarComponent) {
    function LegalDisclosurePageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("legal-disclosure", legalDisclosureBarComponent);

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.legalDisclosure;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }

                appModel.legalDisclosureBar = appModel.contactData;
            }
        };
    };

    return new LegalDisclosurePageModule();
});


