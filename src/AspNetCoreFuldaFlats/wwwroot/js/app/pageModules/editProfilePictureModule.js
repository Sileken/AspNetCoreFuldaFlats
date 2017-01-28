/************************************************************
 * File:            editProfilePictureModule.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Edit profile picture page module.
 ************************************************************/
define([
    'knockout',
    'app/components/breadcrumbBar/breadcrumbBar.component',
    'app/components/editProfilePictureBar/editProfilePictureBar.component'
], function (ko, breadcrumbBarComponent, editProfilePictureBarComponent) {
    function EditProfilePicturePageModule() {
        var self = this;

        ko.components.register("breadcrumb", breadcrumbBarComponent);
        ko.components.register("edit-profile-picture", editProfilePictureBarComponent);

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.editProfilePicture;

                appModel.breadcrumbBar = {
                    homePageInfo: appModel.pages.home,
                    currentPageInfo: appModel.currentPage
                }
            }
        };
    };

    return new EditProfilePicturePageModule();
});


