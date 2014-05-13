/// <reference path="Libs/jquery.js"/>
/// <reference path="Libs/knockout.js"/>

(function($) {

    var ApiUrlBase = "/modules/Creuna.ResourceEditor/api";

    function stringIsEmpty(value) {
        if (value) {
            return false;
        } else {
            return true;
        }
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                       .toString(16)
                       .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
               s4() + '-' + s4() + s4() + s4();
    }

    function findTranslation(allTranslations, translationModel) {
        var result;

        $(allTranslations).each(function(idx, tempItem) {
            if (tempItem.key() == translationModel.Key) {
                result = tempItem;
                return;
            }
        });

        return result;
    }

    function DataController() {
        var self = this;

        var loadData = (function(url, callback) {
            $.getJSON(ApiUrlBase + url, callback);
        });

        var saveData = (function(url, data, onDone, onFail) {

            var request = $.ajax({
                url: ApiUrlBase + url,
                data: JSON.stringify(data),
                type: 'POST',
                dataType: 'json',
                contentType: "application/json"
            });

            request.done(onDone);

            request.fail(function(jqXHR, textStatus, errorThrown) {
                onFail(jqXHR, textStatus, errorThrown, data);
            });
        });

        var loadItems = (function(loadItemsUrl, targetArray, dataToItemMappingFunc, loadFinishedCallback) {
            loadData(loadItemsUrl, function(data) {
                var items = $.map(data, dataToItemMappingFunc);
                targetArray.removeAll();
                $(items).each(function(index, item) {
                    targetArray.push(item);
                });

                if (loadFinishedCallback != null) {
                    loadFinishedCallback();
                }
            });
        });

        var loadTranslationsModel = (function(loadItemsUrl, editorModel, dataToItemMappingFunc) {
            loadData(loadItemsUrl, function(data) {
                var items = $.map(data.Items, dataToItemMappingFunc);

                editorModel.statusMessage(data.StatusMessage);
                editorModel.errorMessage(data.ErrorMessage);

                var numberOfItemsToBeShown = data.Items.length + editorModel.numberOfTranslationsToBeShown();
                editorModel.numberOfTranslationsToBeShown(numberOfItemsToBeShown);

                $(items).each(function(index, item) {
                    editorModel.translations.push(item);
                });

                editorModel.totalTranslationsCount(data.Paging.TotalItemsCount);

                /*editorModel.exportFilteredTranslationsIsVisible(editorModel.isExportFilteredVisible());*/

                editorModel.pageNumber(data.Paging.PageIndex);
            });
        });

        var saveItems = (function(saveItemsUrl, targetArray, itemToDataMapingFunc, saveFailedFunc) {
            $(targetArray).each(function(index, item) {
                item.updateStatusMessage('Saving');
            });

            var dataToSave = $.map(targetArray, itemToDataMapingFunc);

            saveData(saveItemsUrl, dataToSave, function(x) {

                $(x).each(function(index, item) {

                    var targetItem = findTranslation(targetArray, item);

                    if (targetItem != null) {
                        targetItem.updateStatusMessage(item.StatusMessage);
                        targetItem.updateErrorMessage(item.ErrorMessage);
                        targetItem.state(item.State);
                        targetItem.canBeExported(item.CanBeExported);
                    }
                });
            }, saveFailedFunc);
        });

        var saveItem = function(saveItemsUrl, item, dataToItemMappingFunc, saveFailedFunc) {
            var items = [item];
            saveItems(saveItemsUrl, items, dataToItemMappingFunc, saveFailedFunc);
        };

        self.loadLanguages = function(targetArray) {
            loadItems("/languages", targetArray, function(dataItem) { return new Language(dataItem); });
        };

        self.loadResourceProviders = (function(targetArray) {
            loadItems("/resourceProviders", targetArray, function(dataItem) { return new ResourceProvider(dataItem); });
        });

        self.getDownloadLinks = (function (url, requestData, targetArray, mappingFunc, callbackFunc) {

            var options =
            {
                dataType: "json",
                url: ApiUrlBase + url,
                traditional: true,
                success: function(data) {
                    var items = $.map(data, mappingFunc);
                    targetArray.removeAll();
                    $(items).each(function(index, item) {
                        targetArray.push(item);
                    });

                    if (callbackFunc != null) {
                        callbackFunc();
                    }
                }
            };

            if (requestData != null) {
                options.data = { filteredKeys: requestData };
            }

            $.ajax(options);
        });

        self.LoadTranslations = (function (editorModel) {
            editorModel.statusMessage('Loading translations - please wait...');
            var requestUrl = "/translations?providerKey=" + editorModel.resourceProviderKey() + "&fromLangKey=" + editorModel.fromLangKey() + "&toLangKey=" + editorModel.toLangKey() + "&page=" + editorModel.pageNumber() + "&pageSize=" + editorModel.pageSize() + "&searchTerm=" + editorModel.searchTerm() + "&hideTranslated=" + editorModel.hideTranslated() + "&orderBy=" + editorModel.orderBy() + "&orderByIsAscending=" + editorModel.orderByIsAscending();

            loadTranslationsModel(requestUrl, editorModel, function (dataItem) {
                
                var translation = new Translation(dataItem);

                translation.editor = editorModel;

                translation.save = function () { self.saveTranslation(translation); };

                translation.handleSaveError = editorModel.handleSaveError;
                
                translation.currentStatusMessage = function (message) { editorModel.statusMessage(message); }
                translation.currentErrorMessage = function (message) { editorModel.errorMessage(message); }

                return translation;
            });
        });

        self.saveTranslation = (function (translation) {
            saveItem("/SaveTranslations", translation, function (translationItem) {
                var translationData = {};
                translationItem.updateData(translationData);
                return translationData;
            }, translation.handleSaveError);
        });
    }

    $(function () {
        ko.applyBindings(new EditorViewModel());
    });


    function Language(data) {
        this.key = ko.observable(data.Key);
        this.name = ko.observable(data.Name);
    }

    function DownloadMessage(data) {
        this.statusMessage = ko.observable(data.StatusMessage);
        this.errorMessage = ko.observable(data.ErrorMessage);
        this.downloadUrl = ko.observable(data.DownloadUrl);
        this.downloaded = false;
    }

    function ResourceProvider(data) {
        this.key = ko.observable(data.Key);
        this.name = ko.observable(data.Name);
    }

    function Translation(data) {

        var self = this;

        self.key = ko.observable(data.Key);
        self.providerKey = ko.observable(data.ProviderKey);
        self.fromLangKey = ko.observable(data.FromLangKey);
        self.toLangKey = ko.observable(data.ToLangKey);
        self.from = ko.observable(data.From);
        self.state = ko.observable(data.State);
        self.canBeExported = ko.observable(data.CanBeExported);

        if (data.To == null || data.To == undefined) {
            data.To = '';
        }

        self.to = ko.observable(data.To);

        self.statusMessage = ko.observable('');
        self.errorMessage = ko.observable('');

        self.currentStatusMessage = null;
        self.currentErrorMessage = null;

        self.updateStatusMessage = (function(message) {
            self.statusMessage(message);

            if (self.currentStatusMessage != null) {
                self.currentStatusMessage(message);
            }
        });

        self.updateErrorMessage = (function(message) {
            self.errorMessage(message);

            if (self.currentErrorMessage != null) {
                self.currentErrorMessage(message);
            }
        });
        
        self.statusMessageVisible = ko.computed(function () {
            var message = this.statusMessage();

            return !stringIsEmpty(message);
        }, this);

        self.errorMessageVisible = ko.computed(function () {
            var message = self.errorMessage();

            return !stringIsEmpty(message);
        }, this);

        this.updateData = (function (data) {
            data.Key = self.key();
            data.ProviderKey = self.providerKey();
            data.FromLangKey = self.fromLangKey();
            data.ToLangKey = self.toLangKey();
            data.From = self.from();
            data.To = self.to();
            data.StatusMessage = self.statusMessage();
            data.ErrorMessage = self.errorMessage();
            data.State = self.state();
            data.CanBeExported = self.canBeExported();
        });

        self.to.subscribe(function () {
            self.save();
        });
    }

    function EditorViewModel() {

        var self = this;

        self.languages = ko.observableArray([]);
        self.resourceProviders = ko.observableArray([]);
        self.translations = ko.observableArray([]);

        self.fromLangKey = ko.observable();
        self.toLangKey = ko.observable();
        self.resourceProviderKey = ko.observable();

        self.totalTranslationsCount = ko.observable();

        self.autoSaveTimeoutId = ko.observable();
        self.autoSaveInterval = ko.observable();

        self.searchTerm = ko.observable();
        self.hideTranslated = ko.observable(false);
        self.pageNumber = ko.observable();
        self.pageSize = ko.observable();
        self.allLanguagesKey = ko.observable('');
        self.isSimpleMode = ko.observable(false);

        self.filesToDownload = ko.observableArray();

        self.orderBy = ko.observable('');
        self.orderByIsAscending = ko.observable(false);

        self.isAdvancedMode = ko.computed(function() {
            return !self.isSimpleMode();
        });

        self.statusMessage = ko.observable('');
        self.errorMessage = ko.observable('');

        self.statusMessageVisible = ko.computed(function() {
            return !stringIsEmpty(self.statusMessage());
        });

        self.errorMessageVisible = ko.computed(function() {
            return !stringIsEmpty(self.errorMessage());
        });

        self.numberOfTranslationsToBeShown = ko.observable(0);

        self.keyFilter = ko.observable('');
        self.fromFilter = ko.observable('');
        self.toFilter = ko.observable('');

        /*self.exportFilteredTranslationsIsVisible = ko.observable(false);*/

        self.checkFilter = (function (filterValue) {
            if (stringIsEmpty(filterValue)) {
                return false;
            } else {
                return true;
            }
        });

        self.handleSaveError = (function (jqXHR, textStatus, errorThrown, translationItems) {
            for (var i = 0; i < translationItems.length; i++) {

                var translation = findTranslation(self.translations(), translationItems[i]);

                if (translation != null) {
                    translation.state('Error');
                    translation.updateStatusMessage('');
                    translation.updateErrorMessage(errorThrown);
                }
            }
        });

        self.keyFilterApplied = ko.computed(function() {
            var filter = self.keyFilter();

            return self.checkFilter(filter);
        });

        self.fromFilterApplied = ko.computed(function () {
            var filter = self.fromFilter();

            return self.checkFilter(filter);
        });

        self.toFilterApplied = ko.computed(function () {
            var filter = self.toFilter();

            return self.checkFilter(filter);
        });

        self.loadedTranslationsCount = ko.computed(function () {
            return this.translations().length;
        }, this);

        self.loadMoreButtonVisible = ko.computed(function () {
            var loadedItemsCount = this.translations().length;
            var totalItemsCount = this.totalTranslationsCount();

            if (loadedItemsCount > 0 && loadedItemsCount < totalItemsCount) {
                return true;
            } else {
                return false;
            }
        }, this);

        self.anyFilterApplied = ko.computed(function() {
            var anyFilterApplied = self.keyFilterApplied() || self.fromFilterApplied() || self.toFilterApplied();

            return anyFilterApplied;
        });

        self.appliedFilterItemsCount = ko.computed(function () {
            if (self.anyFilterApplied()) {
                var totalItemsCount = self.translations().length;
                var visibleItemsCount = self.translationsToShow().length;

                var result = visibleItemsCount.toString() + '/' + totalItemsCount.toString();

                return result;
            }

            return '';
        });

        self.translationsToShow = ko.computed(function() {
            
            var filteredTranslations = self.translations();

            if (self.anyFilterApplied()) {

                filteredTranslations = self.filterField(self.keyFilterApplied(), filteredTranslations, function(translation) {
                    return self.filterTranslation(translation.key(), self.keyFilter());
                });

                filteredTranslations = self.filterField(self.fromFilterApplied(), filteredTranslations, function(translation) {
                    return self.filterTranslation(translation.from(), self.fromFilter());
                });

                filteredTranslations = self.filterField(self.toFilterApplied(), filteredTranslations, function(translation) {
                    return self.filterTranslation(translation.to(), self.toFilter());
                });
            }

            var currentOrderBy = self.orderBy();
            var currentOrderByIsAscending = self.orderByIsAscending();

            filteredTranslations = filteredTranslations.sort(function (left, right) {
                
                if (currentOrderByIsAscending) {
                    switch (currentOrderBy) {
                        case 'Key':
                            return self.sortByFieldAsc(left.key(), right.key());
                            break;
                        case 'From':
                            return self.sortByFieldAsc(left.from(), right.from());
                            break;
                        case 'To':
                            return self.sortByFieldAsc(left.to(), right.to());
                            break;
                        default:
                            return 0;
                    }
                } else {
                    switch (currentOrderBy) {
                        case 'Key':
                            return self.sortByFieldDesc(left.key(), right.key());
                            break;
                        case 'From':
                            return self.sortByFieldDesc(left.from(), right.from());
                            break;
                        case 'To':
                            return self.sortByFieldDesc(left.to(), right.to());
                            break;
                        default:
                            return 0;
                    }
                }
            } );

            return filteredTranslations;
        });

        self.isExportFilteredVisible = function () {
            var visibleTranslations = self.translationsToShow();

            if (visibleTranslations.length > 0) {
                var translationsThatCanBeExported = [];

                $(visibleTranslations).each(function (index, item) {
                    if (item.canBeExported() == true) {
                        translationsThatCanBeExported.push(item);
                    }
                });

                if (translationsThatCanBeExported.length > 0) {
                    return true;
                } else {
                    return false;
                }
            }
        };

        self.sortByFieldAsc = function(leftField, rightField) {
            return leftField == rightField ? 0 : (leftField < rightField ? -1 : 1);
        };

        self.sortByFieldDesc = function (leftField, rightField) {
            return leftField == rightField ? 0 : (leftField > rightField ? -1 : 1);
        };

        self.isOrderedByField = function (field) {
            var currentOrderBy = self.orderBy();

            return currentOrderBy == field;
        };

        self.orderedByKeyAsc = ko.computed(function () {
            return self.isOrderedByField('Key') && self.orderByIsAscending();
        });
        self.orderedByFromAsc = ko.computed(function() {
            return self.isOrderedByField('From') && self.orderByIsAscending();
        });
        self.orderedByToAsc = ko.computed(function() {
            return self.isOrderedByField('To') && self.orderByIsAscending();
        });
        self.orderedByKeyDesc = ko.computed(function () {
            return self.isOrderedByField('Key') && !self.orderByIsAscending();
        });
        self.orderedByFromDesc = ko.computed(function () {
            return self.isOrderedByField('From') && !self.orderByIsAscending();
        });
        self.orderedByToDesc = ko.computed(function () {
            return self.isOrderedByField('To') && !self.orderByIsAscending();
        });

        self.filterTranslation = (function (translationValue, filterValue) {
            if (translationValue == null) {
                return false;
            }

            var filter = filterValue.toLowerCase();

            if (translationValue.toLowerCase().indexOf(filter) >= 0) {
                return true;
            } else {
                return false;
            }
        });

        self.filterField = (function(performFilter, filteredTranslations, filterFunc) {
            if (performFilter) {
                filteredTranslations = ko.utils.arrayFilter(filteredTranslations, filterFunc);
            }

            return filteredTranslations;
        });

        self.filteredTranslations = ko.observableArray([]);

        self.dataController = new DataController();

        self.init = (function () {
            self.loadConfiguration();
            self.loadLanguages();
            self.loadResourceProviders();

            self.clearAllFilters();
            self.resetOrder();
        });

        self.loadLanguages = (function () {
            self.dataController.loadLanguages(self.languages);
        });

        self.loadConfiguration = (function () {
            if (window.resourceEditorConfig == undefined) {
                alert('Error: Unable to load configuration.');
            }

            self.pageSize(window.resourceEditorConfig.pageSize);
            self.isSimpleMode(window.resourceEditorConfig.isSimpleMode);
            self.autoSaveInterval(window.resourceEditorConfig.autoSaveInterval);
            self.allLanguagesKey(window.resourceEditorConfig.allLanguagesKey);

            self.resetOrder();
        });

        self.changeMode = (function () {
            var isSimple = self.isSimpleMode();

            self.isSimpleMode(!isSimple);

            /*if (isSimple) {
                self.exportFilteredTranslationsIsVisible(self.isExportFilteredVisible());
            }*/
        });

        self.resetOrder = (function() {
            self.orderBy(window.resourceEditorConfig.defaultOrderBy);
            self.orderByIsAscending(window.resourceEditorConfig.defaultOrderByIsAscending);
        });

        self.loadResourceProviders = (function () {
            self.dataController.loadResourceProviders(self.resourceProviders);
        });

        self.loadTranslations = (function () {

            if (self.confirmLoad()) {

                self.resetPaging();

                self.translations.removeAll();

                self.dataController.LoadTranslations(self);

                self.autosaveTranslations();
            }
        });

        self.confirmLoad = (function() {
            var notSavedTranslations = self.getNotSavedTranslations();

            var notSavedTranslationsCount = notSavedTranslations.length;

            if (notSavedTranslationsCount > 0) {
                var message = 'WARNING! There are ' + notSavedTranslationsCount + ' translation(s) that are not saved yet. Press OK to load translations, or Cancel to let us keep trying to save these items.';

                var result = confirm(message);

                return result;
            }

            return true;
        });

        self.autosaveTranslations = (function () {
            var timeoutId = self.autoSaveTimeoutId();

            if (timeoutId != undefined) {
                window.clearInterval(timeoutId);
            }

            self.autosave();

            timeoutId = window.setTimeout(self.autosaveTranslations, self.autoSaveInterval());

            self.autoSaveTimeoutId(timeoutId);
        });

        self.autosave = (function () {

            var notSavedTranslations = self.getNotSavedTranslations();

            for (var i = 0; i < notSavedTranslations.length; i++) {
                notSavedTranslations[i].save();
            }
        });

        self.getNotSavedTranslations = (function() {
            var notSavedTranslations = self.translations();

            notSavedTranslations = self.filterField(true, notSavedTranslations, function (translation) {
                var errorMessage = translation.errorMessage();

                return !stringIsEmpty(errorMessage);
            });

            return notSavedTranslations;
        });

        self.loadMoreTranslations = (function () {
            var pageNumber = self.pageNumber() + 1;
            self.pageNumber(pageNumber);

            self.dataController.LoadTranslations(self);
        });

        self.clearAllFilters = (function () {
            self.resetPaging();
            self.resetOrder();

            self.clearSearchTerm();
            self.hideTranslated(false);

            self.clearClientFilters();
        });

        self.resetFilters = (function () {
            self.clearAllFilters();

            self.loadTranslations();
        });

        self.clearClientFilters = (function() {
            self.clearFilterByKey();
            self.clearFilterByFrom();
            self.clearFilterByTo();
        });

        self.clearSearchTerm = (function() {
            self.searchTerm('');
        });

        self.resetPaging = (function () {
            self.numberOfTranslationsToBeShown(0);
            self.pageNumber(1);
        });

        self.clearFilterByKey = (function() {
            self.keyFilter('');
        });
        self.clearFilterByFrom = (function () {
            self.fromFilter('');
        });
        self.clearFilterByTo = (function () {
            self.toFilter('');
        });


        self.sortByKey = (function () {
            self.sort('Key');
        });
        self.sortByFrom = (function () {
            self.sort('From');
        });
        self.sortByTo = (function () {
            self.sort('To');
        });
        self.sortByStatus = (function () {
            alert('Not implemented yet');
        });

        self.sort = (function(field) {
            var currentOrderBy = self.orderBy();

            if (currentOrderBy == field) {
                var currentIsAscending = self.orderByIsAscending();

                self.orderByIsAscending(!currentIsAscending);
            } else {
                self.orderBy(field);
            }
        });

        self.exportFilteredTranslations = (function() {
            var url = "/GetTranslationsForExport?toLangKey=" + self.toLangKey();

            var requestData = [];

            $(self.translationsToShow()).each(function (index, item) {
                if (item.canBeExported()) {
                    var key = item.key();
                    requestData.push(key);
                }
            });

            self.dataController.getDownloadLinks(url, requestData, self.filesToDownload, function (item) {

                var downloadMessage = new DownloadMessage(item);

                return downloadMessage;
            },
            self.downloadManager
            );
        });

        self.downloadManager = (function () {

            $(self.filesToDownload()).each(function (index, item) {
                
                if (stringIsEmpty(item.errorMessage())  && !stringIsEmpty(item.downloadUrl())) {
                    var ifr = $('<iframe/>', {
                        src: item.downloadUrl(),
                        id : 'iframe_' + guid(),
                        style: 'height:0;width:0;display:none',
                        ready: function () {

                            self.statusMessage(item.statusMessage());

                            return true;
                        }
                    });

                    ifr.attr('data-downloadUrl', item.downloadUrl());

                    $('body').append(ifr);
                } else {
                    self.errorMessage(item.errorMessage());
                }
            });
        });

        self.exportAllTranslations = (function () {
            var url = "/GetTranslationsForExport?toLangKey=" + self.allLanguagesKey();
            
            self.dataController.getDownloadLinks(url, null, self.filesToDownload, function(item) {
                var downloadMessage = new DownloadMessage(item);

                return downloadMessage;
            }, self.downloadManager);

        });

        self.init();
    }

})(jQuery);