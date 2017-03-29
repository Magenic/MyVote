import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { Logger } from 'angular2-logger/core';
import 'rxjs/Rx';

//The @Injectable decorator will attach some dependency injection metadata to the class, letting Angular 2 know about its dependencies
@Injectable()
export class PollsService {

    //private _q: angular.IAngularStatic.IQService;

    constructor(private http: Http, private logger: Logger) { }

    public deletePoll(pollId: number): ng.IPromise<string> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + Globals.zumoUserKey);     

        return this.http
            .delete(`${Globals.apiUrl}/api/Poll/${pollId}`, {  headers: headers })
            .map(() => {
                return 'The poll was deleted';
            })
            .catch((error: any) => {
                this.logger.error('MyVoteService: deletePoll error: ', error);
                return error;
            })
            .toPromise() as ng.IPromise<any>;


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

    public getPoll(id: number): ng.IPromise<MyVote.Services.AppServer.Models.Poll> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + Globals.zumoUserKey);     

        return (this.http
            .get(`${Globals.apiUrl}/api/Poll/${id}`, { headers: headers }) //Template literals `` ES6
            .map((response: Response) => {

                let poll = <MyVote.Services.AppServer.Models.Poll>response.json();
                this.logger.info('MyVoteService: getPoll success');
                return poll;
            })
            .catch((error: any) => {
                this.logger.error('MyVoteService: getPoll error: ', error);
                return error;
            })
            .toPromise()) as ng.IPromise<any>;

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

    public getPolls(filterBy: string): ng.IPromise<MyVote.Services.AppServer.Models.PollSummary[][]> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + Globals.zumoUserKey);

        let params: URLSearchParams = new URLSearchParams();
        params.set('filterBy', filterBy);

        return (this.http
            .get(Globals.apiUrl + '/api/Poll', { search: params, headers: headers })
            .map((response: Response) => {
                
                let polls = <any>response.json();

                if (!polls || (<any>polls).length === 0) {
                    return null;
                };

                this.logger.info('MyVoteService: getPolls success');

                var pollGroups = {};
                for (var i = 0; i < (<any>polls).length; i++) {
                    var poll: MyVote.Services.AppServer.Models.PollSummary = polls[i];
                    if (!(poll.Category in pollGroups))
                        pollGroups[poll.Category] = [];
                    pollGroups[poll.Category].push(poll);
                }
                return pollGroups;
            })
            .catch((error: any) => {
                this.logger.error('MyVoteService: getPolls error: ', error);
                return error;
            }).toPromise()) as ng.IPromise<any>;


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

    public getPollResponse(pollId: number, userId: number): ng.IPromise<MyVote.Services.AppServer.Models.PollInfo> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + Globals.zumoUserKey);

        return (this.http
            .get(`${Globals.apiUrl}/api/Respond/${pollId}/${userId}`, { headers: headers })  //Template literals `` ES6
            .map((response: Response) => {

                let polls = <MyVote.Services.AppServer.Models.PollInfo>response.json();
                this.logger.info('MyVoteService: getPollResponse success');
                return polls;
            })
            .catch((error: any) => {
                this.logger.error('MyVoteService: getPollResponse error: ', error);
                return error;
            }).toPromise()) as ng.IPromise<any>;

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

    public submitResponse(pollResponse: MyVote.Services.AppServer.Models.PollResponse): ng.IPromise<string> {


        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + Globals.zumoUserKey);
        headers.append('Content-Type', 'application/json');

        return (this.http
            .put(`${Globals.apiUrl}/api/Respond`, JSON.stringify(pollResponse), { headers: headers })
            .map((response: Response) => {
                if (response.text()) {
                    let poll = <MyVote.Services.AppServer.Models.Poll>response.json();
                    this.logger.info('MyVoteService: getPoll success');
                    return poll;
                }
            })
            .catch((error: any) => {
                this.logger.error('MyVoteService: getPoll error: ', error);
                return error;
            })
            .toPromise()) as ng.IPromise<any>;


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