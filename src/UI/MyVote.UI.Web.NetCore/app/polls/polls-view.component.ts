declare var angular: angular.IAngularStatic;

import { Component, Inject } from '@angular/core';
//import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { PollsService } from './polls.service';
import { downgradeComponent } from '@angular/upgrade/static';
import { Logger } from 'angular2-logger/core';
//import { AuthService } from '../angular1x/services/auth'

@Component({
    //module.id variable is available and contains the absolute URL of the component class module file. allows using relative paths vs. absolute paths for commonjs modules + systemjs loader
    moduleId: module.id, //remove node typings to show what will happen (or any other typings example)
    selector: 'polls-view',
    templateUrl: './polls-view.component.html',
    styleUrls: ['./polls-view.component.css']
})

export class PollsViewComponent {

    infoMessage: string;
    errorMessage: string;
    pollId: number;
    currentPoll: MyVote.Services.AppServer.Models.Poll;

    constructor(private location: Location,
        private pollsService: PollsService,
        //private route: ActivatedRoute,        
        private logger: Logger,
        @Inject('authService') private authService,
        @Inject('$routeParams') private $routeParams
        //private authService: AuthService
        )
    {
        this.infoMessage = 'Loading poll...';
        this.errorMessage = null;
        this.currentPoll = <MyVote.Services.AppServer.Models.Poll>{};

        this.pollId = $routeParams.pollId;
        //let pollIdQueryParam = router.parseUrl(router.url).queryParams['pollId'];
        //this.pollId = Number(pollIdQueryParam) || null;        
        //route.queryParams.subscribe(
        //    data => this.pollId = data['pollId']);

        pollsService.getPollResponse(this.pollId, authService.userId)
            .then(response => {
                if (response && response.SubmissionDate) {
                    location.go('/pollResult/' + this.pollId);                    
                    location.replaceState;
                    return null;
                } else {
                    return pollsService.getPoll(this.pollId);
                }
            }).then((poll: MyVote.Services.AppServer.Models.Poll): any => {
                this.infoMessage = null;
                this.currentPoll = poll;
            },
            error => {
                this.infoMessage = null;
                this.showError(error, 'Could not load poll.');
                logger.error(error);
            });


    }

    canSubmit() {
        if (!this.currentPoll || this.infoMessage)
            return false;

        var selectedCount = 0;
        angular.forEach(this.currentPoll.PollOptions, (po: MyVote.Services.AppServer.Models.PollOption) => {
            if (po.Selected)
                selectedCount++;
        });

        return this.currentPoll.PollMinAnswers <= selectedCount &&
            selectedCount <= this.currentPoll.PollMaxAnswers;
    };

    canDelete() {
        return !this.infoMessage && this.currentPoll && this.currentPoll.UserID === this.authService.userId;
    };

    submit() {
        var pollResponse: MyVote.Services.AppServer.Models.PollResponse = {
            PollID: this.currentPoll.PollID,
            UserID: this.authService.userId,
            Comment: null,
            ResponseItems: this.currentPoll.PollOptions.map(po => {
                var responseItem: MyVote.Services.AppServer.Models.ResponseItem = {
                    PollOptionID: po.PollOptionID,
                    IsOptionSelected: po.Selected
                };
                return responseItem;
            })
        };

        this.infoMessage = 'Submitting your vote...';

        this.pollsService.submitResponse(pollResponse).then(
            (result: string): any => {
                this.logger.info('ViewPoll submit: ', result);
                this.infoMessage = 'Your vote has been recorded!';
                this.location.go('/pollResult/' + this.pollId);
                this.location.replaceState;
            },
            error => {
                this.showError(error, 'Error occurred recording your vote.');
                this.logger.error(error);
            });
    };

    delete() {
        if (!window.confirm('Are you sure you want to delete this poll?'))
            return;

        this.infoMessage = 'Deleting poll...';
        this.pollsService.deletePoll(this.currentPoll.PollID).then(
            result => {
                this.infoMessage = result;
                this.location.go('/polls');
            },
            error => {
                this.showError(error, 'Error occurred deleting your poll.');
                this.logger.error(error);
            });
    };

    showError(error: any, defaultMessage: string): void {
        var message: string = error.data || error.message || defaultMessage;

        var openParenIndex = message.indexOf('(');
        if (openParenIndex !== -1) {
            var closeParenIndex = message.indexOf(')', openParenIndex);
            if (closeParenIndex !== -1) {
                message = message.substr(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
            }
        }

        this.infoMessage = null;
        this.errorMessage = message;
    }
}

//Instead of registering a component, we register a pollsView directive, a downgraded version of the Angular 2 component.
//The as angular.IDirectiveFactory cast tells the TypeScript compiler that the return value of the downgradeComponent method is a directive factory.
angular.module('MyVoteApp')
    .directive('pollsView', downgradeComponent({ component: PollsViewComponent }) as angular.IDirectiveFactory);