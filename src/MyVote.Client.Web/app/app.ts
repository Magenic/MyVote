/// <reference path="_refs.ts" />

module MyVote {
    'use strict';

    export var App: ng.IModule = angular.module('MyVoteApp', ['ngRoute', 'ngResource', 'highcharts-ng']);

    App.factory('zumoAuthInterceptor', () => {
        return {
            request: (config: any) => {
                if (Globals.zumoUserKey) {
                    config.headers['Authorization'] = 'Bearer ' + Globals.zumoUserKey;
                }
                return config;
            }
        };
    });

    App.config([
        '$routeProvider', '$httpProvider',
        ($routeProvider: ng.route.IRouteProvider, $httpProvider: ng.IHttpProvider) => {
            // enable CORS on IE <= 9
            delete $httpProvider.defaults.headers.common['X-Requested-With'];
            $httpProvider.interceptors.push('zumoAuthInterceptor');

            $routeProvider
                .when('/landing', {
                        controller: MyVote.Controllers.LandingCtrl,
                        templateUrl: '/app/partials/landing.html'
                    })
                .when('/registration', {
                        controller: MyVote.Controllers.RegistrationCtrl,
                        templateUrl: '/app/partials/registration.cshtml'
                })
                .when('/polls', {
                        controller: MyVote.Controllers.PollsCtrl,
                        templateUrl: '/app/partials/polls.html'
                })
                .when('/viewPoll/:pollId', {
                    controller: MyVote.Controllers.ViewPollCtrl,
                    templateUrl: '/app/partials/viewpoll.html'
                })
                .when('/addPoll', {
                    controller: MyVote.Controllers.AddPollCtrl,
                    templateUrl: '/app/partials/addpoll.cshtml'
                })
                .when('/pollResult/:pollId', {
                    controller: MyVote.Controllers.PollResultCtrl,
                    templateUrl: '/app/partials/pollresult.html'
                })
                .otherwise({
                    redirectTo: '/landing'
                });
        }
    ]);

    App.filter('fromNow', () => {
        var fromNow = (dateString: string) => {
            return moment(new Date(dateString)).fromNow();
        };
        return fromNow;
    });

    interface AuthScope extends ng.IScope {
        authMessage: string;
        newPollsCount: number;
        loggedIn(): boolean;
        logOut(): void;
        getNewPollsMessage(): string;
        viewNewPolls(): void;
    }

    class AuthCtrl {
        static $inject = ['$scope', '$route', '$location', '$log', 'authService', 'signalrService'];

        private static _noAuthPaths = [
            '/', '/landing', '/registration', '/pollResult'
        ];

        constructor(
            $scope: AuthScope,
            $route: ng.route.IRouteService,
            $location: ng.ILocationService,
            $log: ng.ILogService,
            authService: Services.AuthService,
            signalrService: Services.SignalrService
        ) {
            $scope.authMessage = null;
            $scope.newPollsCount = 0;
            
            $scope.loggedIn = () => {
                return authService.isLoggedIn();
            };

            $scope.logOut = () => {
                authService.logout();
                $location.path('/landing');
            };

            $scope.getNewPollsMessage = () => {
                return $scope.newPollsCount > 1
                    ? $scope.newPollsCount + " new polls have been posted!"
                    : "One new poll has been posted!";
            };

            $scope.viewNewPolls = () => {
                $scope.newPollsCount = 0;
                if ($location.path() == '/polls') {
                    $route.reload();
                } else {
                    $location.path('/polls');
                }
            };
            
            $scope.$parent.$on(MyVote.Services.SignalrService.PollAddedEvent, () => {
                $scope.$apply(()=> {
                    var newCount = $scope.newPollsCount + 1;
                    $scope.newPollsCount = 0;
                    setTimeout(()=> $scope.$apply(()=> $scope.newPollsCount = newCount), 200);
                });
            });

            $scope.$on('$locationChangeStart', (event: ng.IAngularEvent, ...args: any[])=> {
                var newUrl: string = args[0];
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
                    setTimeout(()=> {
                        $scope.$apply(()=> {
                            $scope.authMessage = 'You must login to view that page!';
                            $location.path('/landing').replace();
                        });
                    }, 0);
                }

                // allowing through - still kick off an attempt to load saved identity
                if (authService.foundSavedLogin()) {
                    authService.loadSavedLogin().then(
                        login=> {
                            if (login) {
                                $scope.authMessage = 'Signed in as ' + authService.userName + '.';
                            }
                        });
                };
            });
        }
    }
    App.controller('auth', AuthCtrl);

    export class UtilityService {
        public static FormatError(error: any, defaultMessage: string): string {
            if (error.data) {
                return error.data.Message || error.data;
            }
            return defaultMessage;
        }
    }
}