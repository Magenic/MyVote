/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Models) {
        'use strict';

        var PollFilter = (function () {
            function PollFilter(name, value) {
                this.name = name;
                this.value = value;
            }
            return PollFilter;
        })();
        Models.PollFilter = PollFilter;

        Models.PollFilters = [
            new PollFilter('Newest', 'Newest'),
            new PollFilter('Most Popular', 'MostPopular')
        ];
    })(MyVote.Models || (MyVote.Models = {}));
    var Models = MyVote.Models;
})(MyVote || (MyVote = {}));
