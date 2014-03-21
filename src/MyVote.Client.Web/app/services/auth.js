/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Services) {
        'use strict';

        var AuthService = (function () {
            function AuthService($q, $rootScope, myVoteService) {
                this.providers = [
                    new MyVote.Models.AuthProvider('twitter', 'Twitter', '/Content/twitter.png', '#01abdf'),
                    new MyVote.Models.AuthProvider('facebook', 'Facebook', '/Content/fb.png', '#3d509f'),
                    new MyVote.Models.AuthProvider('microsoftaccount', 'Microsoft', '/Content/ms.png', '#fff'),
                    new MyVote.Models.AuthProvider('google', 'Google', '/Content/google.png', '#dc021e')
                ];
                this._q = $q;
                this._rootScope = $rootScope;
                this._myVoteService = myVoteService;
                this.userId = null;
                this.userName = null;

                var savedUserJSON = window.localStorage[AuthService._zumoUserKey];
                if (savedUserJSON) {
                    try  {
                        var savedUser = JSON.parse(savedUserJSON);
                        if (savedUser.hasOwnProperty('userId') && savedUser.hasOwnProperty('mobileServiceAuthenticationToken')) {
                            this._savedZumoUser = savedUser;
                            Globals.zumoUserKey = savedUser.mobileServiceAuthenticationToken;
                        } else {
                            window.localStorage.removeItem(AuthService._zumoUserKey);
                        }
                    } catch (e) {
                        window.localStorage.removeItem(AuthService._zumoUserKey);
                    }
                }
            }
            AuthService.prototype.foundSavedLogin = function () {
                return this._savedZumoUser != null;
            };

            AuthService.prototype.loadSavedLogin = function () {
                var _this = this;
                return this._myVoteService.getUser(this._savedZumoUser.userId).then(function (result) {
                    if (result) {
                        AuthService._client.currentUser = _this._savedZumoUser;
                        _this.userId = result.UserID;
                        _this.userName = result.UserName;
                        _this._rootScope.$emit(AuthService.LoginEvent);
                    }
                    return result;
                });
            };

            AuthService.prototype.login = function (provider) {
                var _this = this;
                this.logout();
                return AuthService._client.login(provider).then(function (result) {
                    var zumoUser = AuthService._client.currentUser;
                    _this._savedZumoUser = zumoUser;

                    var zumoUserJSON = JSON.stringify(zumoUser);
                    window.localStorage[AuthService._zumoUserKey] = zumoUserJSON;
                    Globals.zumoUserKey = result.mobileServiceAuthenticationToken;

                    return _this._myVoteService.getUser(zumoUser.userId);
                }).then(function (result) {
                    if (result) {
                        _this.userId = result.UserID;
                        _this.userName = result.UserName;
                        _this._rootScope.$emit(AuthService.LoginEvent);
                    }
                    return result;
                });
            };

            AuthService.prototype.logout = function () {
                this.userId = null;
                this.userName = null;
                this._savedZumoUser = null;
                Globals.zumoUserKey = null;
                window.localStorage.removeItem(AuthService._zumoUserKey);
                AuthService._client.logout();
            };

            AuthService.prototype.isLoggedIn = function () {
                return (this.userId != null);
            };

            AuthService.prototype.isRegistered = function () {
                return (this.isLoggedIn() && this.userId != null);
            };

            Object.defineProperty(AuthService.prototype, "profileId", {
                get: function () {
                    if (!AuthService._client.currentUser)
                        return null;
                    return AuthService._client.currentUser.userId;
                },
                enumerable: true,
                configurable: true
            });
            AuthService._zumoUserKey = 'ZUMO_TOKEN';

            AuthService._client = new WindowsAzure.MobileServiceClient('https://myvote.azure-mobile.net/', Globals.zumoKey);

            AuthService.LoginEvent = 'authService.login';

            AuthService.$inject = ['$q', '$rootScope', 'myVoteService'];
            return AuthService;
        })();
        Services.AuthService = AuthService;
        MyVote.App.service('authService', AuthService);
    })(MyVote.Services || (MyVote.Services = {}));
    var Services = MyVote.Services;
})(MyVote || (MyVote = {}));
