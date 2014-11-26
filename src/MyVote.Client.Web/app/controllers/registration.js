/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Controllers) {
        'use strict';

        var RegistrationCtrl = (function () {
            function RegistrationCtrl($scope, $location, $log, authService, myVoteService) {
                var _this = this;
                this.errorMap = {
                    'email': 'Please enter a valid email address.',
                    'screenName': 'Please enter a screen name.',
                    'birthDate': 'Please enter a valid birth date.',
                    'zip': 'Please enter a valid ZIP code.'
                };
                this.submitted = false;
                $scope.errorMessage = null;
                $scope.busyMessage = null;

                $scope.invalidInput = function (name) {
                    var field = $scope.registrationForm[name];
                    return (_this.submitted || field.$dirty) && field.$invalid;
                };

                $scope.validationMessage = function (name) {
                    return _this.errorMap[name];
                };

                $scope.register = function () {
                    _this.submitted = true;
                    $scope.errorMessage = null;

                    if ($scope.registrationForm.$invalid) {
                        return;
                    }

                    if (!authService.profileId) {
                        $scope.errorMessage = 'You must login first.';
                        return;
                    }

                    $scope.busyMessage = 'Performing registration...';

                    // TODO: Use TypeLite registration interface
                    myVoteService.saveUser({
                        UserID: null,
                        UserRoleID: null,
                        ProfileID: authService.profileId,
                        UserName: $scope.screenName,
                        FirstName: 'firstname_from_auth',
                        LastName: 'lastname_from_auth',
                        PostalCode: $scope.zip,
                        Gender: $scope.sex,
                        EmailAddress: 'email_from_auth',
                        BirthDate: new Date($scope.birthDate)
                    }).then(function (response) {
                        $log.info('RegistrationCtrl: saveUser success.');
                        $location.path('/polls');
                    }, function (error) {
                        $log.error('RegistrationCtrl: saveUser error: ', error);

                        $scope.busyMessage = null;
                        $scope.errorMessage = error;
                    });
                };
            }
            RegistrationCtrl.$inject = ['$scope', '$location', '$log', 'authService', 'myVoteService'];
            return RegistrationCtrl;
        })();
        Controllers.RegistrationCtrl = RegistrationCtrl;
    })(MyVote.Controllers || (MyVote.Controllers = {}));
    var Controllers = MyVote.Controllers;
})(MyVote || (MyVote = {}));
//# sourceMappingURL=registration.js.map
