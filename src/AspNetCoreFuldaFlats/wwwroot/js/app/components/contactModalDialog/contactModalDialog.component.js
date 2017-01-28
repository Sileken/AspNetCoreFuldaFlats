/************************************************************
 * File:            contactModalDialog.component.js
 * Author:          Patrick Hasenauer
 * LastMod:         10.12.2016
 * Description:     JS Component Handler for contact modal dialog.
 ************************************************************/
define(['text!./contactModalDialog.component.html', 'css!./contactModalDialog.component.css', 'knockout', 'jquery', 'leaflet'],
    function (componentTemplate, componentCss, ko, $, L) {
        function ContactInModel($, ko, L) {
            var self = this;
            var dialogContainer = ko.observable();

            var leafletMapElement = ko.observable();
            var leafletMap = ko.observable();
            self.organisationName = ko.observable();
            self.street = ko.observable();
            self.housenumber = ko.observable();
            self.city = ko.observable();
            self.zipCode = ko.observable();
            self.country = ko.observable();
            self.phone = ko.observable();
            self.email = ko.observable();
            self.leafletMapOptions = ko.observable();

            function removeMap() {
                if (leafletMap() && typeof leafletMap().remove === "function") {
                    leafletMap().remove();
                }
            };

            function initializeMap(options) {
                if (L && leafletMapElement() && leafletMapElement().length > 0
                    && options && options.view && options.view.coordinats && options.view.zoom) {
                    var map = L.map(leafletMapElement()[0]);
                    map.setView(options.view.coordinats, options.view.zoom);

                    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                        attribution: options.attribution,
                    }).addTo(map);

                    if (options.markers && options.markers.length > 0) {
                        $.each(options.markers, function (index, marker) {
                            if (marker.iconUrl && marker.iconSize && marker.iconAnchor
                                && marker.popupAnchor && marker.coordinates && marker.popupMarkup) {
                                var mapMarker = L.marker(marker.coordinates, { icon: L.icon(marker) })
                                mapMarker.bindPopup(marker.popupMarkup);
                                mapMarker.addTo(map);
                            }
                        });
                    }

                    leafletMap(map);
                }
            };

            self.initialize = function (params, dialogContainerElement, contactMapElement) {
                if (dialogContainerElement) {
                    dialogContainer(dialogContainerElement);
                    leafletMapElement(contactMapElement);

                    $(dialogContainer).on('hidden.bs.modal', function () {
                        //removeMap();
                    });

                    $(dialogContainer).on('shown.bs.modal', function () {
                        //initializeMap(self.leafletMapOptions());
                    });
                }

                if (params) {
                    self.leafletMapOptions(params.leafletMapOptions || undefined);
                    self.organisationName(params.organisationName || "");
                    self.street(params.street || "");
                    self.housenumber(params.housenumber || "");
                    self.city(params.city || "");
                    self.zipCode(params.zipCode || "");
                    self.country(params.country || "");
                    self.phone(params.phone || "");
                    self.email(params.email || "");
                }

            };
        }

        return {
            viewModel: {
                createViewModel: function (params, componentInfo) {
                    // componentInfo contains for example the root element from the component template
                    var viewModel = null;

                    var templateRoot = $(componentInfo.element);
                    if (templateRoot.length > 0) {
                        var contactDialog = templateRoot.find("#contactModalDialog");
                        var contactMap = contactDialog.find("#contactMap");
                        if (contactDialog.length > 0) {
                            var contact = new ContactInModel($, ko, L);
                            contact.initialize(params, contactDialog, contactMap);
                        }
                    }

                    return contact;
                }
            },
            template: componentTemplate
        };
    });