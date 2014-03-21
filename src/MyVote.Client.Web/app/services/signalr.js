/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Services) {
        'use strict';

        var SignalrService = (function () {
            function SignalrService($rootScope) {
                var _this = this;
                this.$rootScope = $rootScope;
                this.connection = $.hubConnection();
                this.proxy = this.connection.createHubProxy('myVoteHub');
                this.connection.start();

                this.proxy.on('pollAdded', function () {
                    var msg = [];
                    for (var _i = 0; _i < (arguments.length - 0); _i++) {
                        msg[_i] = arguments[_i + 0];
                    }
                    _this.$rootScope.$emit(SignalrService.PollAddedEvent, msg[0]);
                });
            }
            SignalrService.prototype.addPoll = function () {
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
