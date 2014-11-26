/// <reference path="../_refs.ts" />

module MyVote.Services {
    'use strict';

    export class AuthService {
        private static _zumoUserKey = 'ZUMO_TOKEN';

        private static _client: Microsoft.WindowsAzure.MobileServiceClient = new WindowsAzure.MobileServiceClient(
            'https://myvote.azure-mobile.net/', Globals.zumoKey);

        public static LoginEvent = 'authService.login';

        public static $inject = ['$q', '$rootScope', 'myVoteService'];

        public providers: Models.AuthProvider[] = [
            new Models.AuthProvider('twitter', 'Twitter', '/Content/twitter.png', '#01abdf'),
            new Models.AuthProvider('facebook', 'Facebook', '/Content/fb.png', '#3d509f'),
            new Models.AuthProvider('microsoftaccount', 'Microsoft', '/Content/ms.png', '#fff'),
            new Models.AuthProvider('google', 'Google', '/Content/google.png', '#dc021e')
        ];

        public userId: number;
        public userName: string;
        public desiredPath: string;

        private _isAuthError: boolean;

        private _q: ng.IQService;
        private _rootScope: ng.IRootScopeService;
        private _myVoteService: MyVoteService;
        
        private _savedZumoUser: Microsoft.WindowsAzure.User;

        constructor($q: ng.IQService, $rootScope: ng.IRootScopeService, myVoteService: MyVoteService) {
            this._q = $q;
            this._rootScope = $rootScope;
            this._myVoteService = myVoteService;
            this.userId = null;
            this.userName = null;

            var savedUserJSON = window.localStorage[AuthService._zumoUserKey];
            if (savedUserJSON) {
                try {
                    var savedUser = JSON.parse(savedUserJSON);
                    if (savedUser.hasOwnProperty('userId') && savedUser.hasOwnProperty('mobileServiceAuthenticationToken')) {
                        this._savedZumoUser = savedUser;
                        Globals.zumoUserKey = savedUser.mobileServiceAuthenticationToken;
                    } else {
                        window.localStorage.removeItem(AuthService._zumoUserKey);
                    }
                } catch(e) {
                    window.localStorage.removeItem(AuthService._zumoUserKey);
                }
            }
        }

        public foundSavedLogin(): boolean {
            return this._savedZumoUser != null;
        }

        public loadSavedLogin(): ng.IPromise<MyVote.AppServer.Models.User> {
            return this._myVoteService.getUser(this._savedZumoUser.userId)
                .then((result: MyVote.AppServer.Models.User) => {
                    if (result) {
                        AuthService._client.currentUser = this._savedZumoUser;
                        this.userId = result.UserID;
                        this.userName = result.UserName;
                        this._rootScope.$emit(AuthService.LoginEvent);
                    }
                    return result;
                }, (error) => {
                    //Indicate there was an Authorization error and a user was not returned
                    this._isAuthError = true;
                    //Remove any saved user credentials from storage as they were not valid anymore (i.e. expired)
                    window.localStorage.removeItem(AuthService._zumoUserKey);
                    return this._q.reject(error);
                }
            );
        }

        public login(provider: string): Microsoft.WindowsAzure.asyncPromise {
            this.logout();
            return AuthService._client.login(provider).then(
                result => {
                    var zumoUser = AuthService._client.currentUser;
                    this._savedZumoUser = zumoUser;

                    var zumoUserJSON = JSON.stringify(zumoUser);
                    window.localStorage[AuthService._zumoUserKey] = zumoUserJSON;
                    Globals.zumoUserKey = result.mobileServiceAuthenticationToken;

                    return this._myVoteService.getUser(zumoUser.userId);
                }).then((result: MyVote.AppServer.Models.User) => {
                    if (result) {                        
                        this.userId = result.UserID;
                        this.userName = result.UserName;
                        this._rootScope.$emit(AuthService.LoginEvent);
                    }
                    return result;
                });
        }

        public logout(): void {
            this.userId = null;
            this.userName = null;
            this._savedZumoUser = null;
            Globals.zumoUserKey = null;
            window.localStorage.removeItem(AuthService._zumoUserKey);
            AuthService._client.logout();
        }

        public isLoggedIn(): boolean {
            return (this.userId != null);
        }

        public isAuthError(): boolean {
            return this._isAuthError;
        }

        public isRegistered(): boolean {
            return (this.isLoggedIn() && this.userId != null);
        }

        public get profileId(): string {
            if (!AuthService._client.currentUser)
                return null;
            return AuthService._client.currentUser.userId;
        }
    }
    App.service('authService', AuthService);
}