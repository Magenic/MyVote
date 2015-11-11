/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';
    
    //Interface extends ng.IScope from angular.d.ts TypeScript definition file
    //This allows referencing properties on the Angular this object without the TypeScript compiler having issues with undefined properties
    export interface LandingScope extends ng.IScope {
        providers: Models.AuthProvider[];
        errorMessage: string;
        busyMessage: string;
        busy: boolean;

        login(provider: Models.AuthProvider): void;
    }

    export class LandingCtrl {
        static $inject = ['$scope', '$location', '$log', 'authService'];

        constructor(private $scope: LandingScope, $location: ng.ILocationService, $log: ng.ILogService, authService: Services.AuthService) {
            $scope.errorMessage = null;

            if (authService.foundSavedLogin()) {
                $log.info('LandingCtrl: auth service found saved login.');

                this.$scope.busyMessage = 'Checking for saved login...';
                this.$scope.busy = true;

                authService.loadSavedLogin().then(
                    login => {
                        if (login) {
                            var targetPath = authService.desiredPath || '/polls';
                            authService.desiredPath = null;
                            $location.path(targetPath).replace();
                        } else {
                            $scope.providers = authService.providers;
                            this.$scope.busyMessage = null;
                            this.$scope.busy = false;
                        }
                    },
                    error => {
                        this.$scope.busyMessage = null;
                        this.$scope.busy = false;
                        this.$scope.errorMessage = error;
                    }
                );
            } else {
                $scope.providers = authService.providers;
                this.$scope.busyMessage = null;
                this.$scope.busy = false;
            }

            $scope.login = (provider: Models.AuthProvider) => {
                if ($scope.busy) {
                    return;
                }

                $scope.busyMessage = 'Logging in...';
                $scope.busy = true;
                $scope.errorMessage = null;

                authService.login(provider.id).then(
                    result => $scope.$apply(() => {
                        var registered = authService.isRegistered();
                        $log.info('LandingCtrl: login complete. registered: ', registered);

                        var navPath = registered ? '/polls' : '/registration';
                        $location.path(navPath);
                    }),
                    error => $scope.$apply(() => {
                        $log.error('LandingCtrl: login error: ', error);

                        $scope.busyMessage = null;
                        $scope.busy = false;
                        $scope.errorMessage = error;
                    }));
            };
        }

        
    }
}