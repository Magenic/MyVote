/// <reference path="../_refs.ts" />

module MyVote.Services {
    'use strict';

    // todo: probably split into service per resource
    export class MyVoteService {
        public static $inject = ['$q', '$log', '$resource', '$http'];

        private _apiResourceUrl: string;
        private _q: ng.IQService;
        private _log: ng.ILogService;
        private _http: ng.IHttpService;

        private _userResource: ng.resource.IResourceClass<any>;
        private _pollResource: ng.resource.IResourceClass<any>;
        private _respondResource: ng.resource.IResourceClass<any>;
        private _resultResource: ng.resource.IResourceClass<any>;
        private _commentResource: ng.resource.IResourceClass<any>;

        constructor($q: ng.IQService, $log: ng.ILogService, $resource: ng.resource.IResourceService, $http: ng.IHttpService) {
            this._apiResourceUrl = Globals.apiUrl.replace(/:([^\/])/, '\\:$1');
            
            this._q = $q;
            this._log = $log;
            this._http = $http;

            //$resource: The returned resource object has action methods which provide high-level behaviors 
            //without the need to interact with the low level $http service.
            this._userResource = $resource(this._apiResourceUrl + '/api/User/:userProfileId', {}, {
                'get': { method: 'GET' },
                'save': { method: 'POST' }
            });
            this._pollResource = $resource(this._apiResourceUrl + '/api/Poll/:id', {}, {
                'query': { method: 'GET', isArray: true, url: this._apiResourceUrl + '/api/poll/filter/:filterBy' },
                'get': { method: 'GET' },
                'save': { method: 'PUT' },
                'delete': { method: 'DELETE' }
            });            
            this._respondResource = $resource(this._apiResourceUrl + '/api/Respond/:pollID/:userID', {}, {
                'get': { method: 'GET' },
                'save': { method: 'PUT' }
            });
            this._resultResource = $resource(this._apiResourceUrl + '/api/PollResult/:pollID', {}, {
                'get': { method: 'GET' }
            });
            this._commentResource = $resource(this._apiResourceUrl + '/api/PollComment', {}, {
                'save': { method: 'PUT' }
            });
        }

        public getUser(userProfileId: string): ng.IPromise<MyVote.Services.AppServer.Models.User> {
            var deferred = this._q.defer();

            var user = this._userResource.get(
                { userProfileId: userProfileId },
                () => {
                    this._log.info('MyVoteService: getUser success');
                    deferred.resolve(user);
                },
                error => {
                    this._log.error('MyVoteService: getUser error: ', error);
                    var message = error.data.Message || error.data;
                    if (message.indexOf('Sequence contains no elements') !== -1) {
                        deferred.resolve(null);
                    }
                    else {
                        deferred.reject('Error retrieving profile: ' + UtilityService.formatError(error, 'unknown error.'));
                    }
                });

            return deferred.promise;
        }

        public saveUser(user: MyVote.Services.AppServer.Models.User): ng.IPromise<MyVote.Services.AppServer.Models.User> {
            var deferred = this._q.defer();

            var savedUser = this._userResource.save(user,
                () => {
                    this._log.info('MyVoteService: saveUser success');
                    deferred.resolve(savedUser);
                },
                error => {
                    this._log.error('MyVoteService: saveUser error: ', error);
                    var message = error.data.Message || error.data;
                    var reason = message.indexOf('Cannot insert duplicate') !== -1
                        ? 'You have already registered.'
                        : 'Error saving profile: ' + error.data;
                    deferred.reject(reason);
                });

            return deferred.promise;
        }

        public getPolls(filterBy: string): ng.IPromise<MyVote.Services.AppServer.Models.PollSummary[][]> {
            var deferred = this._q.defer();

            var polls = (this._pollResource.query(
                { filterBy: filterBy },
                () => {
                    this._log.info('MyVoteService: getPolls success');

                    if ((<any>polls).length == 0) {
                        deferred.resolve(null);
                        return;
                    };

                    var pollGroups =  {};
                    for (var i = 0; i < (<any>polls).length; i++) {
                        var poll: MyVote.Services.AppServer.Models.PollSummary = polls[i];
                        if (!(poll.Category in pollGroups))
                            pollGroups[poll.Category] = [];
                        pollGroups[poll.Category].push(poll);
                    }
                    deferred.resolve(pollGroups);
                },
                error => {
                    this._log.error('MyVoteService: getPolls error: ', error);
                    deferred.reject(error);
                }));

            return deferred.promise;
        }

