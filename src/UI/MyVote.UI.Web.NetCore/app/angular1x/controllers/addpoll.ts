/// <reference path="../_refs.ts" />

module MyVote.Controllers {
    'use strict';


    export interface AddPollScope {
        vm: AddPollCtrl;
    }

    export class AddPollCtrl implements AddPollScope {

        //Prefer class instance variables as opposed to polluting scope directly with variables
        newPoll: MyVote.Services.AppServer.Models.Poll;
        submitted: boolean;
        busyMessage: string;
        errorMessage: string;
        addPollForm: any;
        setStartEndDate: string;
        defaultStartDate: string;
        defaultEndDate: string;
        startDate: string;
        endDate: string;
        multiAnswer: boolean;
        pollImage: File;
        added: boolean;
        showAccordion: boolean;
        dateOptions: ng.ui.bootstrap.IDatepickerConfig;
        startDateOpened: boolean;
        endDateOpened: boolean;
        pollCategories: { id: number; name: string }[];
        vm: AddPollCtrl;
       
        private errorMap = {
            'newPoll.PollCategoryID': 'Please select a category.',
            'newPoll.PollQuestion': 'Please enter a question.',
            'startDate': 'Please enter a start date.',
            'endDate': 'Please enter an end date.',
            'pollOption.OptionText': 'Please enter at least two answers.',
            'newPoll.PollMinAnswers': 'Please enter the minimum number of answers.',
            'newPoll.PollMaxAnswers': 'Please enter the maximum number of answers.'
        };

        //Ensures when minification occurs, proper dependency injection occurs and is still references by name in the constructor
        static $inject = [            
            '$location',
            '$log',
            '$scope',
            'myVoteService',
            'authService',
            'signalrService'
        ];
        constructor(           
            //this: AddPollScope,
            public $location: ng.ILocationService,
            $log: ng.ILogService,
            $scope: angular.IScope,
            public myVoteService: Services.MyVoteService,
            authService: Services.AuthService,
            public signalrService: Services.SignalrService
        ) {
            this.vm = this;

            this.submitted = false;
            this.errorMessage = null;
            this.busyMessage = null;
            this.added = false;
            
            this.multiAnswer = false;
            this.setStartEndDate = 'no';

            this.defaultStartDate = String(new Date('1/1/0001'));
            this.defaultEndDate = String(new Date('12/31/9999'));

            var pollOptions = [1, 2, 3, 4, 5].map(n => {
                return {
                    PollID: null,
                    PollOptionID: null,
                    OptionPosition: n,
                    OptionText: null,
                    Selected: false
                };
            });
            this.newPoll = {
                PollID: null,
                PollAdminRemovedFlag: false,
                PollCategoryID: null,
                PollDateRemoved: null,
                PollDeletedDate: null,
                PollDeletedFlag: false,
                PollStartDate: null,
                PollEndDate: null,
                PollImageLink: null,
                PollMaxAnswers: 1,
                PollMinAnswers: 1,
                UserID: authService.userId,
                PollDescription: null,
                PollQuestion: null,
                PollOptions: pollOptions
            };

            this.showAccordion = false;

            this.dateOptions = {
                showWeeks: false                             
            };

            //"Fun", "Technology", "Entertainment", "News", "Sports", "Off-Topic"
            this.pollCategories = [
                    {
                        id: 1,
                        name: "Fun"
                    }, 
                    {
                        id: 2,
                        name: "Technology"
                    }, 
                    {
                        id: 3,
                        name: "Entertainment"
                    }, 
                    {
                        id: 4,
                        name: "News"
                    }, 
                    {
                        id: 5,
                        name: "Sports"
                    }, 
                    {
                        id: 6,
                        name: "Off-Topic"
                    }
                ];

            
            //Note '$scope' is still used but in a _specialized_ way for very specific features only. Not as a mamoth class object.
            $scope.$on('$locationChangeStart', (event: angular.IAngularEvent) => {
                if (!this.added && this.addPollForm.$dirty) {
                    if (!confirm('You have unsaved changes. Choose OK to leave this page or Cancel to continue editing.')) {
                        event.preventDefault();
                    }
                }
            });

        }


        answerLetter = (n: number) => String.fromCharCode(65 + n);

        openStartDatePicker = (event: ng.IAngularEvent) => {
            event.preventDefault();
            event.stopPropagation();

            this.startDateOpened = true;
        };

        openEndDatePicker = (event: ng.IAngularEvent) => {
            event.preventDefault();
            event.stopPropagation();

            this.endDateOpened = true;
        };

        invalidInput = (name: string) => {
            var field = this.addPollForm[name];
            if (field === undefined) {
                return false;
            }
            return (this.submitted || field.$dirty) && field.$invalid;
        };

        validationMessage = (name: string) => {
            return this.errorMap[name];
        };

        toggleAccordion = () => {
            this.showAccordion = !(this.showAccordion);
        };

        submit = () => {
            this.submitted = true;
            this.errorMessage = null;

            if (this.addPollForm.$invalid) {
                return;
            }

            this.busyMessage = 'Posting Poll...';

            // remove empty options
            var finalOptions: Services.AppServer.Models.PollOption[] = [];
            var currentPosition = 0;
            for (var optionIndex = 0; optionIndex < this.newPoll.PollOptions.length; optionIndex++) {
                var option = this.newPoll.PollOptions[optionIndex];
                option.OptionPosition = currentPosition++;
                if (option.OptionText) {
                    finalOptions.push(option);
                }
            }
            this.newPoll.PollOptions = finalOptions;

            // if not setting start/end dates, reset to defaults
            if (this.setStartEndDate == 'no') {
                this.startDate = this.defaultStartDate;
                this.endDate = this.defaultEndDate;
            }

            if (!this.multiAnswer) {
                this.newPoll.PollMinAnswers = 1;
                this.newPoll.PollMaxAnswers = 1;
            }

            this.newPoll.PollStartDate = new Date(Date.parse(this.startDate));
            this.newPoll.PollStartDate.setHours(0, 0, 0, 0);

            this.newPoll.PollEndDate = new Date(Date.parse(this.endDate));
            this.newPoll.PollEndDate.setHours(11, 59, 59, 999);

            this.myVoteService.uploadImage(this.pollImage)
                .then((imageUrl: any) => {
                    this.newPoll.PollImageLink = imageUrl;
                    return this.myVoteService.savePoll(this.newPoll);
                }).then((poll: MyVote.Services.AppServer.Models.Poll): any => {
                    this.added = true;
                    this.signalrService.addPoll();
                    this.busyMessage = 'Success! Viewing your poll...';
                    this.$location.path('/viewPoll/' + poll.PollID);
                },
                error => {
                    this.busyMessage = null;
                    this.errorMessage = UtilityService.formatError(error, "Error saving poll.");
                });
        };

    }
}