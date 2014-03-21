
/// <reference path="../../MyVote.Client.Web/app/controllers/landing.js"/>

describe("new LandingCtrl", function () {
    var locationMock = function(initialPath) {
        var pathStr = initialPath || '';
        this.path = function(pathArg) {
            return pathArg ? pathStr = pathArg : pathStr;
        };
    };
    var $location;
    
    beforeEach(inject(function($injector) {
        $location = $injector.get('$location');
        var loc = new locationMock();
        spyOn($location, 'path').and.callFake(loc.path);
    }));

    it("should have a LandingCtrl controller", function () {
        expect(MyVote.Controllers.LandingCtrl).not.toEqual(null);
    });

    it("should have correct default properties", inject(function ($rootScope, $controller) {
        var mockAuthService = {
            foundSavedLogin: function () { return false; },
            providers: [{ id: 'providerId', name: 'providerName' }]
        };
        
        var $scope = $rootScope.$new();
        var ctrl = $controller(
            'MyVote.Controllers.LandingCtrl',
            { $scope: $scope, authService: mockAuthService });

        expect(ctrl).not.toEqual(null);
        expect(ctrl.$scope).toBe($scope);
        expect($scope.providers).toBe(mockAuthService.providers);
        expect($scope.busyMessage).toBeNull();
        expect($scope.busy).toEqual(false);
    }));

    it("should redirect upon loading a saved login from authService", inject(function($rootScope, $controller, $q) {
        var mockAuthService = {
            foundSavedLogin: function() { return true; },
            providers: [{ id: 'providerId', name: 'providerName' }],
            loadSavedLogin: function() {
                var d = $q.defer();
                d.resolve('fakeLogin');
                return d.promise;
            },
            desiredPath: '/testRoute'
        };

        var $scope = $rootScope.$new();
        var ctrl = $controller('MyVote.Controllers.LandingCtrl', {
            $scope: $scope,
            authService: mockAuthService
        });
        $rootScope.$apply();
        
        expect(ctrl.$scope).toBe($scope);
        expect($scope.busyMessage).not.toBeNull();
        expect($scope.busy).toEqual(true);
        expect(mockAuthService.desiredPath).toBeNull();
        expect($location.path()).toEqual('/testRoute');
    }));
    
    it("should not redirect when saved login is null from authService", inject(function ($rootScope, $controller, $q) {
        var mockAuthService = {
            foundSavedLogin: function () { return true; },
            providers: [{ id: 'providerId', name: 'providerName' }],
            loadSavedLogin: function () {
                var d = $q.defer();
                d.resolve(null);
                return d.promise;
            },
            desiredPath: '/testRoute'
        };

        var $scope = $rootScope.$new();
        var ctrl = $controller('MyVote.Controllers.LandingCtrl', {
            $scope: $scope,
            authService: mockAuthService
        });
        $rootScope.$apply();

        expect(ctrl.$scope).toBe($scope);
        expect($scope.providers).toBe(mockAuthService.providers);
        expect($scope.busyMessage).toBeNull();
        expect($scope.busy).toEqual(false);
    }));
    
    it("should display an error when authService.loadSavedLogin reports one", inject(function ($rootScope, $controller, $q) {
        var mockAuthService = {
            foundSavedLogin: function () { return true; },
            providers: [{ id: 'providerId', name: 'providerName' }],
            loadSavedLogin: function () {
                var d = $q.defer();
                d.reject('error loading saved login');
                return d.promise;
            },
            desiredPath: '/testRoute'
        };

        var $scope = $rootScope.$new();
        var ctrl = $controller('MyVote.Controllers.LandingCtrl', {
            $scope: $scope,
            authService: mockAuthService
        });
        $rootScope.$apply();

        expect(ctrl.$scope).toBe($scope);
        expect($scope.busyMessage).toBeNull();
        expect($scope.busy).toEqual(false);
        expect($scope.errorMessage).toEqual('error loading saved login');
    }));

    it("should redirect to polls after registered login", inject(function ($rootScope, $controller, $q) {
        var mockProvider = { id: 'mockProvider', name: 'Mock Provider' };
        var mockAuthService = {
            foundSavedLogin: function() { return false; },
            providers: [mockProvider],
            desiredPath: null,
            _isRegistered: false,
            isRegistered: function () { return this._isRegistered; },
            login: function(providerId) {
                var d = $q.defer();
                this._isRegistered = true;
                d.resolve(true);
                return d.promise;
            }
        };

        var $scope = $rootScope.$new();
        var originalApply = $scope.$apply;
        $scope.$apply = function(f) {
            if ($scope.$$phase) {
                f();
            } else {
                originalApply(f);
            }
        };
        
        var ctrl = $controller('MyVote.Controllers.LandingCtrl', {
            $scope: $scope,
            authService: mockAuthService
        });
        expect(ctrl.$scope).toBe($scope);
        expect($scope.providers).toBe(mockAuthService.providers);
        expect($scope.busyMessage).toBeNull();
        expect($scope.busy).toEqual(false);

        $scope.login(mockProvider);

        expect($scope.busyMessage).not.toBeFalsy();
        expect($scope.busy).toEqual(true);
        expect($scope.errorMessage).toBeNull();
        
        $rootScope.$apply();
        expect(mockAuthService.isRegistered()).toEqual(true);
        expect($location.path()).toEqual('/polls');
    }));
    
    it("should display login error", inject(function ($rootScope, $controller, $q) {
        var mockProvider = { id: 'mockProvider', name: 'Mock Provider' };
        var mockAuthService = {
            foundSavedLogin: function () { return false; },
            providers: [mockProvider],
            desiredPath: null,
            _isRegistered: false,
            isRegistered: function () { return this._isRegistered; },
            login: function (providerId) {
                var d = $q.defer();
                d.reject('ERROR');
                return d.promise;
            }
        };

        var $scope = $rootScope.$new();
        var originalApply = $scope.$apply;
        $scope.$apply = function (f) {
            if ($scope.$$phase) {
                f();
            } else {
                originalApply(f);
            }
        };

        var ctrl = $controller('MyVote.Controllers.LandingCtrl', {
            $scope: $scope,
            authService: mockAuthService
        });

        $scope.login(mockProvider);

        $rootScope.$apply();
        expect(ctrl.$scope).toBe($scope);
        expect($scope.busyMessage).toBeNull();
        expect($scope.busy).toEqual(false);
        expect($scope.errorMessage).toEqual('ERROR');
    }));

    it("should not call on login authservice when busy", inject(function($rootScope, $controller) {
        var mockProvider = { id: 'mockProvider', name: 'Mock Provider' };
        var mockAuthService = {
            foundSavedLogin: function () { return false; },
            providers: [mockProvider],
            login: function(providerId) {}
        };
        spyOn(mockAuthService, 'login');
        
        var $scope = $rootScope.$new();
        var ctrl = $controller('MyVote.Controllers.LandingCtrl', {
            $scope: $scope,
            authService: mockAuthService
        });

        $scope.busy = true;
        $scope.login(mockProvider);
        expect(mockAuthService.login).not.toHaveBeenCalled();
    }));
});
