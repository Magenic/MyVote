/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Services) {
        'use strict';

        var SignalrService = (function () {
            function SignalrService($rootScope) {
                var _this = this;
                this.$rootScope = $rootScope;
                //Get jQuery hubConnection, create the proxy, and start the connection
                this.connection = $.hubConnection();
                this.connection.logging = true;

                //this.connection.hub.qs = { 'customParameter': 'abc123' };
                this.proxy = this.connection.createHubProxy('myVoteHub');
                this.connection.start();

                //Receive SignalR event when server pushes a newly added poll
                this.proxy.on('pollAdded', function () {
                    var msg = [];
                    for (var _i = 0; _i < (arguments.length - 0); _i++) {
                        msg[_i] = arguments[_i + 0];
                    }
                    // $rootScope.$emit will fire an event for all $rootScope.$on listeners only
                    _this.$rootScope.$emit(SignalrService.PollAddedEvent, msg[0]);
                });
            }
            SignalrService.prototype.addPoll = function () {
                //Invoking 'addPoll' method defined in hub
                this.proxy.invoke('addPoll');
            };
            SignalrService.$inject = ['$rootScope'];

            SignalrService.PollAddedEvent = 'myVoteHub.pollAdded';
            return SignalrService;
        })();
        Services.SignalrService = SignalrService;

        MyVote.App.service('signalrService', SignalrService);
    })(MyVote.Services || (MyVote.Services = {}));
    var Services = MyVote.Services;
})(MyVote || (MyVote = {}));
//# sourceMappingURL=signalr.js.map
