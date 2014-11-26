/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Controllers) {
        'use strict';

        var ViewPollCtrl = (function () {
            function ViewPollCtrl($scope, $routeParams, $location, $log, myVoteService, authService) {
                $scope.infoMessage = 'Loading poll...';
                $scope.errorMessage = null;

                $scope.pollId = $routeParams.pollId;

                myVoteService.getPollResponse($scope.pollId, authService.userId).then(function (response) {
                    if (response && response.SubmissionDate) {
                        $location.path('/pollResult/' + $scope.pollId).replace();
                        return null;
                    } else {
                        return myVoteService.getPoll($scope.pollId);
                    }
                }).then(function (poll) {
                    $scope.infoMessage = null;
                    $scope.currentPoll = poll;
                }, function (error) {
                    $scope.infoMessage = null;
                    ViewPollCtrl.showError($scope, error, 'Could not load poll.');
                    $log.error(error);
                });

                $scope.canSubmit = function () {
                    if (!$scope.currentPoll || $scope.infoMessage)
                        return false;

                    var selectedCount = 0;
                    angular.forEach($scope.currentPoll.PollOptions, function (po) {
                        if (po.Selected)
                            selectedCount++;
                    });

                    return $scope.currentPoll.PollMinAnswers <= selectedCount && selectedCount <= $scope.currentPoll.PollMaxAnswers;
                };

                $scope.canDelete = function () {
                    return !$scope.infoMessage && $scope.currentPoll && $scope.currentPoll.UserID === authService.userId;
                };

                $scope.submit = function () {
                    var pollResponse = {
                        PollID: $scope.currentPoll.PollID,
                        UserID: authService.userId,
                        Comment: null,
                        ResponseItems: $scope.currentPoll.PollOptions.map(function (po) {
                            var responseItem = {
                                PollOptionID: po.PollOptionID,
                                IsOptionSelected: po.Selected
                            };
                            return responseItem;
                        })
                    };

                    $scope.infoMessage = 'Submitting your vote...';

                    myVoteService.submitResponse(pollResponse).then(function (result) {
                        $log.info('ViewPoll submit: ', result);
                        $scope.infoMessage = 'Your vote has been recorded!';
                        $location.path('/pollResult/' + $scope.pollId).replace();
                    }, function (error) {
                        ViewPollCtrl.showError($scope, error, 'Error occurred recording your vote.');
                        $log.error(error);
                    });
                };

                $scope.delete = function () {
                    if (!window.confirm('Are you sure you want to delete this poll?'))
                        return;

                    $scope.infoMessage = 'Deleting poll...';
                    myVoteService.deletePoll($scope.currentPoll.PollID).then(function (result) {
                        $scope.infoMessage = result;
                        $location.path('/polls');
                    }, function (error) {
                        ViewPollCtrl.showError($scope, error, 'Error occurred deleting your poll.');
                        $log.error(error);
                    });
                };
            }
            ViewPollCtrl.showError = function ($scope, error, defaultMessage) {
                var message = error.data || error.message || defaultMessage;

                var openParenIndex = message.indexOf('(');
                if (openParenIndex !== -1) {
                    var closeParenIndex = message.indexOf(')', openParenIndex);
                    if (closeParenIndex !== -1) {
                        message = message.substr(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
                    }
                }

                $scope.infoMessage = null;
                $scope.errorMessage = message;
            };
            ViewPollCtrl.$inject = ['$scope', '$routeParams', '$location', '$log', 'myVoteService', 'authService'];
            return ViewPollCtrl;
        })();
        Controllers.ViewPollCtrl = ViewPollCtrl;
    })(MyVote.Controllers || (MyVote.Controllers = {}));
    var Controllers = MyVote.Controllers;
})(MyVote || (MyVote = {}));
//# sourceMappingURL=viewpoll.js.map
