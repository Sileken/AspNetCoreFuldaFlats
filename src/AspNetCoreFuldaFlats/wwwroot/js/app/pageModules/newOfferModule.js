/************************************************************
 * File:            newOfferModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         08.12.2016
 * Description:     New Offer page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/newOfferBar/newOfferBar.component'
], function (ko, breadcrumbBarComponent, newOfferBarComponent) {
    function NewOfferPageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("new-offer", newOfferBarComponent);

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.newOffer;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                };

                appModel.newOfferBar = {
                    currentUser: appModel.currentUser,
                    offerDetailsPageInfo: appModel.pages.offerDetails
                };
            }
        };
    };

    return new NewOfferPageModule();
});


