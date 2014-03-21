// Karma configuration: shared config for controllers

module.exports = function(config) {
  config.set({

    // base path, that will be used to resolve files and exclude
    basePath: '',

    // frameworks to use
    frameworks: ['jasmine'],

    // list of files / patterns to load in the browser
    files: [
      {pattern: '../MyVote.Client.Web/Scripts/angular.js', watched: false},
      {pattern: '../MyVote.Client.Web/Scripts/angular-mocks.js', watched: false},
      '../MyVote.Client.Web/app/controllers/*.js',
      'Tests/*Spec.js'
    ],

    // list of files to exclude
    exclude: [
    ],
    
    preprocessors: {
        '../MyVote.Client.Web/app/controllers/*.js': 'coverage'
    },

    // test results reporter to use
    // possible values: 'dots', 'progress', 'junit', 'growl', 'coverage'
    reporters: ['progress', 'coverage'],

    // web server port
    port: 9876,

    // enable / disable colors in the output (reporters and logs)
    colors: true,

    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,

    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,

    // If browser does not capture in given timeout [ms], kill it
    captureTimeout: 60000,
  });
};
