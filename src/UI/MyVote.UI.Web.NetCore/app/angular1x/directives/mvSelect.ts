﻿/// <reference path="../_refs.ts" />
module MyVote.Directives {
    "use strict";

    MyVote.App.directive("mvSelect", () => {

        return {
            replace: false,
            restrict: "E",
            scope: false,
            template: (element, attrs) => {
                var selectTemplate = `<label for=${attrs.name}>${attrs.description}`;

                if (attrs.required) {
                    selectTemplate += '&nbsp;<span class="form-required" >&bull; </span>';
                }

                selectTemplate += `</label><select id=${attrs.name} name=${attrs.name} ng-model=${attrs.ngModel} ng-options='${attrs.optionsexpr}'` + ((attrs.required) ? " required=true " : "") + ((attrs.autoFocus) ? " autoFocus=true " : "") + `></select>`;

                selectTemplate += `<div class="form-error-block">
                                    <span class="form-error-message"
                                          ng-show="invalidInput('${attrs.name}')" 
                                          ng-bind="validationMessage('${attrs.name}')">
                                    </span></div>`;

                return selectTemplate;

            }

        }

    });

}