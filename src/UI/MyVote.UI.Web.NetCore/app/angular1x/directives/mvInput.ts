/// <reference path="../_refs.ts" />
module MyVote.Directives {
    "use strict";

    MyVote.App.directive("mvInput", () => {

        return {
            replace: false,
            restrict: "E",
            scope: false,
            template: (element, attrs) => {
                var selectTemplate = `<label for=${attrs.name}>${attrs.description}`;

                if (attrs.required) {
                    selectTemplate += '&nbsp;<span class="form-required" >&bull; </span>';
                }

                selectTemplate += `</label><input id=${attrs.name} name=${attrs.name} ng-model=${attrs.ngModel} type=${attrs.type}` + ((attrs.required) ? " required=true" : "") + ((attrs.autoFocus) ? " autoFocus=true" : "") + " />";

                selectTemplate += `<div class="form-error-block">
                                    <span class="form-error-message"
                                          ng-show="vm.invalidInput('${attrs.name}')" 
                                          ng-bind="vm.validationMessage('${attrs.name}')">
                                    </span></div>`;

                return selectTemplate;

            }

        }

    });

}