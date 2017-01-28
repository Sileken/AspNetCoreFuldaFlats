/************************************************************
 * File:            changePasswordModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Change password page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/changePasswordBar/changePasswordBar.component'
], function (ko, breadcrumbBarComponent, changePasswordBarComponent) {
    function ChangePasswordPageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("change-password", changePasswordBarComponent);

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.changePassword;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }
            }
        };
    };

    return new ChangePasswordPageModule();
});


