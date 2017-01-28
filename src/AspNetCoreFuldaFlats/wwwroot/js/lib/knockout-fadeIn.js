/************************************************************
 * File:            knockout-date.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Knockout binding date value binding for date input fields
 * Source:          http://knockoutjs.com/examples/animatedTransitions.html
 *  ************************************************************/
(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(["knockout", "jquery"], factory);
    } else {
        factory(window.ko, $);
    }
})(function (ko, $) {
    ko.bindingHandlers.fadeVisible = {
        init: function (element, valueAccessor) {
            // Initially set the element to be instantly visible/hidden depending on the value
            var value = valueAccessor();
            $(element).toggle(ko.unwrap(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
        },
        update: function (element, valueAccessor) {
            // Whenever the value subsequently changes, slowly fade the element in or out
            var value = valueAccessor();
            ko.unwrap(value) ? $(element).fadeIn() : $(element).fadeOut();
        }
    };
});