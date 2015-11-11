/// <reference path="../_refs.ts" />

module MyVote.Models {
    'use strict';

    export class PollFilter {
        constructor(public name: string, public value: string) {
        }
    }
    
    export var PollFilters: PollFilter[] = [
        new PollFilter('Newest', 'Newest'),
        new PollFilter('Most Popular', 'MostPopular')
    ];
}