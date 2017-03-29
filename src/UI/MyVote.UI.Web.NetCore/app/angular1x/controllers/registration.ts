/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';

    //Interface extends ng.IScope from angular.d.ts TypeScript definition file
    //This allows referencing properties on the Angular this object without the TypeScript compiler having issues with undefined properties
    export interface IRegistration {

        vm: RegistrationCtrl;

        invalidInput(name: string): boolean;
        validationMessage(name: string): string;
        register(): void;
        openBirthDatePicker($event): void;
        
    }

    export class RegistrationCtrl implements IRegistration {

        //Prefer class instance variables as opposed to polluting scope directly with variables
        email: string;
        screenName: string;
        birthDate: string;
        sex: string;
        zip: string;
        sOptions: { id: string; name: string }[];

        registrationForm: ng.IFormController;
        errorMessage: string;
        busyMessage: string;

        dateOptions: ng.ui.bootstrap.IDatepickerConfig;
        birthDateOpened: boolean;

        vm: RegistrationCtrl;

        static $inject = ['$location', '$log', 'authService', 'myVoteService'];

        private submitted: boolean;
        private errorMap = {
            'email': 'Please enter a valid email address.',
            'screenName': 'Please enter a screen name.',
            'birthDate': 'Please enter a valid birth date.',
            'zip': 'Please enter a valid ZIP code.'
        };

        constructor(
            //$scope: RegistrationScope,
            public $location: ng.ILocationService,
            public $log: ng.ILogService,
            public authService: Services.AuthService,
            public myVoteService: Services.MyVoteService) {            

            this.vm = this;
                            
            this.vm.submitted = false;
            this.vm.errorMessage = null;
            this.vm.busyMessage = null;
            
            this.vm.sOptions = [
                {
                    id: "F",
                    name: "Female"                    
                },
                {
                    id: "M",
                    name: "Male"

                }];

            this.vm.dateOptions = {
                showWeeks: false
            };

        }

        public invalidInput = (name: string) => {
            var field = this.vm.registrationForm[name];
            return (this.vm.submitted || field.$dirty) && field.$invalid;
        };

        public validationMessage = (name: string) => {
            return this.vm.errorMap[name];
        };

        public openBirthDatePicker = (event: ng.IAngularEvent) => {
            event.preventDefault();
            event.stopPropagation();

            this.vm.birthDateOpened = true;
        };

        public register = () => {
            this.vm.submitted = true;
            this.vm.errorMessage = null;

            if (this.registrationForm.$invalid) {
                return;
            }

            if (!this.vm.authService.profileId) {
                this.vm.errorMessage = 'You must login first.';
                return;
            }

            this.vm.busyMessage = 'Performing registration...';

            // TODO: Use TypeLite registration interface
            this.vm.myVoteService.saveUser({
                UserID: null,
                ProfileID: this.vm.authService.profileId,
                UserName: this.vm.screenName,
                FirstName: 'firstname_from_auth',
                LastName: 'lastname_from_auth',
                PostalCode: this.vm.zip,
                Gender: this.vm.sex,
                EmailAddress: 'email_from_auth',
                BirthDate: new Date(this.vm.birthDate)
            }).then(
                response => {
                    this.vm.$log.info('RegistrationCtrl: saveUser success.');
                    this.vm.$location.path('/polls');
                },
                error => {
                    this.vm.$log.error('RegistrationCtrl: saveUser error: ', error);

                    this.vm.busyMessage = null;
                    this.vm.errorMessage = error;
                });
        };
        
    }
}