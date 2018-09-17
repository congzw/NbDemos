# Markdown File

字典定制：

    var dicHelper = zqnb.createDicHelper();

    var mainApp = zqnb.mainApp;
    mainApp.service('nbDicCatalogMeta', function () {
        //console.log('service => nbDicCatalogMeta');
        //customize meta => if necessary!
        var meta = dicHelper.createCatalogMeta();
        console.log('customize meta => if necessary!');
        meta.categories[4].disabled = true; //隐藏年级的示例
        console.log(meta);
        return meta;
    });