(function () {
    "use strict";

    //废弃声明 (v1.5)
    //v1.5 中$http 的 success 和 error 方法已废弃。使用 then 方法替代。
    //NBUSE: AngularJS v1.2.8

    var resourceTypeHelper = zqnb.createResourceTypeHelper();
    var guessResourceType = resourceTypeHelper.guessResourceType;
    
    angular.module("courseUploadApp", ['blueimp.fileupload'])
        .directive("courseUpload", function () {
            return {
                restrict: 'E',
                replace: false,
                templateUrl: '/Pages/UploadCourse/Demo?r=' + Math.random(),
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
                    $scope.subjects = $scope.subjectList;
                    $scope.grades = $scope.gradeList;

                    //upload method invoke step 
                    //00: $scope.$emit('uploadTempData') => set url & uploadModel.xxx
                    //01: ng-click="createSave()" => Resource.Create => 02
                    //02: $http.jsonp('xxx') => 03
                    //03: ResProc.ProcessingState

                    $scope.createSave = function () {
                        resourceService.Create($scope.uploadModel)
                            .success(function (data) {
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
                        })
                            .error(function (data) {
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
                        $scope.uploadModel.ResourceType = guessResourceType(data.filename);
                    });
                    $scope.$on('progress', function (event, data) {
                        $scope.pro = parseInt(data.overallProgress.loaded * 100 / data.overallProgress.total); //上传进度，data.overallProgress.total为文件大小值，单位B
                    });
                }
            };
        });
}());