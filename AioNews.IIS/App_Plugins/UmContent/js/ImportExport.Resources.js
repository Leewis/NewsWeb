'use strict';

angular.module('umbraco.resources').factory('ImportExportResources', function ($q, $http, umbRequestHelper) {
    return {
        ExportContent: function (nodeIds) {
            return umbRequestHelper.resourcePromise(
                $http.post("BackOffice/ImportExportTools/ImportExport/Export", { nodeIds: nodeIds })
            );
        },
        ImportContent: function (filedata, destinationSelected) {
            return umbRequestHelper.resourcePromise(
                $http.post("BackOffice/ImportExportTools/ImportExport/Import", { content: filedata, destinationSelected: destinationSelected })
            );
        },
        BeforeImport: function (filedata, destinationSelected, nodeKey, nodeId) {
            return umbRequestHelper.resourcePromise(
                $http.post("BackOffice/ImportExportTools/ImportExport/BeforeImport", {
                    content: filedata,
                    destinationSelected: destinationSelected,
                    DestinationNodeKey: nodeKey,
                    DestinationNodeId: nodeId
                })
            );
        },
        TransformToProp: function (obj) {
            var r = {};
            $.each(obj, function () {
                r[this.Name] = this.Value;
            });
            return r;
        }
    }
});