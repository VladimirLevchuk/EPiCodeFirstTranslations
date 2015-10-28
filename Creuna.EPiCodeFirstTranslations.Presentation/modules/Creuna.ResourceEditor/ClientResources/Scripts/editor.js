/// <reference path="Libs/jquery.js"/>
/// <reference path="Libs/knockout.js"/>
/// <reference path="Libs/jquery.iframe-transport.js"/>
/// <reference path="Libs/FileSaver.js"/>

(function($) {

    var ApiUrlBase = "/modules/Creuna.ResourceEditor/api/";

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

    function DataController() {
        var self = this;

        var loadData = (function(url, onDone, onFail) {
            $.ajax({
                url: ApiUrlBase + url,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                success: onDone,
                error: onFail
            });
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

        var loadItems = (function(loadItemsUrl, targetArray, dataToItemMappingFunc, loadFinishedCallback, onFail) {
            loadData(loadItemsUrl, function(data) {
                    if (targetArray != null) {
                        var items = $.map(data, dataToItemMappingFunc);

                        targetArray.removeAll();
                        $(items).each(function(index, item) {
                            targetArray.push(item);
                        });
                    }

                    if (loadFinishedCallback != null) {
                        loadFinishedCallback(data);
                    }
                },
                onFail);
        });

        var saveItems = (function(saveItemsUrl, targetArray, itemToDataMapingFunc, saveSucceededFunc, saveFailedFunc) {

            var dataToSave = $.map(targetArray, itemToDataMapingFunc);

            saveData(saveItemsUrl, dataToSave, saveSucceededFunc, saveFailedFunc);
        });

        var saveItem = function(saveItemsUrl, item, dataToItemMappingFunc, saveSucceededFunc, saveFailedFunc) {
            var items = [item];
            saveItems(saveItemsUrl, items, dataToItemMappingFunc, saveSucceededFunc, saveFailedFunc);
        };

        self.loadLanguages = function(targetArray, onFail) {
            loadItems("languages", targetArray, function(dataItem) { return new Language(dataItem); }, onFail);
        };

        self.loadResourceProviders = (function(targetArray, onFail) {
            loadItems("resourceProviders", targetArray, function(dataItem) { return new ResourceProvider(dataItem); }, onFail);
        });

        self.loadSettings = function(callbackFunction, onFail) {
            loadItems("settings", null, null, callbackFunction, onFail);
        }

        self.saveSettings = function(settings, onDone, onFail) {
            saveData("saveSettings", settings, onDone, onFail);
        }

        self.loadResourceMessages = function(callbackFunction, onFail) {
            loadItems("resourceMessages", null, null, callbackFunction, onFail);
        }

        self.getDownloadLinks = (function(url, toLangKey, filteredKeys, targetArray, mappingFunc, callbackFunc, onFail) {

            var options =
            {
                url: ApiUrlBase + url,
                type: "POST",
                success: callbackFunc,
                error: onFail
            };

            options.data = {};

            if (filteredKeys != null) {
                options.data.FilteredKeys = JSON.stringify(filteredKeys);
            }

            if (!(stringIsEmpty(toLangKey))) {
                options.data.ToLangKey = toLangKey;
            }

            $.ajax(options);
        });

        self.loadTranslations = (function(url, callbackFunction, onFail) {
            loadData(url, callbackFunction, onFail);
        });

        self.saveTranslation = (function(translation, mappingFunc, saveSucceededFunc, saveFaledFunc) {
            saveItem("SaveTranslations", translation, mappingFunc, saveSucceededFunc, saveFaledFunc);
        });

        self.getImportTranslationsUploadUrl = (function() {
            return ApiUrlBase + "ImportTranslations";
        });
    }

    function Language(data) {
        this.key = ko.observable(data.Key);
        this.name = ko.observable(data.Name);
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
        self.fromTranslated = ko.observable(data.FromTranslated);
        self.fromCodeFirst = ko.observable(data.FromCodeFirst);
        self.isOverridden = ko.observable(data.IsOverridden);
        self.disableSave = ko.observable(false);

        if (data.To == null || data.To == undefined) {
            data.To = '';
        }

        self.to = ko.observable(data.To);
        self.message = ko.observable('');
        self.isError = ko.observable(false);

        self.statusMessageVisible = ko.computed(function() {
            var message = this.message();

            return !stringIsEmpty(message) && !self.isError();
        }, this);

        self.errorMessageVisible = ko.computed(function() {
            var message = self.message();

            return !stringIsEmpty(message) && self.isError();
        }, this);

        this.updateData = (function(data) {
            data.Key = self.key();
            data.ProviderKey = self.providerKey();
            data.FromLangKey = self.fromLangKey();
            data.ToLangKey = self.toLangKey();
            data.From = self.from();
            data.To = self.to();
            data.IsOverridden = self.isOverridden();
        });

        self.to.subscribe(function() {
            if (self.disableSave() == false) {
                self.save();
            }
        });
    }

    function EditorViewModel() {

        var self = this;
        self.dataController = new DataController();

        self.languages = ko.observableArray([]);
        self.resourceProviders = ko.observableArray([]);
        self.translations = ko.observableArray([]);

        self.pageSize = ko.observable(0);

        self.fromLangKey = ko.observable();
        self.toLangKey = ko.observable();
        self.resourceProviderKey = ko.observable();

        self.defaultProviderKey = ko.observable();

        self.totalTranslationsCount = ko.observable();
        self.loadingTranslations = ko.observable(false);

        self.autoSaveTimeoutId = ko.observable();
        self.autoSaveInterval = ko.observable();

        self.searchTerm = ko.observable();
        self.hideTranslated = ko.observable(false);
        self.showOverriddenOnly = ko.observable(false);
        self.pageNumber = ko.observable();
        self.allLanguagesKey = ko.observable('');
        self.isSimpleMode = ko.observable(true);

        self.filesToDownload = ko.observableArray();

        self.orderBy = ko.observable('');
        self.orderByIsAscending = ko.observable(false);

        self.isAdvancedMode = ko.computed(function() {
            return !self.isSimpleMode();
        });

        self.resourceProviderText = ko.computed(function() {
            var result = $("#resurceProviderSelect option[value='" + self.resourceProviderKey() + "']").text();

            return result;
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

        self.importTranslationsUploadUrl = ko.observable(self.dataController.getImportTranslationsUploadUrl());

        self.switchLangs = (function() {
            var tempLangKey = self.fromLangKey();
            self.fromLangKey(self.toLangKey());
            self.toLangKey(tempLangKey);
        });

        self.hideTranslated.subscribe(function(value) {
            if (value === true) {
                self.showOverriddenOnly(false);
            }
        });

        self.showOverriddenOnly.subscribe(function(value) {
            if (value === true) {
                self.hideTranslated(false);
            }
        });

        self.importTranslations = (function() {
            var form = $('#imprtTranslationsForm');

            $.ajax(self.importTranslationsUploadUrl(), {
                dataType: 'json',
                contentType: "application/json",
                files: form.find(':file'),
                iframe: true,
                processData: false,
                type: 'Post'
            }).done(function(data) {
                self.handleMessage(data);
                if (!data.IsError) {
                    $('#importFile').val('');
                }
            });
        });

        self.checkFilter = (function(filterValue) {
            if (stringIsEmpty(filterValue)) {
                return false;
            } else {
                return true;
            }
        });

        self.keyFilterApplied = ko.computed(function() {
            var filter = self.keyFilter();

            return self.checkFilter(filter);
        });

        self.fromFilterApplied = ko.computed(function() {
            var filter = self.fromFilter();

            return self.checkFilter(filter);
        });

        self.toFilterApplied = ko.computed(function() {
            var filter = self.toFilter();

            return self.checkFilter(filter);
        });

        self.loadedTranslationsCount = ko.computed(function() {
            return this.translations().length;
        }, this);

        self.loadMoreButtonVisible = ko.computed(function() {
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

        self.appliedFilterItemsCount = ko.computed(function() {
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

            filteredTranslations = filteredTranslations.sort(function(left, right) {

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
            });

            return filteredTranslations;
        });

        self.settingsAreLoaded = ko.observable(false);

        self.pageSize.subscribe(function() {
            if (self.settingsAreLoaded()) {
                self.saveSettings();
            }
        });

        self.isExportFilteredVisible = function() {
            var visibleTranslations = self.translationsToShow();

            if (visibleTranslations.length > 0) {
                var translationsThatCanBeExported = [];

                $(visibleTranslations).each(function(index, item) {
                    if (item.isOverridden() == true) {
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
            var leftFieldLower = leftField.toLowerCase();
            var rightFieldLower = rightField.toLowerCase();

            return leftFieldLower == rightFieldLower ? 0 : (leftFieldLower < rightFieldLower ? - 1 : 1);
        };

        self.sortByFieldDesc = function(leftField, rightField) {
            var leftFieldLower = leftField.toLowerCase();
            var rightFieldLower = rightField.toLowerCase();

            return leftFieldLower == rightFieldLower ? 0 : (leftFieldLower > rightFieldLower ? -1 : 1);
        };

        self.isOrderedByField = function(field) {
            var currentOrderBy = self.orderBy();

            return currentOrderBy == field;
        };

        self.orderedByKeyAsc = ko.computed(function() {
            return self.isOrderedByField('Key') && self.orderByIsAscending();
        });
        self.orderedByFromAsc = ko.computed(function() {
            return self.isOrderedByField('From') && self.orderByIsAscending();
        });
        self.orderedByToAsc = ko.computed(function() {
            return self.isOrderedByField('To') && self.orderByIsAscending();
        });
        self.orderedByKeyDesc = ko.computed(function() {
            return self.isOrderedByField('Key') && !self.orderByIsAscending();
        });
        self.orderedByFromDesc = ko.computed(function() {
            return self.isOrderedByField('From') && !self.orderByIsAscending();
        });
        self.orderedByToDesc = ko.computed(function() {
            return self.isOrderedByField('To') && !self.orderByIsAscending();
        });

        self.filterTranslation = (function(translationValue, filterValue) {
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

        self.init = (function() {
            self.loadResourceMessages();

            self.loadLanguages();
            self.loadResourceProviders();

            if (window.resourceEditorConfig == undefined) {
                alert(self.resourceMessages.UnableToLoadConfiguration);
            }

            self.loadSettings();

            self.clearAllFilters();
            self.resetOrder();
        });

        self.loadLanguages = (function() {
            self.dataController.loadLanguages(self.languages);
        });

        self.loadResourceMessages = (function() {
            self.dataController.loadResourceMessages(function(data) {
                self.resourceMessages = data.Messages;
            });
        });

        self.loadSettings = (function() {
            self.dataController.loadSettings(function(loadedSettings) {
                self.pageSize(loadedSettings.PageSize);
                self.isSimpleMode(loadedSettings.IsSimpleMode);

                self.fromLangKey(loadedSettings.DefaultFrom);
                self.toLangKey(loadedSettings.DefaultTo);

                self.resourceProviderKey(loadedSettings.DefaultResourceProvider);
                self.defaultProviderKey(loadedSettings.DefaultResourceProvider);

                self.settingsAreLoaded(true);
            }, self.handleServerError);

            self.autoSaveInterval(window.resourceEditorConfig.autoSaveInterval);
            self.allLanguagesKey(window.resourceEditorConfig.allLanguagesKey);
        });

        self.saveSettings = (function() {

            var settings = {
                pageSize: self.pageSize()
            };

            self.dataController.saveSettings(settings, function(data) {
                    self.handleMessage(data);
            },
            self.handleServerError);
        });

        self.switchMode = (function() {
            var isSimple = self.isSimpleMode();

            var advancedPanel = $('.advanced-panel');
            var resultsBlock = $('.results');

            if (isSimple) {

                resultsBlock.animate({ width: "79%" }, 500);
                advancedPanel.animate({ width: "21%" }, 500);
            }
            else {
                advancedPanel.animate({ width: "0" }, 500);
                resultsBlock.animate({ width: "100%" }, 500);
            }

            self.isSimpleMode(!isSimple);
        });

        self.handleMessage = (function(message) {
            if (message.IsError) {
                self.errorMessage(message.Message);
                self.statusMessage('');
            } else {
                self.errorMessage('');
                self.statusMessage(message.Message);
            }
        });

        self.resetOrder = (function() {
            self.orderBy(window.resourceEditorConfig.defaultOrderBy);
            self.orderByIsAscending(window.resourceEditorConfig.defaultOrderByIsAscending);
        });

        self.loadResourceProviders = (function() {
            self.dataController.loadResourceProviders(self.resourceProviders);
        });

        self.loadTranslations = (function() {

            if (self.confirmLoad()) {

                self.resetPaging();

                self.translations.removeAll();

                self.performTranslationsLoad();

                self.autosaveTranslations();
            }
        });

        self.handleTranslationMessage = (function(dataItem) {
            $(self.translations()).each(function(idx, targetItem) {
                if (targetItem.key() == dataItem.Key) {
                    targetItem.isError(dataItem.IsError);
                    targetItem.message(dataItem.Message);
                    targetItem.isOverridden(dataItem.IsOverridden);
                    targetItem.disableSave(true);
                    targetItem.to(dataItem.To);
                    targetItem.disableSave(false);
                }
            });

            self.handleMessage(dataItem);
        });

        self.handleServerError = (function (jqXHR, textStatus, errorThrown) {
            var message = '';

            if (jqXHR != null && !stringIsEmpty(jqXHR.responseText)) {
                if (/^[\],:{}\s]*$/.test(jqXHR.responseText.replace(/\\["\\\/bfnrtu]/g, '@').
                    replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
                    replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                    var exception = JSON.parse(jqXHR.responseText);

                    if (exception != null) {
                        message = exception.Message;
                    }
                }
            }

            if (stringIsEmpty(message)) {
                message = errorThrown;
            }

            if (stringIsEmpty(message)) {
                message = self.resourceMessages.ServerIsUnavailable;
            }

            self.statusMessage('');
            self.errorMessage(message);
        });

        self.performTranslationsLoad = (function() {
            self.loadingTranslations(true);

            var requestUrl = "translations?providerKey=" + self.resourceProviderKey() + "&fromLangKey=" + self.fromLangKey() + "&toLangKey=" + self.toLangKey() + "&page=" + self.pageNumber() + "&pageSize=" + self.pageSize() + "&searchTerm=" + self.searchTerm() + "&hideTranslated=" + self.hideTranslated() + "&overriddenOnly=" + self.showOverriddenOnly() + "&orderBy=" + self.orderBy() + "&orderByIsAscending=" + self.orderByIsAscending();

            self.dataController.loadTranslations(requestUrl, function (data) {

                    var items = $.map(data.Items, function(dataItem) {

                        var translation = new Translation(dataItem);

                        translation.save = function() {
                            translation.message(self.resourceMessages.SavingTranslation);

                            self.dataController.saveTranslation(translation,
                                function(translationItem) { // mappingFunc
                                    var translationData = {};
                                    translationItem.updateData(translationData);
                                    return translationData;
                                },
                                function(x) { // saveSucceededFunc
                                    $(x).each(function(index, item) {
                                        self.handleTranslationMessage(item);
                                    });
                                },
                                function(jqXHR, textStatus, errorThrown, translationItems) {
                                    for (var i = 0; i < translationItems.length; i++) {
                                        translationItems[i].Message = errorThrown;
                                        translationItems[i].IsError = true;
                                        self.handleTranslationMessage(translationItems[i]);
                                    }
                                    self.handleServerError(jqXHR, textStatus, errorThrown);
                                }); // saveFailedFunc
                        };

                        return translation;
                    });

                    var numberOfItemsToBeShown = data.Items.length + self.numberOfTranslationsToBeShown();
                    self.numberOfTranslationsToBeShown(numberOfItemsToBeShown);

                    $(items).each(function(index, item) {
                        self.translations.push(item);
                    });

                    self.totalTranslationsCount(data.Paging.TotalItemsCount);

                    self.pageNumber(data.Paging.PageIndex);

                    self.loadingTranslations(false);
                    self.handleMessage(data);
                },
                function(jqXHR, textStatus, errorThrown) {
                    self.loadingTranslations(false);
                    self.handleServerError(jqXHR, textStatus, errorThrown);
                });
        });

        self.confirmLoad = (function() {
            var notSavedTranslations = self.getNotSavedTranslations();

            var notSavedTranslationsCount = notSavedTranslations.length;

            if (notSavedTranslationsCount > 0) {
                var message = self.resourceMessages.AutoSaveWarning.replace("{0}", notSavedTranslationsCount);

                var result = confirm(message);

                return result;
            }

            return true;
        });

        self.autosaveTranslations = (function() {
            var timeoutId = self.autoSaveTimeoutId();

            if (timeoutId != undefined) {
                window.clearInterval(timeoutId);
            }

            self.autosave();

            timeoutId = window.setTimeout(self.autosaveTranslations, self.autoSaveInterval());

            self.autoSaveTimeoutId(timeoutId);
        });

        self.autosave = (function() {
            var notSavedTranslations = self.getNotSavedTranslations();

            for (var i = 0; i < notSavedTranslations.length; i++) {
                notSavedTranslations[i].save();
            }
        });

        self.getNotSavedTranslations = (function() {
            var notSavedTranslations = self.translations();

            notSavedTranslations = self.filterField(true, notSavedTranslations, function(translation) {
                return translation.isError();
            });

            return notSavedTranslations;
        });

        self.loadMoreTranslations = (function() {
            var pageNumber = self.pageNumber() + 1;
            self.pageNumber(pageNumber);

            self.performTranslationsLoad();
        });

        self.clearAllFilters = (function() {
            self.resetPaging();
            self.resetOrder();

            self.clearSearchTerm();
            self.hideTranslated(false);
            self.showOverriddenOnly(false);

            self.clearClientFilters();
        });

        self.resetFilters = (function() {
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

        self.resetPaging = (function() {
            self.numberOfTranslationsToBeShown(0);
            self.pageNumber(1);
        });

        self.clearFilterByKey = (function() {
            self.keyFilter('');
        });
        self.clearFilterByFrom = (function() {
            self.fromFilter('');
        });
        self.clearFilterByTo = (function() {
            self.toFilter('');
        });


        self.sortByKey = (function() {
            self.sort('Key');
        });
        self.sortByFrom = (function() {
            self.sort('From');
        });
        self.sortByTo = (function() {
            self.sort('To');
        });
        self.sortByStatus = (function() {
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
            var requestData = [];

            $(self.translationsToShow()).each(function(index, item) {
                if (item.isOverridden()) {
                    var key = item.key();
                    requestData.push(key);
                }
            });

            self.dataController.getDownloadLinks('getTranslationsForExport', self.toLangKey(), requestData, self.filesToDownload, function(item) {
                    debugger;
                    var downloadMessage = new DownloadMessage(item);

                    return downloadMessage;
                },
                self.downloadManager,
                self.handleServerError
            );
        });

        self.exportAllTranslations = (function() {
            self.dataController.getDownloadLinks('getTranslationsForExport', self.allLanguagesKey(), null, self.filesToDownload, function (item) {
                debugger;
                var downloadMessage = new DownloadMessage(item);

                return downloadMessage;

            }, self.downloadManager,
            self.handleServerError);

        });

        self.downloadManager = (function (downloadContent) {
            var blob = null;

            if (downloadContent.ContentType == "application/zip") {

                var sliceSize = 512;

                var byteCharacters = atob(downloadContent.Content);
                var byteArrays = [];

                for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                    var slice = byteCharacters.slice(offset, offset + sliceSize);

                    var byteNumbers = new Array(slice.length);
                    for (var i = 0; i < slice.length; i++) {
                        byteNumbers[i] = slice.charCodeAt(i);
                    }

                    var byteArray = new Uint8Array(byteNumbers);

                    byteArrays.push(byteArray);
                }

                blob = new Blob(byteArrays, { type: downloadContent.Content });
            } else {
                blob = new Blob([downloadContent.Content], { type: downloadContent.ContentType });
            }

            saveAs(blob, downloadContent.FileName);
        });

        self.leaveFocusOnEnter = (function(viewModel, event) {
            var keyCode = event.which || event.keyCode;

            if (keyCode == 13) {
                event.target.blur();
                return false;
            }

            return true;
        });

        self.init();
    }

    ko.applyBindings(new EditorViewModel());

})(jQuery);