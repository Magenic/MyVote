/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Controllers) {
        'use strict';

        var PollResultCtrl = (function () {
            function PollResultCtrl($scope, $rootScope, $routeParams, $q, $log, $location, myVoteService, authService) {
                $scope.infoMessage = 'Loading results...';
                $scope.authorized = authService.userId != null;

                $rootScope.$on(MyVote.Services.AuthService.LoginEvent, function () {
                    $scope.authorized = authService.isLoggedIn();
                });

                $scope.chartConfig = {
                    loading: true,
                    options: {
                        chart: {
                            backgroundColor: null
                        },
                        credits: { enabled: false }
                    }
                };

                myVoteService.getPollResult($routeParams.pollId).then(function (result) {
                    $scope.infoMessage = null;
                    $scope.pollResult = result;

                    // set up chart data - object matrix {}[][]
                    var categories = [];
                    var resultValues = $scope.pollResult.Results.map(function (resultItem, index) {
                        categories.push(resultItem.OptionText);
                        return resultItem.ResponseCount === 0 ? null : resultItem.ResponseCount;
                    });

                    $scope.chartConfig = {
                        options: {
                            chart: {
                                type: 'bar',
                                backgroundColor: 'rgba(255,255,255,0.75)',
                                borderRadius: 0,
                                marginLeft: 10,
                                marginRight: 0,
                                height: categories.length * 80
                            },
                            title: null,
                            credits: { enabled: false },
                            legend: { enabled: false },
                            yAxis: {
                                gridLineWidth: 0,
                                labels: { enabled: false },
                                title: null
                            }
                        },
                        xAxis: {
                            categories: categories,
                            tickLength: 0,
                            lineWidth: 0,
                            labels: {
                                align: 'left',
                                x: 3,
                                y: -20,
                                useHTML: true,
                                style: {
                                    color: '#444',
                                    fontSize: '16px',
                                    width: '1000px'
                                }
                            }
                        },
                        series: [
                            {
                                data: resultValues,
                                name: 'Votes',
                                borderWidth: 0,
                                color: '#444',
                                shadow: true,
                                pointWidth: 25,
                                verticalAlign: 'middle',
                                dataLabels: {
                                    enabled: true,
                                    align: 'right',
                                    x: -10,
                                    style: {
                                        color: '#dedede',
                                        fontSize: '14px',
                                        fontWeight: 'bold'
                                    }
                                }
                            }
                        ],
                        loading: false
                    };
                }, function (error) {
                    $log.error('PollResultCtrl load result error: ', error);
                    $scope.errorMessage = MyVote.UtilityService.FormatError(error, 'Error loading result.');
                });

                $scope.canDelete = function () {
                    return $scope.pollResult && $scope.pollResult.IsPollOwnedByUser && !$scope.infoMessage;
                };

                $scope.delete = function () {
                    if (!window.confirm('Are you sure you want to delete this poll?'))
                        return;

                    $scope.infoMessage = 'Deleting poll...';
                    myVoteService.deletePoll($scope.pollResult.PollID).then(function (result) {
                        $scope.infoMessage = result;
                        $location.path('/polls');
                    }, function (error) {
                        $log.error(error);
                        $scope.errorMessage = MyVote.UtilityService.FormatError(error, 'Error occurred deleting your poll.');
                    });
                };

                $scope.submitComment = function (parentComment, text) {
                    if (!text) {
                        return;
                    }
                    var newComment = {
                        PollID: $scope.pollResult.PollID,
                        ParentCommentID: parentComment == null ? null : parentComment.PollCommentID,
                        CommentText: text,
                        UserID: authService.userId,
                        UserName: authService.userName,
                        PollCommentID: null,
                        CommentDate: null,
                        Comments: []
                    };
                    myVoteService.submitComment(newComment).then(function (result) {
                        newComment.PollCommentID = result.PollCommentID;
                        newComment.CommentDate = new Date();
                        if (newComment.ParentCommentID == null) {
                            $scope.newCommentText = null;
                            $scope.pollResult.Comments.push(newComment);
                        } else {
                            parentComment.Comments.push(newComment);
                            parentComment.replyText = '';
                        }
                    }, function (error) {
                        $log.error(error);
                        $scope.errorMessage = MyVote.UtilityService.FormatError(error, 'Error occurred posting a comment.');
                    });
                };
            }
            PollResultCtrl.$inject = ['$scope', '$rootScope', '$routeParams', '$q', '$log', '$location', 'myVoteService', 'authService'];
            return PollResultCtrl;
        })();
        Controllers.PollResultCtrl = PollResultCtrl;
    })(MyVote.Controllers || (MyVote.Controllers = {}));
    var Controllers = MyVote.Controllers;
})(MyVote || (MyVote = {}));
