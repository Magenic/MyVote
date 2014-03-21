/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';

    export interface PollResultScope extends ng.IScope {
        pollResult: MyVote.AppServer.Models.PollResult;
        infoMessage: string;
        errorMessage: string;
        newCommentText: string;
        authorized: boolean;

        chartConfig: any;

        canDelete(): boolean;
        delete(): void;
        submitComment(parentComment: MyVote.AppServer.Models.PollResultComment, text: string): void;
    }

    export interface PollResultParams extends ng.route.IRouteParamsService {
        pollId: number;
    }

    export class PollResultCtrl {
        static $inject = ['$scope', '$rootScope', '$routeParams', '$q', '$log', '$location', 'myVoteService', 'authService'];

        constructor($scope: PollResultScope, $rootScope: ng.IRootScopeService, $routeParams: PollResultParams, $q: ng.IQService, $log: ng.ILogService,
            $location: ng.ILocationService, myVoteService: MyVote.Services.MyVoteService, authService: MyVote.Services.AuthService) {

            $scope.infoMessage = 'Loading results...';
            $scope.authorized = authService.userId != null;

            $rootScope.$on(Services.AuthService.LoginEvent, ()=> {
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

            myVoteService.getPollResult($routeParams.pollId).then(
                (result: MyVote.AppServer.Models.PollResult) => {
                    $scope.infoMessage = null;
                    $scope.pollResult = result;

                    // set up chart data - object matrix {}[][]
                    var categories: string[] = [];
                    var resultValues: number[] = $scope.pollResult.Results.map((resultItem: MyVote.AppServer.Models.PollResultItem, index: number) => {
                        categories.push(resultItem.OptionText);
                        return resultItem.ResponseCount === 0? null : resultItem.ResponseCount;
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
                },
                error => {
                    $log.error('PollResultCtrl load result error: ', error);
                    $scope.errorMessage = UtilityService.FormatError(error, 'Error loading result.');
                });

            $scope.canDelete = () => {
                return $scope.pollResult && $scope.pollResult.IsPollOwnedByUser && !$scope.infoMessage;
            };

            $scope.delete = () => {
                if (!window.confirm('Are you sure you want to delete this poll?'))
                    return;

                $scope.infoMessage = 'Deleting poll...';
                myVoteService.deletePoll($scope.pollResult.PollID).then(
                    result => {
                        $scope.infoMessage = result;
                        $location.path('/polls');
                    },
                    error => {
                        $log.error(error);
                        $scope.errorMessage = UtilityService.FormatError(error, 'Error occurred deleting your poll.');
                    });
            };

            $scope.submitComment = (parentComment: MyVote.AppServer.Models.PollResultComment, text: string)=> {
                if (!text) {
                    return;
                }
                var newComment: MyVote.AppServer.Models.PollResultComment = {
                    PollID: $scope.pollResult.PollID,
                    ParentCommentID: parentComment == null? null : parentComment.PollCommentID,
                    CommentText: text,
                    UserID: authService.userId,
                    UserName: authService.userName,
                    PollCommentID: null,
                    CommentDate: null,
                    Comments: []
                };
                myVoteService.submitComment(newComment).then(
                    result => {
                        newComment.PollCommentID = result.PollCommentID;
                        newComment.CommentDate = new Date();
                        if (newComment.ParentCommentID == null) {
                            $scope.newCommentText = null;
                            $scope.pollResult.Comments.push(newComment);
                        } else {
                            parentComment.Comments.push(newComment);
                            (<any> parentComment).replyText = '';
                        }
                    },
                    error=> {
                        $log.error(error);
                        $scope.errorMessage = UtilityService.FormatError(error, 'Error occurred posting a comment.');
                    });
            };
        }
    }
}