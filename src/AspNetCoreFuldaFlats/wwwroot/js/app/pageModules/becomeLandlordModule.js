/************************************************************
 * File:            becomeLandlordModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Become landlord page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/becomeLandlordBar/becomeLandlordBar.component'
], function (ko, breadcrumbBarComponent, becomeLandlordBarComponent) {
    function BecomeLandlordPageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("become-landlord", becomeLandlordBarComponent);

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.becomeLandlord;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }
            }
        };
    };

    return new BecomeLandlordPageModule();
});


