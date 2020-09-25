'use strict';

angular.module("umbraco").controller("ImportExportController",
    function ($scope, ImportExportResources, editorService, angularHelper) {

        function setFormState(state) {
            // get the current form
            var currentForm = angularHelper.getCurrentForm($scope);
            // set state
            if (state === 'dirty') {
                currentForm.$setDirty();
            } else if (state === 'pristine') {
                currentForm.$setPristine();
            }
        }
        setFormState('pristine');

        $scope.isLoading = true;
        $scope.isValid = false;
        $scope.nodes = [];
        $scope.ProgressIndx = 0;
        $scope.DataMessage = $scope.LoadingMessage = "Wait while content is loading...";

        $scope.InitProgress = function (start, interval, step, end, callback) {
            $scope.Progress = start;

            clearInterval($scope.ProgressIndx);
            $scope.ProgressIndx = setInterval(function () {
                $scope.safeApply();
                start = start + step;
                $scope.Progress = { width: "" + start + "%" };
                if (start > end) {
                    clearInterval($scope.ProgressIndx);
                    if (callback) callback();
                };
            }, interval);
        };
        $scope.safeApply = function (fn) {
            var scope = $scope;
            var r = (scope.$$phase || scope.$root.$$phase) ? (fn ? fn() : null) : scope.$apply(fn ? fn() : null);
        };

        $scope.openMultipleContentPicker = function () {
            editorService.contentPicker({
                multiPicker: true,
                submit: function (data) {
                    $scope.nodes = data.selection;
                    editorService.close();
                },
                close: function (data) {
                    editorService.close();
                }

            });

        };

        $scope.removeItem = function (item) {
            var index = $scope.nodes.indexOf(item);
            $scope.nodes.splice(index, 1);
        };
        $scope.checkBoxSelectAll = function () {
            $scope.existingNodes.forEach(el => el.OverrideExisting = true);
        };

        $scope.checkBoxUnSelectAll = function () {
            $scope.existingNodes.forEach(el => el.OverrideExisting = false);
        };

        $scope.closeModal = function () {
            $scope.showModal = false;
            $scope.showLogModal = false;
            $('#navigation').show();
        };

        $scope.openLogModal = function () {
            $('#navigation').hide();
            $scope.showLogModal = true;
        };

        $scope.beforeImport = function (file, node) {
            if (!file) return;

            $scope.isLoading = true;
            $scope.InitProgress(0, 200, 1, 100, function () { $scope.isLoading = false; $scope.safeApply(); });

            var destinationSelected = false;
            var nodeKey = null;
            var nodeId = null;
            if (node) {
                destinationSelected = true;
                nodeKey = node.key;
                nodeId = node.id;
            }

            ImportExportResources.BeforeImport(file, destinationSelected, nodeKey, nodeId).then(function (res) {
                $scope.DataMessage = res.Message;

                $scope.showModal = true;
                $scope.existingNodes = res.data;
                console.log(res.data);
                $scope.InitProgress(0, 100, 5, 100, function () { $scope.DataMessage = $scope.LoadingMessage; $scope.isLoading = false; $scope.safeApply(); });
            });
        };

        $scope.importContent = function (file, node, insertInToRoot) {
            
            var fileData = JSON.parse(file)
            if (!fileData) return;

            $scope.existingNodes.forEach(el => {
                fileData[fileData.findIndex(fd => fd.Id == el.Id)].OverrideExisting = el.OverrideExisting;
            });

            var mainParents = fileData.filter(it => it.IsMainParent);
            var mainParantIds = mainParents.map(m => m.Id);
            var destinationSelected = false;

            if (insertInToRoot) {
                destinationSelected = true;
                mainParantIds.forEach(id => {
                    fileData[fileData.findIndex(el => el.Id = id)].parentId = -1;
                });
            }
            else
                if (node) {
                    destinationSelected = true;
                    mainParantIds.forEach(id => {
                        fileData[fileData.findIndex(el => el.Id = id)].parentId = node.key;
                        fileData[fileData.findIndex(el => el.Id = id)].parentId = node.id;
                    });
                }

            $scope.isLoading = true;
            $scope.showModal = false;
            $scope.InitProgress(0, 200, 1, 100, function () { $scope.isLoading = false; $scope.safeApply(); });
            ImportExportResources.ImportContent(JSON.stringify(fileData), destinationSelected).then(function (res) {
                $scope.DataMessage = res.Message;

                $scope.importingLogs.items = res.Log;
                console.log($scope.importingLogs.items);
                $scope.InitProgress(0, 100, 1, 100, function () { $scope.DataMessage = $scope.LoadingMessage; $scope.isLoading = false; $scope.safeApply();; });
            });
        };

        $scope.fileSelected = function (files) {
            if (!files) return;
            
            var fr = new FileReader();
            fr.onload = function (e) {
                $scope.file = e.target.result;
            }
            fr.readAsText(files);
            setFormState('pristine');
        };

        $scope.exportContent = function (nodes) {
            if (!nodes) return;

            var nodeIds = nodes.map((node) => node.id);
            $scope.isLoading = true;
            $scope.InitProgress(0, 100, 1, 100, function () { $scope.isLoading = false; $scope.safeApply(); });
            ImportExportResources.ExportContent(nodeIds).then(function (res) {
                $scope.DataMessage = res.Message;

                $scope.downloadJson(res.data);

                $scope.InitProgress(0, 200, 5, 100, function () { $scope.DataMessage = $scope.LoadingMessage; $scope.isLoading = false; $scope.safeApply();; });
            });
        };

        $scope.downloadJson = function (obj) {
            var data = "text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(obj));
            document.getElementById('downloadContainer').innerHTML = '<span style="float:right"><a href="data:' + data + '" download="' + obj[0].Name + '.json">download ' + obj[0].Name + '.json </a><span>';
        };

        $scope.file = null;
        $scope.existingNodes = null;
        $scope.importingLogs = {
            items: null,
            options: [
                { alias: "Alias", header: "Alias" },
                { alias: "Cultures", header: "Cultures" },
                { alias: "State", header: "State" }
            ]
        };
        $scope.showModal = false;
        $scope.showLogModal = false;
        $scope.isValid = true;
        $scope.isLoading = false;
    });