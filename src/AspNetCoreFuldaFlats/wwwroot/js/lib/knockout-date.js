/************************************************************
 * File:            knockout-date.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Knockout binding date value binding for date input fields
 *  ************************************************************/
(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(["knockout", "moment"], factory);
    } else {
        factory(window.ko, moment);
    }
})(function (ko, moment) {
    ko.bindingHandlers.date = {
        init: function (element, valueAccessor) {
            var dateConverter = ko.computed({
                read: function () {
                    var returnValue = "";
                    var currentValue = ko.unwrap(valueAccessor());
                    if (currentValue instanceof Date) {
                        return moment(currentValue).format("YYYY-MM-DD"); // date input required format
                    }
                    return returnValue;
                },
                write: function (newValue) {
                    var newDate = moment(newValue, 'YYYY-MM-DD', true);
                    if (newDate.isValid()) {
                        valueAccessor()(newDate.toDate());
                    }else{
                        valueAccessor()(undefined);
                    }
                }
            });

            ko.applyBindingsToNode(element, { value: dateConverter });
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var currentValue = ko.unwrap(valueAccessor);
            if (currentValue instanceof Date) {
                element.value = moment(currentValue).format("YYYY-MM-DD");
            }
        }
    };
});

