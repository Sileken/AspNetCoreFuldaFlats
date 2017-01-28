/************************************************************
 * File:            searchModule
 * Author:          Patrick Hasenauer, Jonas Kleinkauf
 * LastMod:         02.12.2016
 * Description:     Search page module.
 ************************************************************/
define([
    'knockout',
    'app/components/searchBar/searchBar.component',
    'app/components/searchResultBar/searchResultBar.component',
], function (ko, searchBarComponent, searchResultBarComponent) {
    function SearchResultPageModule() {
        var self = this;

        ko.components.register("search", searchBarComponent);
        ko.components.register("search-results", searchResultBarComponent);

        self.search = function() {
            console.log("GO SEARCH GETS HERE");
            var searchQuery = ko.toJSON(model.queryParameter);
            console.log(searchQuery);
            $.ajax({
                url: "/api/offers/search",
                type: "post",
                dataType: "application/json",
                contentType: "application/json",
                data: searchQuery,
                success: function (data, status, req) {
                    searchResultBarComponent.getSearchResults();
                },
                error: function (req, status, err) {
                    console.log("Error!");
                    console.log(err);
                }
            });
        }

        self.initialize = function (appModel) {
            if (appModel) {
                appModel.currentPage = appModel.pages.search;

                appModel.searchPanelBar = {
                    offerTypes: appModel.offerTypes,
                    searchPageInfo: appModel.pages.search
                }
            }
        };
    };

    return new SearchResultPageModule();
});


