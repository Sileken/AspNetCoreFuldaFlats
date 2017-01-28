/************************************************************
 * File:            editOfferDetailsModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Edit offer details page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/editOfferDetailsBar/editOfferDetailsBar.component'
], function(ko, breadcrumbBarComponent, editOfferDetailsBarComponent) {
    function EditOfferDetailsPageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("edit-offer-details", editOfferDetailsBarComponent);

        self.initialize = function(appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.editOfferDetails;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }

                appModel.editOfferDetailsBar = {
                    currentUser: appModel.currentUser,
                    offerDetailsPageInfo: appModel.pages.offerDetails
                }
            }
        };
    };

    return new EditOfferDetailsPageModule();
});


