/// <reference path="_refs.ts" />
var MyVote;
(function (MyVote) {
    'use strict';

    MyVote.App = angular.module('MyVoteApp', ['ngRoute', 'ngResource', 'highcharts-ng']);

    MyVote.App.factory('zumoAuthInterceptor', function () {
        return {
            request: function (config) {
                if (Globals.zumoUserKey) {
                    config.headers['Authorization'] = 'Bearer ' + Globals.zumoUserKey;
                }
                return config;
            }
        };
    });

    MyVote.App.config([
        '$routeProvider', '$httpProvider',
        function ($routeProvider, $httpProvider) {
            // enable CORS on IE <= 9
            delete $httpProvider.defaults.headers.common['X-Requested-With'];
            $httpProvider.interceptors.push('zumoAuthInterceptor');

            $routeProvider.when('/landing', {
                controller: MyVote.Controllers.LandingCtrl,
                templateUrl: '/app/partials/landing.html'
            }).when('/registration', {
                controller: MyVote.Controllers.RegistrationCtrl,
                templateUrl: '/app/partials/registration.cshtml'
            }).when('/polls', {
                controller: MyVote.Controllers.PollsCtrl,
                templateUrl: '/app/partials/polls.html'
            }).when('/viewPoll/:pollId', {
                controller: MyVote.Controllers.ViewPollCtrl,
                templateUrl: '/app/partials/viewpoll.html'
            }).when('/addPoll', {
                controller: MyVote.Controllers.AddPollCtrl,
                templateUrl: '/app/partials/addpoll.cshtml'
            }).when('/pollResult/:pollId', {
                controller: MyVote.Controllers.PollResultCtrl,
                templateUrl: '/app/partials/pollresult.html'
            }).otherwise({
                redirectTo: '/landing'
            });
        }
    ]);

    MyVote.App.filter('fromNow', function () {
        var fromNow = function (dateString) {
            return moment(new Date(dateString)).fromNow();
        };
        return fromNow;
    });

    var AuthCtrl = (function () {
        function AuthCtrl($scope, $route, $location, $log, authService, signalrService) {
            $scope.authMessage = null;
            $scope.newPollsCount = 0;

            $scope.loggedIn = function () {
                return authService.isLoggedIn();
            };

            $scope.logOut = function () {
                authService.logout();
                $location.path('/landing');
            };

            $scope.getNewPollsMessage = function () {
                return $scope.newPollsCount > 1 ? $scope.newPollsCount + " new polls have been posted!" : "One new poll has been posted!";
            };

            $scope.viewNewPolls = function () {
                $scope.newPollsCount = 0;
                if ($location.path() == '/polls') {
                    $route.reload();
                } else {
                    $location.path('/polls');
                }
            };

            $scope.$parent.$on(MyVote.Services.SignalrService.PollAddedEvent, function () {
                $scope.$apply(function () {
                    var newCount = $scope.newPollsCount + 1;
                    $scope.newPollsCount = 0;
                    setTimeout(function () {
                        return $scope.$apply(function () {
                            return $scope.newPollsCount = newCount;
                        });
                    }, 200);
                });
            });

            $scope.$on('$locationChangeStart', function (event) {
                var args = [];
                for (var _i = 0; _i < (arguments.length - 1); _i++) {
                    args[_i] = arguments[_i + 1];
                }
                var newUrl = args[0];
                $log.info('AuthCtrl check: ', newUrl);

                $scope.authMessage = null;
                var loggedIn = authService.isLoggedIn();
                if (loggedIn) {
                    $scope.authMessage = 'Signed in as ' + authService.userName + '.';
                    return;
                }

                var newPath = newUrl.substring(newUrl.indexOf('#') + 1);
                var lastSlashIndex = newPath.lastIndexOf('/');
                if (lastSlashIndex > 0)
                    newPath = newPath.substring(0, lastSlashIndex);

                if (newPath === '/')
                    return;

                if (AuthCtrl._noAuthPaths.indexOf(newPath) === -1) {
                    $log.info('AuthCtrl unauthorized or unrecognized route:', newPath);
                    authService.desiredPath = newPath;
                    event.preventDefault();
                    setTimeout(function () {
                        $scope.$apply(function () {
                            $scope.authMessage = 'You must login to view that page!';
                            $location.path('/landing').replace();
                        });
                    }, 0);
                }

                // allowing through - still kick off an attempt to load saved identity
                if (authService.foundSavedLogin()) {
                    authService.loadSavedLogin().then(function (login) {
                        if (login) {
                            $scope.authMessage = 'Signed in as ' + authService.userName + '.';
                        }
                    });
                }
                ;
            });
        }
        AuthCtrl.$inject = ['$scope', '$route', '$location', '$log', 'authService', 'signalrService'];

        AuthCtrl._noAuthPaths = [
            '/', '/landing', '/registration', '/pollResult'
        ];
        return AuthCtrl;
    })();
    MyVote.App.controller('auth', AuthCtrl);

    var UtilityService = (function () {
        function UtilityService() {
        }
        UtilityService.FormatError = function (error, defaultMessage) {
            if (error.data) {
                return error.data.Message || error.data;
            }
            return defaultMessage;
        };
        return UtilityService;
    })();
    MyVote.UtilityService = UtilityService;
})(MyVote || (MyVote = {}));
