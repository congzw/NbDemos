(function () {
    "use strict";
    angular.module("UploadModule", ['blueimp.fileupload'])
        .directive("mediaUpload", function () {
            return {
                restrict: 'E',
                replace: false,
                templateUrl: '/Areas/Widget/MediaUpload/Template?r=' + Math.random(),
                scope: {
                    siteId: '@',
                    loginName: '@',
                    userName: '@',
                    subjectList: '=', //变量传递
                    gradeList: '='
                },
                controller: function ($scope, $http, dicService, serviceFactory, makeDialog) {
                    $scope.userLoginName = $scope.loginName;
                    $scope.filePath = '';
                    $scope.showTag = true;
                    $scope.uploadModel = {
                        SubjectName: '',
                        GradeName: '',
                        PhaseName: '', // 新增属性 实现赋值 
                        FileName: '',
                        AppRevicerDefine: $scope.loginName,
                        AppName: 'Space',
                        Title: '',
                        Id: '',
                        ResourceType: '',
                        Size: '',
                        Description: '',
                        Teacher: $scope.userName,
                        Ex1: '', //增加扩展预留
                        Ex2: '',
                        Ex3: ''
                    };

                    var resourceService = serviceFactory.createService("Resource");
                    var resProcService = serviceFactory.createService("ResProc");
                    resourceService.addFunction("Create", "post");
                    resProcService.addFunction("ProcessingState", "post");
                    //var ss = eval("(" + $scope.subjectList + ")"); //字符串变为js变量
                    $scope.subjects = $scope.subjectList;
                    $scope.grades = $scope.gradeList;

                    //获取学科字典(启用)
                    //dicService.getSiteDicItem($scope.siteId, "subject", true).success(function (data) {
                    //    $scope.subjects = data;
                    //});

                    //获取年级字典(启用)
                    //dicService.getSiteDicItem($scope.siteId, "grade", true).success(function (data) {
                    //    $scope.grades = data;
                    //});

                    //上传错误提示
                    //$scope.ftpHelp = "资源无法上传，请查看上传帮助";

                    //根据文件路径文件全名判断资源类型 ：1---视频类型 ；2---文档类型
                    $scope.CheckType = function (filename) {
                        var type = filename.substring(filename.lastIndexOf(".") + 1);
                        var resourceTypeList = [
                            {
                                "resourceTypeControl": ["wmv", "avi", "dat", "asf", "rm", "rmvb", "ram", "mpg", "mpeg", "3gp", "mov", "mp4", "m4v", "dvix", "dv", "dat", "mkv", "flv", "vob", "qt", "ram", "divx", "fli", "cpk", "flc", "mod", "mts"],
                                "resourceType": 1 //vedio
                            },
                            {
                                "resourceTypeControl": ["doc", "docx", "pdf", "txt", "ppt", "pptx", "xls", "xlsx", "wps", "et", "zip", "rar"],
                                "resourceType": 2 //document
                            }
                        ];
                        for (var i = 0; i < resourceTypeList.length; i++) {
                            for (var j = 0; j < resourceTypeList[i].resourceTypeControl.length; j++) {
                                if (type.toLowerCase() == resourceTypeList[i].resourceTypeControl[j].toLowerCase()) {
                                    return i + 1;
                                }
                            }
                        }
                    };

                    $scope.createSave = function () {
                        resourceService.Create($scope.uploadModel).success(function (data) {
                            var metaValue = angular.toJson(data);
                            var token = $scope.uploadModel.Id;
                            //console.log(token);
                            //console.log(metaValue);
                            //模拟的meta数据
                            //var metaMockData = '{"Title":"title","Teacher":"teacher","Subject":null,"Grade":"grade","Description":"desc","To":"admin.space@domain","From":"xinyida","Files":[{"Name":"teacher.mp4","Format":"video/mp4"}]}';
                            $http({
                                method: 'JSONP',
                                url: $scope.serviceUrl + '/meta/set?callback=JSON_CALLBACK' + '&token=' + token + '&metaValue=' + metaValue
                            }).success(function (data1) {

                                var stateModel = {};
                                stateModel.Id = data.Files[0].Id;
                                stateModel.Title = data.Title;
                                stateModel.CurrentState = 2;
                                stateModel.StateDesc = "等待后处理中";
                                stateModel.To = data.To;
                                stateModel.NowHost = "";
                                stateModel.NowPosition = "";
                                stateModel.RelationId = $scope.uploadModel.Id;
                                resProcService.ProcessingState(stateModel).success(function (data2) {
                                    makeDialog.success("上传成功！");
                                    window.location = location;
                                });


                                //跳转到我的资源上传记录tab项
                                //window.location = "/space/" + $scope.loginName + "/resource/list?tab=2";

                            }).error(function () {
                                makeDialog.info("上传失败！");
                            });
                        }).error(function (data) {
                            makeDialog.info("连接错误");
                        });

                    };
                    $scope.$on('uploadTempData', function (event, data) {
                        $scope.uploadTempData = data;
                        $scope.serviceUrl = data.upload.ServerBaseAddr;
                        //  console.log($scope.serviceUrl);
                        $scope.uploadModel.Id = data.upload.Token;
                        $scope.uploadModel.FileName = data.filename;
                        $scope.uploadModel.Size = data.size;
                        $scope.uploadModel.ResourceType = $scope.CheckType(data.filename);
                    });
                    $scope.$on('progress', function (event, data) {
                        $scope.pro = parseInt(data.overallProgress.loaded * 100 / data.overallProgress.total); //上传进度，data.overallProgress.total为文件大小值，单位B
                    });
                }
            };
        })
        .controller("DemoFileUploadController", function ($scope, $http, $element, serviceFactory, makeDialog) {
            var dsService = serviceFactory.createService("Ds");
            dsService.addFunction("FetchHttpServer", "get");
            $scope.options = {};
            dsService.FetchHttpServer({ isAllIps: true }).success(function (data1) {
                var temp = data1.Data;

                //console.log(data1);
                if (!data1.Success) {
                    makeDialog.info(data1.Message);
                    return;
                };
                var token = temp.Token;
                var serverBaseAddr;
                if (temp.ServerBaseAddr != null) {
                    serverBaseAddr = temp.ServerBaseAddr;
                } else {
                    serverBaseAddr = getTheBestAddr(temp.ServerBaseAddrList);
                }


                $scope.overallProgress = $('#fileupload').fileupload('progress');
                $scope.$watch('overallProgress.loaded', function () {
                    $scope.$emit("progress", {
                        overallProgress: $scope.overallProgress,
                    });
                });

                $('#fileupload').fileupload({
                    change: function (e, data) {
                        var flag = true;
                        //添加文件的个数限制为一个
                        if (data.files.length > 1) {
                            makeDialog.info("一次只能添加一个资源！");
                            data.files.length = 0;
                            flag = false;
                            //data.files.splice(1, data.files.length - 1);

                        }
                        //判断文件的类型是否合法
                        if (!$scope.CheckType(data.files[0].name)) {
                            var type = data.files[0].name.substring(data.files[0].name.lastIndexOf(".") + 1);
                            makeDialog.info("不支持上传" + type + "格式的文件！");
                            //makeDialog.info("请上传限定的文件格式。");
                            //console.log("false");
                            //return false;
                            flag = false;
                        }
                        //判断文件的大小
                        if (data.files[0].size > 400 * 1024 * 1024) {
                            makeDialog.info("上传的文件过大（超过400M），请使用FTP上传。");
                            flag = false;
                        }
                        //长传文件个数限制为一个
                        if ($scope.queue.length > 0) {
                            makeDialog.info("一次只能上传一个资源！");
                            $scope.queue.splice(0, 1);

                        }
                        return flag;
                    },
                    submit: function (e, data) {
                        //附加表单的信息
                        data.formData = { token: token };
                        //console.log(token);
                        $scope.$emit("uploadTempData", {
                            upload: temp,
                            filename: $scope.queue[0].name,
                            size: $scope.queue[0].size
                        });
                    }
                });

                function getTheBestAddr(addrList) {
                    var uploadUrl = '/Upload/UploadHandler.ashx';
                    angular.forEach(addrList, function (addr) {
                        var promise = $http({
                            url: addr,
                            method: 'get'
                        });
                        promise.then(function (data) {
                            if (data.status == 200) {
                                //上传物理文件时的参数，以下为服务器地址和文件大小限制
                                $scope.options = {
                                    url: addr + uploadUrl,
                                    maxFileSize: 2 * 1024 * 1024 * 1024,
                                };
                                temp.ServerBaseAddr = addr;
                                return addr;
                            }
                        });
                    });
                }
            }).error(function (data1) {
                //console.log(data1);
                makeDialog.info("获取服务器失败,请配置好服务器再试");
                $scope.serviceError = true;
            });
        });
}());