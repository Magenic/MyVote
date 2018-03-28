import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { Logger } from 'angular2-logger/core';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/toPromise';
import { Observable } from "rxjs/Observable";
import { AppSettings } from "../index";

//The @Injectable decorator will attach some dependency injection metadata to the class, letting Angular 2 know about its dependencies
@Injectable()
export class PollsService {

    //private _q: angular.IAngularStatic.IQService;
    private zumoUserKey: string;

    constructor(private http: Http, private logger: Logger) {
        var savedUserJSON = window.localStorage[AppSettings.zumoUserKey];
        if (savedUserJSON) {
            var savedUser = JSON.parse(savedUserJSON);
            if (savedUser.hasOwnProperty('userId') && savedUser.hasOwnProperty('mobileServiceAuthenticationToken')) {
                this.zumoUserKey = savedUser.mobileServiceAuthenticationToken;
            }
        }
    }

    public deletePoll(pollId: number): Promise<string> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + this.zumoUserKey);     

        return this.http
            .delete(`${AppSettings.apiUrl}/api/Poll/${pollId}`, {  headers: headers })
            .map(() => {
                return 'The poll was deleted';
            })
            .catch((error: any) => {
                this.logger.error('MyVoteService: deletePoll error: ', error);
                return error;
            })
            .toPromise() as Promise<any>;


        //var deferred = this._q.defer();
        //this._pollResource.delete(
        //    { id: pollId },
        //    () => {
        //        this._log.info('MyVoteService: deletePoll success');
        //        deferred.resolve('The poll was deleted.');
        //    },
        //    error => {
        //        this._log.error('MyVoteService: deletePoll error: ', error);
        //        deferred.reject(error);
        //    });
        //return deferred.promise;
    }

    public getPoll(id: number): Promise<MyVote.Services.AppServer.Models.Poll> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + this.zumoUserKey);     

        return new Promise((resolve, reject) => {
            return (this.http
                .get(`${AppSettings.apiUrl}/api/Poll/${id}`, { headers: headers }) //Template literals `` ES6
                .map((response: Response) => {

                    let poll = <MyVote.Services.AppServer.Models.Poll>response.json();
                    this.logger.info('MyVoteService: getPoll success');
                    resolve(poll);
                })
                .catch((error: any) => {
                    this.logger.error('MyVoteService: getPoll error: ', error);
                    return Observable.throw(error);
                })
                .toPromise()) as Promise<any>;
        });



        //var deferred = this._q.defer();
        //var poll = (this._pollResource.get(
        //    { id: id },
        //    () => {
        //        this._log.info('MyVoteService: getPoll success');
        //        deferred.resolve(poll);
        //    },
        //    error => {
        //        this._log.error('MyVoteService: getPoll error: ', error);
        //        deferred.reject(error);
        //    }));
        //return deferred.promise;
    }

    public getPolls(filterBy: string): Promise<MyVote.Services.AppServer.Models.PollSummary[][]> {

        return new Promise((resolve, reject) => {
            let headers = new Headers();
            headers.append('Authorization', 'Bearer ' + this.zumoUserKey);

            let params: URLSearchParams = new URLSearchParams();
            params.set('filterBy', filterBy);

            return (this.http
                .get(AppSettings.apiUrl + '/api/Poll', { search: params, headers: headers })
                .map((response: Response) => {

                    let polls = <any>response.json();

                    if (!polls || (<any>polls).length === 0) {
                        return null;
                    };

                    this.logger.info('MyVoteService: getPolls success');

                    var pollGroups: MyVote.Services.AppServer.Models.PollSummary[][] = [];
                    for (var i = 0; i < (<any>polls).length; i++) {
                        var poll: MyVote.Services.AppServer.Models.PollSummary = polls[i];
                        if (!(poll.Category in pollGroups))
                            pollGroups[poll.Category] = [];
                        pollGroups[poll.Category].push(poll);
                    }
                    resolve(pollGroups);
                })
                .catch((error: any) => {
                    this.logger.error('MyVoteService: getPolls error: ', error);
                    return Observable.throw(error);
                }).toPromise()) as Promise<any>;
        });




        //var deferred = this._q.defer();
        //var polls = (this._pollResource.query(
        //    { filterBy: filterBy },
        //    () => {
        //        //this._log.info('MyVoteService: getPolls success');

        //        if ((<any>polls).length == 0) {
        //            deferred.resolve(null);
        //            return;
        //        };

        //        var pollGroups = {};
        //        for (var i = 0; i < (<any>polls).length; i++) {
        //            var poll: MyVote.Services.AppServer.Models.PollSummary = polls[i];
        //            if (!(poll.Category in pollGroups))
        //                pollGroups[poll.Category] = [];
        //            pollGroups[poll.Category].push(poll);
        //        }
        //        deferred.resolve(pollGroups);
        //    },
        //    error => {
        //        //this._log.error('MyVoteService: getPolls error: ', error);
        //        deferred.reject(error);
        //    }));
        //return deferred.promise;
    }

    public getPollResponse(pollId: number, userId: number): Promise<MyVote.Services.AppServer.Models.PollInfo> {

        return new Promise((resolve, reject) => {
            let headers = new Headers();
            headers.append('Authorization', 'Bearer ' + this.zumoUserKey);

            return (this.http
                .get(`${AppSettings.apiUrl}/api/Respond/${pollId}/${userId}`, { headers: headers })  //Template literals `` ES6
                .map((response: Response) => {

                    let polls = <MyVote.Services.AppServer.Models.PollInfo>response.json();
                    this.logger.info('MyVoteService: getPollResponse success');
                    resolve(polls);
                })
                .catch((error: any) => {
                    this.logger.error('MyVoteService: getPollResponse error: ', error);
                    return Observable.throw(error);
                }).toPromise()) as Promise<any>;
        });

        //var deferred = this._q.defer();
        //var response = (this._respondResource.get(
        //    { pollID: pollId, userID: userId },
        //    () => {
        //        this._log.info('MyVoteService: getPollResponse success');
        //        deferred.resolve(response);
        //    },
        //    error => {
        //        this._log.error('MyVoteService: getPollResponse error: ', error);
        //        deferred.reject(error);
        //    }));
        //return deferred.promise;
    }

    public submitResponse(pollResponse: MyVote.Services.AppServer.Models.PollResponse): Promise<string> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + this.zumoUserKey);
        headers.append('Content-Type', 'application/json');

        return new Promise((resolve, reject) => {
            return (this.http
                .put(`${AppSettings.apiUrl}/api/Respond`, JSON.stringify(pollResponse), { headers: headers })
                .map((response: Response) => {
                    if (response) {
                        this.logger.info('MyVoteService: getPoll success');
                        resolve('success');
                    }
                })
                .catch((error: any) => {
                    this.logger.error('MyVoteService: getPoll error: ', error);
                    return Observable.throw(error);
                })
                .toPromise()) as Promise<any>;
        });

        //var deferred = this._q.defer();
        //this._respondResource.save(
        //    response,
        //    () => {
        //        this._log.info('MyVoteService: submitResponse success');
        //        deferred.resolve('Your vote has been saved!');
        //    },
        //    error => {
        //        this._log.error('MyVoteService: submitResponse error: ', error);
        //        deferred.reject(error);
        //    });
        //return deferred.promise;
    }

}