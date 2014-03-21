/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Services) {
        'use strict';

        // todo: probably split into service per resource
        var MyVoteService = (function () {
            function MyVoteService($q, $log, $resource, $http) {
                this._apiResourceUrl = Globals.apiUrl.replace(/:([^\/])/, '\\:$1');

                this._q = $q;
                this._log = $log;
                this._http = $http;

                this._userResource = $resource(this._apiResourceUrl + '/api/User', {}, {
                    'get': { method: 'GET' },
                    'save': { method: 'PUT' }
                });
                this._pollResource = $resource(this._apiResourceUrl + '/api/Poll', {}, {
                    'query': { method: 'GET', isArray: true },
                    'get': { method: 'GET' },
                    'save': { method: 'PUT' },
                    'delete': { method: 'DELETE' }
                });
                this._respondResource = $resource(this._apiResourceUrl + '/api/Respond', {}, {
                    'get': { method: 'GET' },
                    'save': { method: 'PUT' }
                });
                this._resultResource = $resource(this._apiResourceUrl + '/api/PollResult', {}, {
                    'get': { method: 'GET' }
                });
                this._commentResource = $resource(this._apiResourceUrl + '/api/PollComment', {}, {
                    'save': { method: 'PUT' }
                });
            }
            MyVoteService.prototype.getUser = function (userProfileId) {
                var _this = this;
                var deferred = this._q.defer();

                var user = this._userResource.get({ userProfileId: userProfileId }, function () {
                    _this._log.info('MyVoteService: getUser success');
                    deferred.resolve(user);
                }, function (error) {
                    _this._log.error('MyVoteService: getUser error: ', error);
                    var message = error.data.Message || error.data;
                    if (message.indexOf('Sequence contains no elements') !== -1) {
                        deferred.resolve(null);
                    } else {
                        deferred.reject('Error retrieving profile: ' + MyVote.UtilityService.FormatError(error, 'unknown error.'));
                    }
                });

                return deferred.promise;
            };

            MyVoteService.prototype.saveUser = function (user) {
                var _this = this;
                var deferred = this._q.defer();

                var savedUser = this._userResource.save(user, function () {
                    _this._log.info('MyVoteService: saveUser success');
                    deferred.resolve(savedUser);
                }, function (error) {
                    _this._log.error('MyVoteService: saveUser error: ', error);
                    var message = error.data.Message || error.data;
                    var reason = message.indexOf('Cannot insert duplicate') !== -1 ? 'You have already registered.' : 'Error saving profile: ' + error.data;
                    deferred.reject(reason);
                });

                return deferred.promise;
            };

            MyVoteService.prototype.getPolls = function (filterBy) {
                var _this = this;
                var deferred = this._q.defer();

                var polls = (this._pollResource.query({ filterBy: filterBy }, function () {
                    _this._log.info('MyVoteService: getPolls success');

                    if (polls.length == 0) {
                        deferred.resolve(null);
                        return;
                    }
                    ;

                    var pollGroups = {};
                    for (var i = 0; i < polls.length; i++) {
                        var poll = polls[i];
                        if (!(poll.Category in pollGroups))
                            pollGroups[poll.Category] = [];
                        pollGroups[poll.Category].push(poll);
                    }
                    deferred.resolve(pollGroups);
                }, function (error) {
                    _this._log.error('MyVoteService: getPolls error: ', error);
                    deferred.reject(error);
                }));

                return deferred.promise;
            };

            MyVoteService.prototype.getPoll = function (id) {
                var _this = this;
                var deferred = this._q.defer();

                var poll = (this._pollResource.get({ id: id }, function () {
                    _this._log.info('MyVoteService: getPoll success');
                    deferred.resolve(poll);
                }, function (error) {
                    _this._log.error('MyVoteService: getPoll error: ', error);
                    deferred.reject(error);
                }));

                return deferred.promise;
            };

            MyVoteService.prototype.savePoll = function (poll) {
                var _this = this;
                var deferred = this._q.defer();

                var newPoll = this._pollResource.save(poll, function () {
                    _this._log.info('MyVoteService: savePoll success');
                    deferred.resolve(newPoll);
                }, function (error) {
                    _this._log.error('MyVoteService: savePoll error: ', error);
                    deferred.reject(error);
                });

                return deferred.promise;
            };

            MyVoteService.prototype.deletePoll = function (pollId) {
                var _this = this;
                var deferred = this._q.defer();

                this._pollResource.delete({ id: pollId }, function () {
                    _this._log.info('MyVoteService: deletePoll success');
                    deferred.resolve('The poll was deleted.');
                }, function (error) {
                    _this._log.error('MyVoteService: deletePoll error: ', error);
                    deferred.reject(error);
                });

                return deferred.promise;
            };

            MyVoteService.prototype.submitResponse = function (response) {
                var _this = this;
                var deferred = this._q.defer();

                this._respondResource.save(response, function () {
                    _this._log.info('MyVoteService: submitResponse success');
                    deferred.resolve('Your vote has been saved!');
                }, function (error) {
                    _this._log.error('MyVoteService: submitResponse error: ', error);
                    deferred.reject(error);
                });

                return deferred.promise;
            };

            MyVoteService.prototype.uploadImage = function (file) {
                var _this = this;
                var deferred = this._q.defer();
                var pollImageUrl = Globals.apiUrl + '/api/PollImage';

                if (!file) {
                    deferred.resolve(null);
                } else {
                    this._http.put(pollImageUrl, file, {
                        headers: { 'Content-Type': file.type }
                    }).then(function (result) {
                        _this._log.info('MyVoteService: uploadImage success: ', result);
                        deferred.resolve(result.data.imageUrl);
                    }, function (error) {
                        _this._log.error('MyVoteService: uploadImage error: ', error);
                        deferred.reject(error);
                    });
                }
                return deferred.promise;
            };

            MyVoteService.prototype.getPollResponse = function (pollId, userId) {
                var _this = this;
                var deferred = this._q.defer();

                var response = (this._respondResource.get({ pollID: pollId, userID: userId }, function () {
                    _this._log.info('MyVoteService: getPollResponse success');
                    deferred.resolve(response);
                }, function (error) {
                    _this._log.error('MyVoteService: getPollResponse error: ', error);
                    deferred.reject(error);
                }));

                return deferred.promise;
            };

            MyVoteService.prototype.getPollResult = function (pollId) {
                var _this = this;
                var deferred = this._q.defer();

                var result = (this._resultResource.get({ pollID: pollId }, function () {
                    _this._log.info('MyVoteService: getPollResult success');
                    deferred.resolve(result);
                }, function (error) {
                    _this._log.error('MyVoteService: getPollResponse error: ', error);
                    deferred.reject(error);
                }));

                return deferred.promise;
            };

            MyVoteService.prototype.submitComment = function (newComment) {
                var _this = this;
                var deferred = this._q.defer();

                this._commentResource.save(newComment, function (result) {
                    _this._log.info('MyVoteService: submitComment success');
                    deferred.resolve(result);
                }, function (error) {
                    _this._log.error('MyVoteService: submitComment error:', error);
                    deferred.reject(error);
                });

                return deferred.promise;
            };
            MyVoteService.$inject = ['$q', '$log', '$resource', '$http'];
            return MyVoteService;
        })();
        Services.MyVoteService = MyVoteService;
        MyVote.App.service('myVoteService', MyVoteService);
    })(MyVote.Services || (MyVote.Services = {}));
    var Services = MyVote.Services;
})(MyVote || (MyVote = {}));
