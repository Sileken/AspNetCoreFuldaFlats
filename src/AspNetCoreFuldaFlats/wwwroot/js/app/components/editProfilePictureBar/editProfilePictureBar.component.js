/************************************************************
 * File:            editProfilePictureBar.component.js
 * Author:          Michelle Rothenbücher, Patrick Hasenauer
 * LastMod:         05.12.2016
 * Description:     JS Component Handler for edit profile picture bar.
 ************************************************************/
define(['text!./editProfilePictureBar.component.html', 'css!./editProfilePictureBar.component.css', 'knockout', 'jquery'],
    function (componentTemplate, componentCss, ko, $) {
        function EditProfilePictureModel(params) {
            var self = this;
            self.currentUser = ko.observable();

            $.ajax({
                method: "GET",
                url: "/api/users/me",
                contentType: "application/json",
                success: function (data, status, req) {
                    self.currentUser(data);
                },
                error: function (req, status, err) {
                    window.location = "/";
                }
            });

            self.openFileDialog = function () {
                var elem = document.getElementById("hiddenFileInput");
                if (elem && document.createEvent) {
                    var evt = document.createEvent("MouseEvents");
                    evt.initEvent("click", true, false);
                    elem.dispatchEvent(evt);
                }
            }

            var percentageXhr = function () {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = (evt.loaded / evt.total) * 100;
                        percentComplete = percentComplete + "%";
                        $("#uploadProgressBar").css("width", percentComplete);
                    }
                }, false);
                return xhr;
            };

            self.uploadPicture = function () {
                var form = new FormData();
                form.append("file", $("#hiddenFileInput")[0].files[0]);
                $.ajax({
                    xhr: percentageXhr,
                    async: true,
                    url: "/api/files/profilePicture",
                    method: "POST",
                    processData: false,
                    contentType: false,
                    mimeType: "multipart/form-data",
                    data: form,
                    success: function (data, status, req) {
                        console.log("SUCCESS!");
                        window.location = "/pages/myProfile";
                        return;
                    },
                    error: function (req, status, error) {
                        console.error(req);
                        $("#alertProfilePictureUpload").text(JSON.parse(req.responseText).error);
                        $("#alertProfilePictureUpload").show();
                    }
                });
            }

            self.standardPicture = function (pictureUrl, data, event) {

                var pictureData = {};
                pictureData.img = pictureUrl;
                console.log(pictureData);
                $.ajax({
                    method: "PUT",
                    url: "/api/users/standardPicture",
                    dataType: "application/json",
                    contentType: "application/json",
                    data: JSON.stringify(pictureData),
                    success: function (data, status, req) {
                        window.location = "/pages/myProfile";
                    },
                    error: function (req, status, error) {
                        if (req.status == 200) {
                            window.location = "/pages/myProfile";
                            return;
                        }
                        //errorCallback(error);
                        console.log(req);
                    }
                });
            }

        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    return new EditProfilePictureModel(params);
                }
            },
            template: componentTemplate
        };
    });