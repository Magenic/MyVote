/// <reference path="../_refs.ts" />
module MyVote.Directives {
    'use strict';

    MyVote.App.directive('file', ['$parse', ($parse) => {
        return {
            restrict: 'E',
            template: '<input type="file" />',
            replace: true,
            link: (scope: ng.IScope, element: any, attr: any) => {
                var modelGet = $parse(attr.fileModel);
                var modelSet = modelGet.assign;
                
                var listener = () => {
                    return scope.$apply(() => modelSet(scope, element[0].files[0]));
                };
                element.bind('change', listener);
            }
        };
    }]);
}