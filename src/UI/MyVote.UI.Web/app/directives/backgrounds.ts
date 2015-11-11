/// <reference path="../_refs.ts" />

module MyVote.Directives {
    'use strict';

    MyVote.App.directive('backImg', () => {
        return ((scope, element, attrs) => {
            attrs.$observe(
                'backImg',
                (value: string) => {
                    if (value) {
                        element.css({
                            'background-image': 'url(' + value + ')',
                            'background-size': 'cover'
                        });
                    }
                });
        });
    });

    MyVote.App.directive('backColor', () => {
        return ((scope, element, attrs) => {
            attrs.$observe(
                'backColor',
                (value: string) => {
                    if (value) {
                        element.css({ 'background-color': value });
                    }
                });
        });
    });
}