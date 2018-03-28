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

        public getUser(userProfileId: string): Promise<MyVote.Services.AppServer.Models.User> {

            return new Promise((resolve, reject) => {

                var user = this._userResource.get(
                    { userProfileId: userProfileId },
                    () => {
                        //this._log.info('MyVoteService: getUser success');
                        resolve(user);
                    },
                    error => {
                        //this._log.error('MyVoteService: getUser error: ', error);
                        var message = error.data.Message || error.data;
                        if (message.indexOf('Sequence contains no elements') !== -1) {
                            resolve(null);
                        } else {
                            reject('Error retrieving profile: ' + UtilityService.formatError(error, 'unknown error.'));
                        }
                    });

            });
        }

        public saveUser(user: MyVote.Services.AppServer.Models.User): Promise<MyVote.Services.AppServer.Models.User> {

            return new Promise((resolve, reject) => {
                var savedUser = this._userResource.save(user,
                    () => {
                        //this._log.info('MyVoteService: saveUser success');
                        resolve(savedUser);
                    },
                    error => {
                        //this._log.error('MyVoteService: saveUser error: ', error);
                        var message = error.data.Message || error.data;
                        var reason = message.indexOf('Cannot insert duplicate') !== -1
                            ? 'You have already registered.'
                            : 'Error saving profile: ' + error.data;
                        reject(reason);
                    });
            });
        }

        public getPolls(filterBy: string): Promise<MyVote.Services.AppServer.Models.PollSummary[][]> {

            return new Promise((resolve, reject) => {
                var polls = (this._pollResource.query(
                    { filterBy: filterBy },
                    () => {
                        //this._log.info('MyVoteService: getPolls success');

                        if ((<any>polls).length == 0) {
                            resolve(null);
                            return;
                        };

                        let pollGroups: Promise<MyVote.Services.AppServer.Models.PollSummary[][]>;
                        for (var i = 0; i < (<any>polls).length; i++) {
                            var poll: MyVote.Services.AppServer.Models.PollSummary = polls[i];
                            if (!(poll.Category in pollGroups))
                                pollGroups[poll.Category] = [];
                            pollGroups[poll.Category].push(poll);
                        }
                        resolve(pollGroups);
                    },
                    error => {
                        //this._log.error('MyVoteService: getPolls error: ', error);
                        reject(error);
                    }));
            });


        }

        public getPoll(id: number): Promise<MyVote.Services.AppServer.Models.Poll> {

            return new Promise((resolve, reject) => {
                var poll = (this._pollResource.get(
                    { id: id },
                    () => {
                        //this._log.info('MyVoteService: getPoll success');
                        resolve(poll);
                    },
                    error => {
                        //this._log.error('MyVoteService: getPoll error: ', error);
                        reject(error);
                    }));
            });
     
        }

        public savePoll(poll: AppServer.Models.Poll): Promise<MyVote.Services.AppServer.Models.Poll> {

            return new Promise((resolve, reject) => {
                var newPoll = this._pollResource.save(
                    poll,
                    () => {
                        //this._log.info('MyVoteService: savePoll success');
                        resolve(newPoll);
                    },
                    error => {
                        //this._log.error('MyVoteService: savePoll error: ', error);
                        reject(error);
                    });
            });

        }

        public deletePoll(pollId: number): Promise<string> {

            return new Promise((resolve, reject) => {
                this._pollResource.delete(
                    { id: pollId },
                    () => {
                        //this._log.info('MyVoteService: deletePoll success');
                        resolve('The poll was deleted.');
                    },
                    error => {
                        //this._log.error('MyVoteService: deletePoll error: ', error);
                        reject(error);
                    });
            });

        }

        public submitResponse(response: MyVote.Services.AppServer.Models.PollResponse): Promise<string> {

            return new Promise((resolve, reject) => {
                this._respondResource.save(
                    response,
                    () => {
                        //this._log.info('MyVoteService: submitResponse success');
                        resolve('Your vote has been saved!');
                    },
                    error => {
                        //this._log.error('MyVoteService: submitResponse error: ', error);
                        reject(error);
                    });
            });
        }
        
        public uploadImage(file: File): Promise<string> {

            if (!file) {
                return new Promise((resolve, reject) => {
                    resolve(null);
                });
            }

            return new Promise((resolve: (value?: string | PromiseLike<string>) => void, reject) => {

                var pollImageUrl = Globals.apiUrl + '/api/PollImage';
                var formdata = new FormData();
                formdata.append('file', file);

                //this._http.post(pollImageUrl, formdata, {
                //    transformRequest: angular.identity,
                //    headers: { 'Content-Type': undefined }
                ////}).then((result: { data: { imageUrl: string } }): any => {
                //    }).then((result: { data: { imageUrl: string } }): any => {
                //    //this._log.info('MyVoteService: uploadImage success: ', result);
                //    resolve(result.data.imageUrl);
                //},
                //    error => {
                //        //this._log.error('MyVoteService: uploadImage error: ', error);
                //        reject(error);
                //    });

                this._http.post(pollImageUrl, formdata, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                    //}).then((result: { data: { imageUrl: string } }): any => {
                //}).then((response: { data: { imageUrl: string } }) => {
                }).then((response: any): void => {
                    //this._log.info('MyVoteService: uploadImage success: ', result);
                    if (response.data && response.data.imageUrl) {
                        resolve(response.data.imageUrl);
                    }                    
                },
                error => {
                    //this._log.error('MyVoteService: uploadImage error: ', error);
                    reject(error);
                });


            });

        }

        public getPollResponse(pollId: number, userId: number): Promise<MyVote.Services.AppServer.Models.PollInfo> {

            return new Promise((resolve, reject) => {
                var response = (this._respondResource.get(
                    { pollID: pollId, userID: userId },
                    () => {
                        //this._log.info('MyVoteService: getPollResponse success');
                        resolve(response);
                    },
                    error => {
                        //this._log.error('MyVoteService: getPollResponse error: ', error);
                        reject(error);
                    }));
            });

        }

        public getPollResult(pollId: number): Promise<MyVote.Services.AppServer.Models.PollResult> {

            //You can also access the raw $http promise via the $promise property on the object returned
            return new Promise((resolve, reject) => {
                this._resultResource.get({ pollID: pollId }).$promise
                    .then((result) => {
                        //this._log.info('MyVoteService: getPollResult success');
                        resolve(result);
                    }, (error) => {
                        //this._log.error('MyVoteService: getPollResponse error: ', error);
                        reject(error);
                    });
            });
        }

        public submitComment(newComment: any): Promise<any> {

            return new Promise((resolve, reject) => {
                this._commentResource.save(
                    newComment,
                    result => {
                        //this._log.info('MyVoteService: submitComment success');
                        resolve(result);
                    },
                    error => {
                        //this._log.error('MyVoteService: submitComment error:', error);
                        reject(error);
                    });
            });
        }
    }
    MyVote.App.service('myVoteService', MyVoteService);
}