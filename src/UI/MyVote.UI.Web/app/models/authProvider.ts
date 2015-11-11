/// <reference path="../_refs.ts" />

module MyVote.Models {
    'use strict';

    export class AuthProvider {
        constructor(public id: string, public name: string, public image: string, public color: string) {
        }
    }
}