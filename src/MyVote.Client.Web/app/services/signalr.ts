/// <reference path="../_refs.ts" />

module MyVote.Services {
    'use strict';
    
    export class SignalrService {
        public static $inject = ['$rootScope'];

        public static PollAddedEvent = 'myVoteHub.pollAdded';

        private connection: HubConnection;
        private proxy: HubProxy;
        
        constructor(private $rootScope: ng.IRootScopeService) {
            this.connection = $.hubConnection();
            this.proxy = this.connection.createHubProxy('myVoteHub');
            this.connection.start();

            this.proxy.on('pollAdded', (...msg: any[]) => {
                this.$rootScope.$emit(SignalrService.PollAddedEvent, msg[0]);
            });
        }
        
        public addPoll() {
            this.proxy.invoke('addPoll');
        }
    }

    App.service('signalrService', SignalrService);
}