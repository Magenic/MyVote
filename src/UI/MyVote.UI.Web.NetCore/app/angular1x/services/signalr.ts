/// <reference path="../_refs.ts" />

module MyVote.Services {
    'use strict';
    
    export class SignalrService {
        public static $inject = ['$rootScope'];

        public static PollAddedEvent = 'myVoteHub.pollAdded';

        //Todo: re-add once building
        //private connection: HubConnection;
        //private proxy: HubProxy;
        
        //constructor(private $rootScope: ng.IRootScopeService) {
        //    //Get jQuery hubConnection, create the proxy, and start the connection
        //    this.connection = $.hubConnection();
        //    this.connection.logging = true;
        //    //this.connection.hub.qs = { 'customParameter': 'abc123' };
        //    this.proxy = this.connection.createHubProxy('myVoteHub');
        //    this.connection.start();

        //    //Receive SignalR event when server pushes a newly added poll
        //    this.proxy.on('pollAdded', (...msg: any[]) => {
        //        // $rootScope.$emit will fire an event for all $rootScope.$on listeners only
        //        this.$rootScope.$emit(SignalrService.PollAddedEvent, msg[0]);
        //    });
        //}
        
        public addPoll() {
            //Todo: re-add once building
            //Invoking 'addPoll' method defined in hub
            //this.proxy.invoke('addPoll');
        }
    }

    App.service('signalrService', SignalrService);
}