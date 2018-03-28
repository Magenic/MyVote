// Karma configuration
// Generated on Sat Nov 04 2017 20:12:48 GMT-0400 (Eastern Daylight Time)

module.exports = function (config) {

    var appBase = 'app/';       // transpiled app JS and map files
    var appSrcBase = 'app/'; // app source TS files

    config.set({

        // base path that will be used to resolve all patterns (eg. files, exclude)
        basePath: '',

        // frameworks to use
        // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
        frameworks: ['jasmine'],

        plugins: [
            require('karma-jasmine'),
            require('karma-chrome-launcher'),
            require('karma-mocha-reporter'),
            require('karma-jasmine-html-reporter')
        ],

        //client: {
        //    builtPaths: [appBase], // add more spec base paths as needed
        //    clearContext: false // leave Jasmine Spec Runner output visible in browser
        //},

        // list of files / patterns to load in the browser
        //'./app/**/*spec.js',
        files: [

            'node_modules/systemjs/dist/system.src.js',

            // Polyfills
            'node_modules/core-js/client/shim.js',
            'node_modules/reflect-metadata/Reflect.js',

            // zone.js
            'node_modules/zone.js/dist/zone.js',
            'node_modules/zone.js/dist/long-stack-trace-zone.js',
            'node_modules/zone.js/dist/proxy.js',
            'node_modules/zone.js/dist/sync-test.js',
            'node_modules/zone.js/dist/jasmine-patch.js',
            'node_modules/zone.js/dist/async-test.js',
            'node_modules/zone.js/dist/fake-async-test.js',

            //{ pattern: 'wwwroot/libs/**/*.js', included: false, watched: false },
            //{ pattern: 'wwwroot/libs/**/*.js.map', included: false, watched: false },
            //RxJs
            { pattern: 'node_modules/rxjs/**/*.js', included: false, watched: false },
            { pattern: 'node_modules/rxjs/**/*.js.map', included: false, watched: false },

            // Paths loaded via module imports:
            // Angular itself
            { pattern: 'node_modules/@angular/**/*.js', included: false, watched: false },
            { pattern: 'node_modules/@angular/**/*.js.map', included: false, watched: false },

            { pattern: 'node_modules/angular2-logger/**/*.js', included: false, watched: false },
            { pattern: 'node_modules/angular2-logger/**/*.js.map', included: false, watched: false },

            { pattern: 'systemjs-angular-loader.js', included: false, watched: false },  //<--Needed this
            { pattern: 'systemjs.config.js', included: false, watched: false },
            'karma-test-shim.js',

            { pattern: 'app/index.js', included: false, watched: true },
            { pattern: 'app/config/appSettings.js', included: false, watched: true },
            { pattern: 'app/polls/polls.service.js', included: false, watched: true },
            { pattern: 'app/polls/polls.service.spec.js', included: false, watched: true }

            // transpiled application & spec code paths loaded via module imports
            //{ pattern: appBase + '**/*.js', included: false, watched: true },

            // Asset (HTML & CSS) paths loaded via Angular's component compiler
            // (these paths need to be rewritten, see proxies section)
            //{ pattern: appBase + '**/*.html', included: false, watched: true },
            //{ pattern: appBase + '**/*.css', included: false, watched: true },

            //{ pattern: 'app/polls/polls.service.spec.js', included: false, watched: true }
            //{ pattern: 'app/**/*spec.js', included: false, watched: true },
            //'./app/**/*spec.js'

            // Paths for debugging with source maps in dev tools
            //{ pattern: appBase + '**/*.ts', included: false, watched: false },
            //{ pattern: appBase + '**/*.js.map', included: false, watched: false },

            //{ pattern: 'app/**/*.js', included: false, watched: true }, //<--These DO NOT work
            //{ pattern: 'app/**/*.ts', included: false, watched: true },
            //{ pattern: 'app/**/*.js.map', included: false, watched: true }
        ],

        // Proxied base paths for loading assets
        //proxies: {
        //    // required for modules fetched by SystemJS
        //    //"/app/": "/base/app/"
        //    //'/base/node_modules/': '/base/node_modules/'
        //    "/node_modules/": "/base/node_modules/"
        //},

        // list of files to exclude
        exclude: [],
        // preprocess matching files before serving them to the browser
        // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
        preprocessors: {},
        // test results reporter to use
        // possible values: 'dots', 'progress'
        // available reporters: https://npmjs.org/browse/keyword/karma-reporter
        //Don't include more than 1 reporter, or tests will report running multiple times for each one
        reporters: ['mocha'],
        // web server port
        port: 9876,
        // enable / disable colors in the output (reporters and logs)
        colors: true,
        // level of logging
        // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
        logLevel: config.LOG_INFO,
        // enable / disable watching file and executing tests whenever any file changes
        autoWatch: true,  //<-- when false testing doesn't work'
        // start these browsers
        // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
        //browsers: ['Chrome'],
        browsers: ['ChromeHeadless'],
        // Continuous Integration mode
        // if true, Karma captures browsers, runs the tests and exits
        singleRun: true,
        // Concurrency level
        // how many browser should be started simultaneous
        //concurrency: Infinity
    });
}
