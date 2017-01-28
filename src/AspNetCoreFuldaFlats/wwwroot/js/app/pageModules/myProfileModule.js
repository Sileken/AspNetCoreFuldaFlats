/************************************************************
 * File:            myProfileModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     My profile page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/myProfileBar/myProfileBar.component'
], function (ko, breadcrumbBarComponent, myProfileBarComponent) {
    function MyProfilePageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("my-profile", myProfileBarComponent);

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.myProfile;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }
            }
        };
    };

    return new MyProfilePageModule();
});


