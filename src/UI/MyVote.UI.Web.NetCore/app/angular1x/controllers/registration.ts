/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';

    //Interface extends ng.IScope from angular.d.ts TypeScript definition file
    //This allows referencing properties on the Angular this object without the TypeScript compiler having issues with undefined properties
    export interface RegistrationScope extends ng.IScope {
        email: string;
        screenName: string;
        birthDate: string;
        sex: string;
        zip: string;
        sOptions: { id: string; name: string }[];        

        registrationForm: ng.IFormController;
        errorMessage: string;
        busyMessage: string;
        invalidInput(name: string): boolean;
        validationMessage(name: string): string;
        register(): void;
        openBirthDatePicker($event): void;
        dateOptions: ng.ui.bootstrap.IDatepickerConfig;
        birthDateOpened: boolean;
        
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
            
            $scope.sOptions = [
                {
                    id: "F",
                    name: "Female"                    
                },
                {
                    id: "M",
                    name: "Male"

                }];

            $scope.invalidInput = (name: string) => {
                var field = $scope.registrationForm[name];
                return (this.submitted || field.$dirty) && field.$invalid;
            };

            $scope.validationMessage = (name: string) => {
                return this.errorMap[name];
            };

            $scope.dateOptions = {
                showWeeks: false
            };

            $scope.openBirthDatePicker = (event: ng.IAngularEvent) => {
                event.preventDefault();
                event.stopPropagation();

                $scope.birthDateOpened = true;
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