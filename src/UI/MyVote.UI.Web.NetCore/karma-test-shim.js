//Error.stackTraceLimit = 0;
// Use 'Infinity' get full stacktrace output. This helps with debugging unit testing errors more easily from the output
Error.stackTraceLimit = Infinity;

jasmine.DEFAULT_TIMEOUT_INTERVAL = 1000;

// (1)
var builtPaths = (__karma__.config.builtPaths || ['app/'])
    .map(function (p) { return '/base/' + p; });

// (2) - Here we are using this hook to tell Karma not to start the tests. We will manually trigger the tests after SystemJS has loaded all the modules
__karma__.loaded = function () { };

// (3) - Helper Function
function isJsFile(path) {
    return path.slice(-3) == '.js';
}

// (4) - Helper Function
function isSpecFile(path) {
    return /\.spec\.(.*\.)?js$/.test(path);
}

// (5) - Helper Function
function isBuiltFile(path) {
    return isJsFile(path) &&
        builtPaths.reduce(function (keep, bp) {
            return keep || (path.substr(0, bp.length) === bp);
        }, false);
}

// (6) - Get all spec files that are part of our application
//Grabs all the files in our Karma config 'files' array, and then use the helper functions to filter them out
//Note: The spec files already import the application files, so we don’t need to import them manually
//cont. they will be imported transitively when SystemJS sees the 'require' statements
var allSpecFiles = Object.keys(window.__karma__.files)
    .filter(isSpecFile)
    .filter(isBuiltFile);

// (7) - Set the baseURL in the SystemJS configuration. Karma adds the base path base to the webserver URL, SystemJS will use the same base URL when trying to load modules
SystemJS.config({
    baseURL: 'base',

    //NOTE: Any testing modules used/imported MUST be added here or a 404 will result when running the unit tests
    map: {
        '@angular/core/testing': 'npm:@angular/core/bundles/core-testing.umd.js',
        '@angular/common/testing': 'npm:@angular/common/bundles/common-testing.umd.js',
        '@angular/compiler/testing': 'npm:@angular/compiler/bundles/compiler-testing.umd.js',
        '@angular/platform-browser/testing': 'npm:@angular/platform-browser/bundles/platform-browser-testing.umd.js',
        '@angular/platform-browser-dynamic/testing': 'npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic-testing.umd.js',
        '@angular/http/testing': 'npm:@angular/http/bundles/http-testing.umd.js',
        '@angular/router/testing': 'npm:@angular/router/bundles/router-testing.umd.js',
        '@angular/forms/testing': 'npm:@angular/forms/bundles/forms-testing.umd.js'
        //'@angular/core/testing': 'wwwroot/libs/core-testing.umd.js',
        //'@angular/common/testing': 'wwwroot/libs/common-testing.umd.js',
        //'@angular/compiler/testing': 'wwwroot/libs/compiler-testing.umd.js',
        //'@angular/platform-browser/testing': 'wwwroot/libs/platform-browser-testing.umd.js',
        //'@angular/platform-browser-dynamic/testing': 'wwwroot/libs/platform-browser-dynamic-testing.umd.js',
        //'@angular/http/testing': 'wwwroot/libs/http-testing.umd.js',
        //'@angular/router/testing': 'wwwroot/libs/router-testing.umd.js',
        //'@angular/forms/testing': 'wwwroot/libs/forms-testing.umd.js'
    }
});

// (8) - Load the systemjs.config.js file
System.import('systemjs.config.js')
    .then(initTestBed)
    .then(initTesting);

function initTestBed() {
    return Promise.all([
        System.import('@angular/core/testing'),
        System.import('@angular/platform-browser-dynamic/testing')
    ])
        .then(function (providers) {
            var coreTesting = providers[0];
            var browserTesting = providers[1];

            coreTesting.TestBed.initTestEnvironment(
                browserTesting.BrowserDynamicTestingModule,
                browserTesting.platformBrowserDynamicTesting());
        });
}

// (9) - use the helper function (that grabbed all the spec files), we iterate through them, importing all of them with System.import
//After they are all resolved, we manually start Karma by passing the karma start function to the Promise then
function initTesting() {
    return Promise.all(
        allSpecFiles.map(function (moduleName) {
            return System.import(moduleName);
        })
    )
        .then(__karma__.start, __karma__.error);
}