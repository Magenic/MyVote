/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';

    export interface ViewPollScope extends ng.IScope {
        pollId: number;
        currentPoll: MyVote.Services.AppServer.Models.Poll;
        infoMessage: string;
        errorMessage: string;

        canSubmit(): boolean;
        canDelete(): boolean;
        submit(): void;
        delete(): void;
    }

    export interface ViewPollParams extends ng.route.IRouteParamsService {
        pollId: number;
    }

    export class ViewPollCtrl {
        static $inject = ['$scope', '$routeParams', '$location', '$log', 'myVoteService', 'authService'];

        constructor(
            $scope: ViewPollScope,
            $routeParams: ViewPollParams,
            $location: ng.ILocationService,
            $log: ng.ILogService,
            myVoteService: Services.MyVoteService,
            authService: Services.AuthService
        ) {
            $scope.infoMessage = 'Loading poll...';
            $scope.errorMessage = null;
            
            $scope.pollId = $routeParams.pollId;

            myVoteService.getPollResponse($scope.pollId, authService.userId)
                .then(response => {
                    if (response && response.SubmissionDate) {
                        $location.path('/pollResult/' + $scope.pollId).replace();
                        return null;
                    } else {
                        return myVoteService.getPoll($scope.pollId);
                    }
                }).then((poll: MyVote.Services.AppServer.Models.Poll) => {
                    $scope.infoMessage = null;
                    $scope.currentPoll = poll;
                },
                error => {
                    $scope.infoMessage = null;
                    ViewPollCtrl.showError($scope, error, 'Could not load poll.');
                    $log.error(error);
                });

            $scope.canSubmit = () => {
                if (!$scope.currentPoll || $scope.infoMessage)
                    return false;

                var selectedCount = 0;
                angular.forEach($scope.currentPoll.PollOptions, (po: MyVote.Services.AppServer.Models.PollOption) => {
                    if (po.Selected)
                        selectedCount++;
                });

                return $scope.currentPoll.PollMinAnswers <= selectedCount &&
                    selectedCount <= $scope.currentPoll.PollMaxAnswers;
            };

            $scope.canDelete = () => {
                return !$scope.infoMessage && $scope.currentPoll && $scope.currentPoll.UserID === authService.userId;
            };

            $scope.submit = () => {
                var pollResponse: MyVote.Services.AppServer.Models.PollResponse = {
                    PollID: $scope.currentPoll.PollID,
                    UserID: authService.userId,
                    Comment: null,
                    ResponseItems: $scope.currentPoll.PollOptions.map(po => {
                        var responseItem: MyVote.Services.AppServer.Models.ResponseItem = {
                            PollOptionID: po.PollOptionID,
                            IsOptionSelected: po.Selected
                        };
                        return responseItem;
                    })
                };

                $scope.infoMessage = 'Submitting your vote...';
                
                myVoteService.submitResponse(pollResponse).then(
                    (result: string) => {
                        $log.info('ViewPoll submit: ', result);
                        $scope.infoMessage = 'Your vote has been recorded!';
                        $location.path('/pollResult/' + $scope.pollId).replace();
                    },
                    error => {
                        ViewPollCtrl.showError($scope, error, 'Error occurred recording your vote.');
                        $log.error(error);
                    });
            };

            $scope.delete = () => {
                if (!window.confirm('Are you sure you want to delete this poll?'))
                    return;

                $scope.infoMessage = 'Deleting poll...';
                myVoteService.deletePoll($scope.currentPoll.PollID).then(
                    result => {
                        $scope.infoMessage = result;
                        $location.path('/polls');
                    },
                    error => {
                        ViewPollCtrl.showError($scope, error, 'Error occurred deleting your poll.');
                        $log.error(error);
                    });
            };
        }

        private static showError($scope: ViewPollScope, error: any, defaultMessage: string): void {
            var message: string = error.data || error.message || defaultMessage;
            
            var openParenIndex = message.indexOf('(');
            if (openParenIndex !== -1) {
                var closeParenIndex = message.indexOf(')', openParenIndex);
                if (closeParenIndex !== -1) {
                    message = message.substr(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
                }
            }

            $scope.infoMessage = null;
            $scope.errorMessage = message;
        }
    }
}