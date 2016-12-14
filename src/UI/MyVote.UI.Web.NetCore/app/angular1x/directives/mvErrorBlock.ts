/// <reference path="../_refs.ts" />
module MyVote.Directives {
    "use strict";

    MyVote.App.directive("mvErrorBlock", () => {

        return {
            replace: false,
            restrict: "E",
            scope: false,
            template: (element, attrs) => {

                const selectTemplate = `<div class="form-error-block">
                                            <span class="form-error-message"
                                                    ng-show="invalidInput('${attrs.name}')" 
                                                    ng-bind="validationMessage('${attrs.name}')">
                                            </span>
                                        </div>`;

                return selectTemplate;

            }

        }

    });

}