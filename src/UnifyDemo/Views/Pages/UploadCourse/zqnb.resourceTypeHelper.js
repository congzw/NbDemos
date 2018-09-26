(function (_) {
    'use strict';

    var resourceTypeHelper = function () {
        var konwnResourceTypes = function () {
            //todo read from config?
            var types = [
                {
                    "resourceType": 1, //vedio
                    "fileExtensions": ["wmv", "avi", "dat", "asf", "rm", "rmvb", "ram", "mpg", "mpeg", "3gp", "mov", "mp4", "m4v", "dvix", "dv", "dat", "mkv", "flv", "vob", "qt", "ram", "divx", "fli", "cpk", "flc", "mod", "mts"]
                },
                {
                    "resourceType": 2,//document
                    "fileExtensions": ["doc", "docx", "pdf", "txt", "ppt", "pptx", "xls", "xlsx", "wps", "et", "zip", "rar"]
                }
            ];
            return types;
        }();
        //根据文件路径文件全名判断资源类型 ：1---视频类型 ；2---文档类型
        var guessResourceType = function (fileName, guessFailReturnType) {

            var type = fileName.substring(fileName.lastIndexOf(".") + 1);
            for (var i = 0; i < konwnResourceTypes.length; i++) {
                var currentType = konwnResourceTypes[i];
                for (var j = 0; j < currentType.fileExtensions.length; j++) {
                    var currentExtension = currentType.fileExtensions[j];
                    if (type.toLowerCase() === currentExtension.toLowerCase()) {
                        return currentType.resourceType;
                    }
                }
            }

            if (!guessFailReturnType) {
                return guessFailReturnType;
            }
            return konwnResourceTypes[0].resourceType;
        };

        return {
            konwnResourceTypes: konwnResourceTypes,
            guessResourceType: guessResourceType
        };
    }();

    _.createResourceTypeHelper = function () {
        return resourceTypeHelper;
    };
})(zqnb || {});