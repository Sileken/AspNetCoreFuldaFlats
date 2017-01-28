/************************************************************
 * File:            knockout-date.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Knockout binding for possible large text.
 *  ************************************************************/
(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(["knockout", 'jquery'], factory);
    } else {
        factory(window.ko, $);
    }
})(function (ko, $) {
    ko.bindingHandlers.truncate = {
        update: function (element, valueAccessor, allBindingsAccessor, context) {
            var value = ko.unwrap(valueAccessor());
            var maxLength = ko.unwrap(allBindingsAccessor().maxLength);
            var truncated = "";
            if (value && value.toString().length > maxLength) {
                var truncated = value.toString().substring(0, maxLength - 1);
                truncated = truncated.substring(0, truncated.lastIndexOf(' '));
                truncated = truncated + '\u2026';
                $(element).text(truncated);
            } else {
                truncated = value;
            }

            $(element).text(truncated);
        }
    };
});