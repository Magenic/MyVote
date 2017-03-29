/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';

    //Interface extends ng.IScope from angular.d.ts TypeScript definition file
    //This allows referencing properties on the Angular this object without the TypeScript compiler having issues with undefined properties
    export interface IPollsScope {

        vm: PollsCtrl;

        responseText(count: number): string;
        addPoll(): void;
        viewPoll(id: number): void;
        getBackImg(imageLink: string): string;
    }

    export class PollsCtrl implements IPollsScope {

        //Prefer class instance variables as opposed to polluting scope directly with variables
        message: string;
        filters: Models.PollFilter[];
        filterBy: Models.PollFilter;
        pollGroups: MyVote.Services.AppServer.Models.PollSummary[][];

        vm: PollsCtrl;

        static $inject = ['$scope', '$location', 'myVoteService'];

        constructor($scope: angular.IScope, public $location: ng.ILocationService, myVoteService: Services.MyVoteService) {

            this.vm = this;

            this.vm.filters = Models.PollFilters;
            this.vm.filterBy = this.vm.filters[0];

            $scope.$watch('filterBy', () => {
                this.vm.pollGroups = null;
                this.vm.message = 'Loading polls...';
                myVoteService.getPolls(this.vm.filterBy.value).then((result: MyVote.Services.AppServer.Models.PollSummary[][]):any => {
                    this.vm.message = result ? null : "There are no polls!";
                    this.vm.pollGroups = result;
                });
            });

        }

        public responseText = (count: number) => {
            return count === 1 ? "Response" : "Responses";
        };

        public addPoll = () => {
            this.vm.$location.path('/addPoll');
        };

        public viewPoll = (id) => {
            this.vm.$location.path('/viewPoll/' + id);
        };

        public getBackImg = (imageLink: string) => {
            return imageLink && imageLink !== '0'
                ? imageLink
                : '/app/shared/content/checkmark.svg';
        };
    }
}