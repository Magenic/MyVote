/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Controllers) {
        'use strict';

        var LandingCtrl = (function () {
            function LandingCtrl($scope, $location, $log, authService) {
                var _this = this;
                this.$scope = $scope;
                $scope.errorMessage = null;

                if (authService.foundSavedLogin()) {
                    $log.info('LandingCtrl: auth service found saved login.');

                    this.$scope.busyMessage = 'Checking for saved login...';
                    this.$scope.busy = true;

                    authService.loadSavedLogin().then(function (login) {
                        if (login) {
                            var targetPath = authService.desiredPath || '/polls';
                            authService.desiredPath = null;
                            $location.path(targetPath).replace();
                        } else {
                            $scope.providers = authService.providers;
                            _this.$scope.busyMessage = null;
                            _this.$scope.busy = false;
                        }
                    }, function (error) {
                        _this.$scope.busyMessage = null;
                        _this.$scope.busy = false;
                        _this.$scope.errorMessage = error;
                    });
                } else {
                    $scope.providers = authService.providers;
                    this.$scope.busyMessage = null;
                    this.$scope.busy = false;
                }

                $scope.login = function (provider) {
                    if ($scope.busy) {
                        return;
                    }

                    $scope.busyMessage = 'Logging in...';
                    $scope.busy = true;
                    $scope.errorMessage = null;

                    authService.login(provider.id).then(function (result) {
                        return $scope.$apply(function () {
                            var registered = authService.isRegistered();
                            $log.info('LandingCtrl: login complete. registered: ', registered);

                            var navPath = registered ? '/polls' : '/registration';
                            $location.path(navPath);
                        });
                    }, function (error) {
                        return $scope.$apply(function () {
                            $log.error('LandingCtrl: login error: ', error);

                            $scope.busyMessage = null;
                            $scope.busy = false;
                            $scope.errorMessage = error;
                        });
                    });
                };
            }
            LandingCtrl.$inject = ['$scope', '$location', '$log', 'authService'];
            return LandingCtrl;
        })();
        Controllers.LandingCtrl = LandingCtrl;
    })(MyVote.Controllers || (MyVote.Controllers = {}));
    var Controllers = MyVote.Controllers;
})(MyVote || (MyVote = {}));
