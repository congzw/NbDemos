﻿(function (_) {
    'use strict';

    var equalIgnoreCase = function (str, str2) {
        if (str === null || str2 === null) {
            return true;
        }
        if (typeof str !== 'string' || typeof str2 !== 'string') {
            return false;
        }
        if (str === '' && str2 === '') {
            return true;
        }
        return (str2.toUpperCase() === str.toUpperCase());
    },
    sameCodeItem = function (item1, item2) {
        return item1 === item2 || equalIgnoreCase(item1.Code, item2.Code);
    },
    containItem = function (items, itemToCheck) {
        if (!items || !itemToCheck) {
            return false;
        }
        for (var i = 0; i < items.length; i++) {
            if (sameCodeItem(items[i], itemToCheck)) {
                return true;
            }
        }
        return false;
    },
    findItem = function (items, code) {
        for (var i = 0; i < items.length; i++) {
            var current = items[i];
            var sameCode = equalIgnoreCase(current.Code, code);
            if (sameCode) {
                return current;
            }
        }
        return null;
    },
    createEmptyItem = function () {
        return { Code: "", Name: "全部" };
    },
    createArrayCode = function (arr) {
        if (arr.length === 0) {
            return '';
        }
        var code = arr.join(',');
        return code;
    },
    createCodeItem = function () {
        var arr = Array.from(arguments);
        var code = createArrayCode(arr);
        var codeItem = { Code: code };
        return codeItem;
    },
    createInitItems = function (items, emptyItem) {
        if (!items || items.length === 0) {
            return null;
        }
        var initItems = [];
        if (emptyItem) {
            initItems.push(emptyItem);
        }
        for (var i = 0; i < items.length; i++) {
            var current = items[i];
            var copy = { Code: current.Code, Name: current.Name };

            //hack for org
            if (current.ParentCode) {
                copy.ParentCode = current.ParentCode;
            }
            if (current.OrgTypeCode) {
                copy.OrgTypeCode = current.OrgTypeCode;
            }
            initItems.push(copy);
        }
        return initItems;
    },
    categories = function () {
        return [
            {
                key: "orgType",
                name: "组织类型"
            },
            {
                key: "org",
                name: "组织"
            },
            {
                key: "phase",
                name: "学段"
            },
            {
                key: "subject",
                name: "学科"
            },
            {
                key: "grade",
                name: "年级"
            }]
        ;
    }(),
    getCategoryKey = function (category) {
        if (category === undefined) {
            console.log('getCategoryKey typeof');
            console.log(typeof category);
            throw { name: 'bad' };
        }
        if (typeof category === "string") {
            return category;
        }
        return category.key;
    },
    getCategoryItemsKey = function (category) {
        var key = getCategoryKey(category);
        return key + 's';

    },
    getCategoryEmptyItemKey = function (category) {
        var key = getCategoryKey(category);
        return key + 'Empty';
    };

    var dicHelper = function () {
        return {
            equalIgnoreCase: equalIgnoreCase,
            sameCodeItem: sameCodeItem,
            containItem: containItem,
            findItem: findItem,
            createEmptyItem: createEmptyItem,
            createArrayCode: createArrayCode,
            createCodeItem: createCodeItem,
            createInitItems: createInitItems,
            categories: categories
        };
    }();
    _.createDicHelper = function () {
        return dicHelper;
    };

    var createDicCatalogVm = function () {

        //private methods
        var fixOrgModels = function(orgs) {
            var fixOrgs = [];
            for (var i = 0; i < orgs.length; i++) {
                var current = orgs[i];
                fixOrgs.push({ Code: current.Id, Name: current.Name, OrgTypeCode: current.OrgTypeCode, ParentCode: current.ParentId });
            }
            return fixOrgs;
        };

        //return model
        var dicCatalogVm = {
            //选择结果
            selectResult: {},
            //是否自动补齐全部
            autoAppendEmpty: true
        };

        //setup properties for categories
        for (var i = 0; i < categories.length; i++) {
            var emptyItem = createEmptyItem();
            var category = categories[i];

            var categoryKey = getCategoryKey(category);
            var categoryItemsKey = getCategoryItemsKey(category);
            var categoryEmptyItemKey = getCategoryEmptyItemKey(category);

            //items: null
            //emptyItem: createEmptyItem()
            dicCatalogVm[categoryItemsKey] = null;
            dicCatalogVm[categoryEmptyItemKey] = emptyItem;
            //selectResult.item = emptyItem;
            dicCatalogVm.selectResult[categoryKey] = emptyItem;
        }

        //[this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];
        dicCatalogVm.selectResult.display = function () {
            //console.log('display');
            //console.log(dicCatalogVm.selectResult);
            //return [this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];
            var items = [];
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                var categoryKey = getCategoryKey(category);
                items.push(dicCatalogVm.selectResult[categoryKey].Name);
            }
            return items;
        }
        dicCatalogVm.orgTypePhases = null;
        dicCatalogVm.visiableOrgTypePhases = null;

        //初始化字典项
        dicCatalogVm.initItems = function(config) {
            if (!config) {
                return;
            }
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                var categoryItemsKey = getCategoryItemsKey(category);
                var categoryEmptyItemKey = getCategoryEmptyItemKey(category);
                var items = config[categoryItemsKey];
                if (items) {
                    //hack for orgs
                    if (categoryItemsKey === "orgs") {
                        items = fixOrgModels(items);
                    }
                    var appendEmptyItem = dicCatalogVm.autoAppendEmpty ? dicCatalogVm[categoryEmptyItemKey] : null;
                    dicCatalogVm[categoryItemsKey] = createInitItems(items, appendEmptyItem);
                }
            }
        };
        //是否是空的集合（或只有【全部】按钮）
        dicCatalogVm.isEmptyItems = function (currentCategory) {
                //theVm.phases, theVm.orgs, ...
            var categoryItemsKey = getCategoryItemsKey(currentCategory);
                var currentItems = dicCatalogVm[categoryItemsKey];
                if (!currentItems) {
                    return false;
                }
                if (currentItems.length === 0 || (currentItems.length === 1 && currentItems[0].Code === '')) {
                    return false;
                }
                return true;
            };
        return dicCatalogVm;
    };
    _.createDicCatalogVm = function () {
        return createDicCatalogVm();
    };
})(zqnb || {});