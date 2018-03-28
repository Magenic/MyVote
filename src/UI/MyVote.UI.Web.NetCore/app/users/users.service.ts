import { Injectable } from "@angular/core";
import { Http, Response, Headers } from "@angular/http";
import { Logger } from 'angular2-logger/core';
//import { Observable } from "rxjs";  //ctrl-click to see Rx.d.ts
import { Observable } from "rxjs/Observable";
//import { map } from "rxjs/operator/map";
//import { _catch } from "rxjs/operator/catch";
//import { toPromise } from "rxjs/operator/toPromise";

//import 'rxjs/add/operator/map';
//import 'rxjs/add/operator/catch';
//import 'rxjs/add/operator/toPromise';
import UtilityService = MyVote.UtilityService;

//The @Injectable decorator will attach some dependency injection metadata to the class, letting Angular know about its dependencies
@Injectable()
export class UsersService {

    constructor(private http: Http, private logger: Logger) { }

    public getUser(userProfileId: string): Promise<MyVote.Services.AppServer.Models.User> {

        let headers = new Headers();
        headers.append('Authorization', 'Bearer ' + Globals.zumoUserKey);   

        return new Promise((resolve, reject) => {
            return (this.http
                .get(`${Globals.apiUrl}/api/User/${userProfileId}`, { headers: headers }) //Template literals `` ES6
                .map((response: Response) => {
                    let user = <MyVote.Services.AppServer.Models.User>response.json();
                    this.logger.info('MyVoteService: getUser success');
                    resolve(user);
                })
                .catch((error: any) => {
                    let message = error.data.Message || error.data;
                    if (message.indexOf('Sequence contains no elements') !== -1) {
                        resolve(null);
                    } else {
                        reject('Error retrieving profile: ' + UtilityService.formatError(error, 'unknown error.'));
                    }
                    return Observable.throw(error);
                })
                .toPromise()) as Promise<any>;



            //var user = this._userResource.get(
            //    { userProfileId: userProfileId },
            //    () => {
            //        //this._log.info('MyVoteService: getUser success');
            //        resolve(user);
            //    },
            //    error => {
            //        //this._log.error('MyVoteService: getUser error: ', error);
            //        var message = error.data.Message || error.data;
            //        if (message.indexOf('Sequence contains no elements') !== -1) {
            //            resolve(null);
            //        } else {
            //            reject('Error retrieving profile: ' + UtilityService.formatError(error, 'unknown error.'));
            //        }
            //    });

        });
    }

    //Todo: Implement
    //public saveUser(user: MyVote.Services.AppServer.Models.User): Promise<MyVote.Services.AppServer.Models.User> {

    //    let headers = new Headers();
    //    headers.append('Authorization', 'Bearer ' + Globals.zumoUserKey);

    //    return new Promise((resolve, reject) => {
    //        return (this.http
    //            .post(`${Globals.apiUrl}/api/User`, user, { headers: headers }) //Template literals `` ES6
    //            .subscribe()
    //            //.catch((error: any) => {
    //            //    let message = error.data.Message || error.data;
    //            //    let reason = message.indexOf('Cannot insert duplicate') !== -1
    //            //        ? 'You have already registered.'
    //            //        : 'Error saving profile: ' + error.data;                                                            
    //            //    reject(reason);                    
    //            //    return Observable.throw(error);
    //            //})
    //            .toPromise()) as Promise<any>;


    //        //var savedUser = this._userResource.save(user,
    //        //    () => {
    //        //        //this._log.info('MyVoteService: saveUser success');
    //        //        resolve(savedUser);
    //        //    },
    //        //    error => {
    //        //        //this._log.error('MyVoteService: saveUser error: ', error);
    //        //        var message = error.data.Message || error.data;
    //        //        var reason = message.indexOf('Cannot insert duplicate') !== -1
    //        //            ? 'You have already registered.'
    //        //            : 'Error saving profile: ' + error.data;
    //        //        reject(reason);
    //        //    });
    //    });

    //}

}