/************************************************************
 * File:            fileUploaderModal.component.js
 * Author:          Franz Weidmann
 * LastMod:         9.12.2016
 * Description:     Logic for the fileUploader component
************************************************************/
define(['text!./fileUploaderModal.component.html',
    'css!./fileUploaderModal.component.css',
    'fuldaflatsApiClient', 'knockout', 'jquery'],
    function(componentTemplate, componentCss, api, ko, $) {
        function FileUploaderModalModel($, ko) {

            var self = this;

            var UPLOAD_LIMIT = 6;
            var UPLOAD_FILESIZE_LIMIT = 5000000; //bytes

            // MediaObjects which are already uploaded
            self.offerMediaObjectsOnline = ko.observableArray([]);
            // MediaObjects which are selected and are uploadable
            self.offerMediaObjectsOffline = ko.observableArray([]);
            self.offerId = ko.observable(getURLParameter("offerId") || undefined);

            self.removeOfflineMediaObject = function(file) {
                self.offerMediaObjectsOffline.remove(file);

                 if (self.offerMediaObjectsOffline().length == 0)
                    $(".uploadAllBtn").hide(); 
            }

            self.deleteOnlineMediaObject = function(file) {                                
                self.offerMediaObjectsOnline.remove(file);
                api.mediaObjects.deleteMediaObjectById(file.id);
            }

            self.uploadMediaObject = function(file) {
                var fileIndex = self.offerMediaObjectsOffline.indexOf(file);
                var form = new FormData();
                form.append("file", file);

                var offerId = ko.unwrap(self.offerId);

                if (self.offerMediaObjectsOnline().length > UPLOAD_LIMIT)
                    newError(fileIndex, "You reached the upload limit of 7 images!");
                else if (file.size > UPLOAD_FILESIZE_LIMIT)
                    newError(fileIndex, "This image exceeds the size limit of 5 MB!");
                else if (file.type.split("/")[0] != "image")
                    newError(fileIndex, "This is not an image!");
                else if (!isNaN(offerId)) {
                    $.ajax({
                        url: "/api/files/offers/" + offerId,
                        method: "POST",
                        async: true,
                        processData: false,
                        contentType: false,
                        mimeType: "multipart/form-data",
                        data: form,
                        xhr: newXHRObject.bind(null, file),
                    }).done(function(data) {
                        self.offerMediaObjectsOffline.remove(file);
                        updateMediaObjectsOnline();

                    }).fail(function(err) {
                        $("#" + fileIndex + ".removeFileBtn").show();
                        $("#" + fileIndex + ".uploadFileBtn").show();
                        $("#" + fileIndex + ".progressMediaObjectUpLoad").hide();
                        $("#" + fileIndex + ".progressMediaObjectUpLoad").css("width", "0");

                        newError(fileIndex, JSON.parse(err.responseText).error);
                    })

                    $("#" + fileIndex + ".removeFileBtn").hide();
                    $("#" + fileIndex + ".uploadFileBtn").hide();
                    $("#" + fileIndex + ".progressMediaObjectUpLoad").show();
                }
            }

            self.uploadAll = function() {
                self.offerMediaObjectsOffline().forEach(function(file) {
                    self.uploadMediaObject(file);
                })
            }

            self.initialize = function(params) {
                if (params && ko.isObservable(params.offerId)) {
                    self.offerId = params.offerId;
                    self.offerId.subscribe(function() {
                        updateMediaObjectsOnline();
                    });
                }

                updateMediaObjectsOnline();

                $("#mediaObjectFile").on("change", function(event) {
                    for (var i = 0; i < event.target.files.length; i++) {
                        self.offerMediaObjectsOffline.push(event.target.files[i]);
                    }

                    if (self.offerMediaObjectsOffline().length > 0)
                        $(".uploadAllBtn").show();
                });

                // "data-bind = click" doesnt work...
                $(".uploadAllBtn").click(function(btn) {
                    self.uploadAll();
                });
            };

            function newError(fileIndex, errMsg) {
                $("#" + fileIndex + ".alertFileUpload").show();
                $("#" + fileIndex + ".alertFileUpload").text(errMsg);
            }

            function updateMediaObjectsOnline() {
                var offerId = ko.unwrap(self.offerId);
                if (!isNaN(offerId)) {
                    var mediaObjectsPromise = api.mediaObjects.findMediaObjectsByOfferID(offerId);
                    mediaObjectsPromise.then(function(data) {
                        self.offerMediaObjectsOnline(data);
                    })
                }
            }

            function newXHRObject(file) {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener("progress", progressUpLoad.bind(null, file), false);
                return xhr;
            }
            function progressUpLoad(file, evt) {
                if (evt.lengthComputable) {
                    var fileIndex = self.offerMediaObjectsOffline.indexOf(file);
                    var percent = Math.round(evt.loaded / evt.total * 100, 2);

                    if (percent >= 53)
                        $("#" + fileIndex + ".progressMediaObjectUpLoad > span").css("color", "white");

                    $("#" + fileIndex + ".progressMediaObjectUpLoad > div").attr("aria-valuenow", percent + "");
                    $("#" + fileIndex + ".progressMediaObjectUpLoad > div").width(percent + "%");
                    $("#" + fileIndex + ".progressMediaObjectUpLoad > span").text(percent + " %");

                }
            }

            function getURLParameter(name) {
                return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
            };

        }
        return {
            viewModel: {
                createViewModel: function(params, componentInfo) {
                    var fileUploader = new FileUploaderModalModel($, ko);
                    fileUploader.initialize(params);
                    return fileUploader;
                }
            },
            template: componentTemplate
        };
    });