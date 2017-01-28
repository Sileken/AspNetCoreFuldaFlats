/************************************************************
 * File:            ie-custom-event-polyfill.js
 * Author:          Patrick Hasenauer
 * LastMod:         02.12.2016
 * Description:     Polyfill for IE window.CustomEvent
 ************************************************************/
/* Source. https://developer.mozilla.org/en-US/docs/Web/API/CustomEvent/CustomEvent */
(function () {

    if (typeof window.CustomEvent === "function") return false;

    function CustomEvent(event, params) {
        params = params || { bubbles: false, cancelable: false, detail: undefined };
        var evt = document.createEvent('CustomEvent');
        evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
        return evt;
    }

    CustomEvent.prototype = window.Event.prototype;

    window.CustomEvent = CustomEvent;
})();