        public getPoll(id: number): ng.IPromise<MyVote.Services.AppServer.Models.Poll> {
            var deferred = this._q.defer();

            var poll = (this._pollResource.get(
                { id: id },
                () => {
                    this._log.info('MyVoteService: getPoll success');
                    deferred.resolve(poll);
                },
                error => {
                    this._log.error('MyVoteService: getPoll error: ', error);
                    deferred.reject(error);
                }));

            return deferred.promise;
        }

        public savePoll(poll: AppServer.Models.Poll): ng.IPromise<MyVote.Services.AppServer.Models.Poll> {
            var deferred = this._q.defer();

            var newPoll = this._pollResource.save(
                poll,
                () => {
                    this._log.info('MyVoteService: savePoll success');
                    deferred.resolve(newPoll);
                },
                error => {
                    this._log.error('MyVoteService: savePoll error: ', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        }

        public deletePoll(pollId: number): ng.IPromise<string> {
            var deferred = this._q.defer();

            this._pollResource.delete(
                { id: pollId },
                () => {
                    this._log.info('MyVoteService: deletePoll success');
                    deferred.resolve('The poll was deleted.');
                },
                error => {
                    this._log.error('MyVoteService: deletePoll error: ', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        }

        public submitResponse(response: MyVote.Services.AppServer.Models.PollResponse): ng.IPromise<string> {
            var deferred = this._q.defer();

            this._respondResource.save(
                response,
                () => {
                    this._log.info('MyVoteService: submitResponse success');
                    deferred.resolve('Your vote has been saved!');
                },
                error => {
                    this._log.error('MyVoteService: submitResponse error: ', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        }
        
        public uploadImage(file: File): ng.IPromise<string> {            
            var deferred = this._q.defer();
            var pollImageUrl = Globals.apiUrl + '/api/PollImage';
            var formdata = new FormData();
            formdata.append('file', file);

            if (!file) {
                deferred.resolve(null);
            } else {
                this._http.post(pollImageUrl, formdata, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).then((result: { data: { imageUrl: string } }): any => {
                        this._log.info('MyVoteService: uploadImage success: ', result);
                        deferred.resolve(result.data.imageUrl);
                    },
                    error => {
                        this._log.error('MyVoteService: uploadImage error: ', error);
                        deferred.reject(error);
                    });
            }
            return deferred.promise;
        }

        public getPollResponse(pollId: number, userId: number): ng.IPromise<MyVote.Services.AppServer.Models.PollInfo> {
            var deferred = this._q.defer();

            var response = (this._respondResource.get(
                { pollID: pollId, userID: userId },
                () => {
                    this._log.info('MyVoteService: getPollResponse success');
                    deferred.resolve(response);
                },
                error => {
                    this._log.error('MyVoteService: getPollResponse error: ', error);
                    deferred.reject(error);
                }));
            
            return deferred.promise;
        }

        public getPollResult(pollId: number): ng.IPromise<MyVote.Services.AppServer.Models.PollResult> {
            var deferred = this._q.defer();

            //You can also access the raw $http promise via the $promise property on the object returned
            this._resultResource.get({ pollID: pollId }).$promise
                .then((result) => {
                    this._log.info('MyVoteService: getPollResult success');
                    deferred.resolve(result);
                }, (error) => {
                    this._log.error('MyVoteService: getPollResponse error: ', error);
                    deferred.reject(error);
                });

            //var result = (this._resultResource.get(
            //    { pollID: pollId },
            //    () => {
            //        this._log.info('MyVoteService: getPollResult success');
            //        deferred.resolve(result);
            //    },
            //    error => {
            //        this._log.error('MyVoteService: getPollResponse error: ', error);
            //        deferred.reject(error);
            //    }));

            return deferred.promise;
        }

        public submitComment(newComment: any): ng.IPromise<any> {
            var deferred = this._q.defer();

            this._commentResource.save(
                newComment,
                result => {
                    this._log.info('MyVoteService: submitComment success');
                    deferred.resolve(result);
                },
                error => {
                    this._log.error('MyVoteService: submitComment error:', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        }
    }
    MyVote.App.service('myVoteService', MyVoteService);
}