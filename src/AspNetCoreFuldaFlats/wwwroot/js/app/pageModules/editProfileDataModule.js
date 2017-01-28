/************************************************************
 * File:            editProfileDataModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Edit profile data page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/editProfileDataBar/editProfileDataBar.component'
], function(ko, breadcrumbBarComponent, editProfileDataBarComponent) {
    function EditProfileDataPageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("edit-profile-data", editProfileDataBarComponent);

        self.initialize = function(appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.editProfileData;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }
            }
        };
    };

    return new EditProfileDataPageModule();
});


