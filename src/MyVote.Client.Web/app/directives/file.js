var MyVote;
(function (MyVote) {
    /// <reference path="../_refs.ts" />
    (function (Directives) {
        'use strict';

        MyVote.App.directive('file', [
            '$parse', function ($parse) {
                return {
                    restrict: 'E',
                    template: '<input type="file" />',
                    replace: true,
                    link: function (scope, element, attr) {
                        var modelGet = $parse(attr.fileModel);
                        var modelSet = modelGet.assign;

                        var listener = function () {
                            return scope.$apply(function () {
                                return modelSet(scope, element[0].files[0]);
                            });
                        };
                        element.bind('change', listener);
                    }
                };
            }]);
    })(MyVote.Directives || (MyVote.Directives = {}));
    var Directives = MyVote.Directives;
})(MyVote || (MyVote = {}));
//# sourceMappingURL=file.js.map
