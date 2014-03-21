/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';

    export interface RegistrationScope extends ng.IScope {
        email: string;
        screenName: string;
        birthDate: string;
        sex: string;
        zip: string;

        registrationForm: ng.IFormController;
        errorMessage: string;
        busyMessage: string;
        invalidInput(name: string): boolean;
        validationMessage(name: string): string;
        register(): void;
    }

    export class RegistrationCtrl {
        static $inject = ['$scope', '$location', '$log', 'authService', 'myVoteService'];

        private submitted: boolean;
        private errorMap = {
            'email': 'Please enter a valid email address.',
            'screenName': 'Please enter a screen name.',
            'birthDate': 'Please enter a valid birth date.',
            'zip': 'Please enter a valid ZIP code.'
        };

        constructor(
            $scope: RegistrationScope,
            $location: ng.ILocationService,
            $log: ng.ILogService,
            authService: Services.AuthService,
            myVoteService: Services.MyVoteService) {
                
            this.submitted = false;
            $scope.errorMessage = null;
            $scope.busyMessage = null;

            $scope.invalidInput = (name: string) => {
                var field = $scope.registrationForm[name];
                return (this.submitted || field.$dirty) && field.$invalid;
            };

            $scope.validationMessage = (name: string) => {
                return this.errorMap[name];
            };

            $scope.register = () => {
                this.submitted = true;
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
                }).then(
                    response => {
                        $log.info('RegistrationCtrl: saveUser success.');
                        $location.path('/polls');
                    },
                    error => {
                        $log.error('RegistrationCtrl: saveUser error: ', error);

                        $scope.busyMessage = null;
                        $scope.errorMessage = error;
                    });
            };
        }
    }
}