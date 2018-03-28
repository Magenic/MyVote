declare var angular: angular.IAngularStatic;

import { OnInit, Injectable } from '@angular/core';
import { downgradeInjectable } from '@angular/upgrade/static';
import { HubConnection } from '@aspnet/signalr-client';
import { Observable } from "rxjs/Observable";
import { Observer } from "rxjs/Observer";
import 'rxjs/add/operator/share'

@Injectable()
export class SignalRService {

    public pollAddedObserver: Observer<string>;
    public pollAddedChanged$: Observable<any>;
    private _hubConnection: HubConnection;

    constructor() {

        this._hubConnection = new HubConnection('/myvotehub');

        this._hubConnection.on('pollAdded',
            (...msg: any[]) => {
                if (this.pollAddedObserver) {
                    this.pollAddedObserver.next(msg[0]);
                }
            });

        this._hubConnection.start()
            .then(() => {
                console.log('MyVoteHub connection started');
            })
            .catch(err => {
                console.log('Error while establishing connection');
            });

        this.pollAddedChanged$ = new Observable((observer: any) => this.pollAddedObserver = observer).share();
    }

    public addPollNotification(): void {
        //Invoking 'AddPollAsync' method defined in SignalR hub
        this._hubConnection.invoke('AddPollAsync');
    }

}

angular.module('MyVoteApp')
    .factory('ngSignalRService', downgradeInjectable(SignalRService));