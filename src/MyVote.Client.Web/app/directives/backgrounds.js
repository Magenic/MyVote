/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Directives) {
        'use strict';

        MyVote.App.directive('backImg', function () {
            return (function (scope, element, attrs) {
                attrs.$observe('backImg', function (value) {
                    if (value) {
                        element.css({
                            'background-image': 'url(' + value + ')',
                            'background-size': 'cover'
                        });
                    }
                });
            });
        });

        MyVote.App.directive('backColor', function () {
            return (function (scope, element, attrs) {
                attrs.$observe('backColor', function (value) {
                    if (value) {
                        element.css({ 'background-color': value });
                    }
                });
            });
        });
    })(MyVote.Directives || (MyVote.Directives = {}));
    var Directives = MyVote.Directives;
})(MyVote || (MyVote = {}));
//# sourceMappingURL=backgrounds.js.map
