/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';

    export interface PollsScope extends ng.IScope {
        message: string;
        filters: Models.PollFilter[];
        filterBy: Models.PollFilter;
        pollGroups: MyVote.Services.AppServer.Models.PollSummary[][];

        responseText(count: number): string;
        addPoll(): void;
        viewPoll(id: number): void;
        getBackImg(imageLink: string): string;
    }

    export class PollsCtrl {
        static $inject = ['$scope', '$location', 'myVoteService'];

        constructor($scope: PollsScope, $location: ng.ILocationService, myVoteService: Services.MyVoteService) {
            $scope.filters = Models.PollFilters;
            $scope.filterBy = $scope.filters[0];

            $scope.$watch('filterBy', () => {
                $scope.pollGroups = null;
                $scope.message = 'Loading polls...';
                myVoteService.getPolls($scope.filterBy.value).then((result: MyVote.Services.AppServer.Models.PollSummary[][]) => {
                    $scope.message = result ? null : "There are no polls!";
                    $scope.pollGroups = result;
                });
            });

            $scope.responseText = (count: number) => {
                return count == 1 ? "Response" : "Responses";
            };

            $scope.addPoll = () => {
                $location.path('/addPoll');
            };

            $scope.viewPoll = (id) => {
                $location.path('/viewPoll/' + id);
            };

            $scope.getBackImg = (imageLink: string) => {
                return imageLink && imageLink != '0'
                    ? imageLink
                    : '/Content/checkmark.svg';
            };
        }
    }
}