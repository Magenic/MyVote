/// <binding AfterBuild='defaultBuild, moveToApp, moveToContent, moveToLibs, moveToRoot, moveToShared' Clean='cleanRootFiles' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp");
var clean = require('gulp-clean');
var merge = require('merge-stream');
var runSequence = require('run-sequence');

var paths = {
   appRoot: "./",
   indexTarget: "./wwwroot/",
   npmSrc: "./node_modules/",
   libTarget: "./wwwroot/libs/",
   stylesTarget: "./wwwroot/app/shared/content"
};

var appRootFilesToMove = [
   paths.appRoot + "index.html",
   paths.appRoot + "favicon.ico",
   paths.appRoot + "systemjs.config.js"
];

var libsToMove = [
   paths.npmSrc + "/moment/moment.js",
   paths.npmSrc + "/jquery/dist/jquery.js",
   paths.npmSrc + "/jquery-ui/jquery-ui.js",
   paths.npmSrc + "/bootstrap/dist/js/bootstrap.js",
   paths.npmSrc + "/angular/angular.js",
   paths.npmSrc + "/angular-route/angular-route.js",
   paths.npmSrc + "/angular-resource/angular-resource.js",
   paths.npmSrc + "/highcharts/highcharts.js",
   paths.npmSrc + "/highcharts-ng/dist/highcharts-ng.js",
   paths.npmSrc + "/angular-ui-bootstrap/dist/ui-bootstrap.js",
   paths.npmSrc + "/angular-ui-bootstrap/dist/ui-bootstrap-tpls.js",
   paths.npmSrc + "/ms-signalr-client/jquery.signalr-2.2.0.js",
   paths.npmSrc + "/modernizr/src/modernizr.js",

   paths.npmSrc + "@angular/core/bundles/core.umd.js",
   paths.npmSrc + "@angular/common/bundles/common.umd.js",
   paths.npmSrc + "@angular/compiler/bundles/compiler.umd.js",
   paths.npmSrc + "@angular/platform-browser/bundles/platform-browser.umd.js",
   paths.npmSrc + "@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js",
   paths.npmSrc + "@angular/http/bundles/http.umd.js",
   paths.npmSrc + "@angular/router/bundles/router.umd.js",
   paths.npmSrc + "@angular/router/bundles/router-upgrade.umd.js",
   paths.npmSrc + "@angular/forms/bundles/forms.umd.js",
   paths.npmSrc + "@angular/upgrade/bundles/upgrade.umd.js",
   paths.npmSrc + "@angular/upgrade/bundles/upgrade-static.umd.js",
   paths.npmSrc + "@angular/upgrade/bundles/upgrade-static.umd.js",

   //paths.npmSrc + 'angular2-logger/bundles/angular2-logger.sys.js',
   //paths.npmSrc + 'angular2-logger/core.js',

   paths.npmSrc + "core-js/client/shim.js",
   paths.npmSrc + "zone.js/dist/zone.js",
   paths.npmSrc + "reflect-metadata/Reflect.js",
   paths.npmSrc + "systemjs/dist/system.src.js"

];

//var fullLibsToMove = [
//   paths.npmSrc + "rxjs/**/*.*"
//];

var stylesToMove = [
   paths.npmSrc + "/jquery-ui/themes/base/jquery-ui.css",
   paths.npmSrc + "/bootstrap/dist/css/bootstrap.css"
];

//Single task to run for this gulpfile. Organizes task order to execute and increases performance by running in parallel
gulp.task("defaultBuild", function (done) {

   //ensure the clean task is complete prior to running the others in parallel
   runSequence("cleanRootFiles",
      ["moveToApp", "moveToLibs", "moveToRoot", "moveToShared", "moveToContent"], function () {
         done();
      });
});

gulp.task('cleanRootFiles', function () {
   return gulp.src(paths.indexTarget, { read: false })
       .pipe(clean({ force: true }));
});

//Todo: need to add .map files for TS browser debugging
gulp.task("moveToApp", function () {
   var appRootJsFiles = gulp.src(["./app/*.js"]).pipe(gulp.dest("./wwwroot/app"));
   var appRootTsFiles = gulp.src(["./app/*.ts"]).pipe(gulp.dest("./wwwroot/app"));
   var jsFiles = gulp.src(["./app/angular1x/**/*.js"]).pipe(gulp.dest("./wwwroot/app/angular1x"));
   var tsFiles = gulp.src(["./app/angular1x/**/*.ts"]).pipe(gulp.dest("./wwwroot/app/angular1x"));
   var htmlFiles = gulp.src(["./app/angular1x/**/*.html"]).pipe(gulp.dest("./wwwroot/app/angular1x"));
   var a2Polls = gulp.src(["./app/polls/**/*"]).pipe(gulp.dest("./wwwroot/app/polls"));
   return merge(appRootJsFiles, appRootTsFiles, jsFiles, tsFiles, htmlFiles, a2Polls);
});

gulp.task("moveToShared", function () {
   return gulp.src(["./app/shared/**/*.*"]).pipe(gulp.dest("./wwwroot/app/shared"));
});

gulp.task("moveToLibs", function () {
   var individualLibs = gulp.src(libsToMove).pipe(gulp.dest(paths.libTarget));
   //var angular2Libs = gulp.src(paths.npmSrc + "@angular/**/*").pipe(gulp.dest("./wwwroot/libs/@angular"));
   var rxjsLibs = gulp.src(paths.npmSrc + "rxjs/**/*").pipe(gulp.dest("./wwwroot/libs/rxjs/"));
   var angularLoggerLibs = gulp.src(paths.npmSrc + "angular2-logger/**/*").pipe(gulp.dest("./wwwroot/libs/angular2-logger/"));
   //var momentLibs = gulp.src(paths.npmSrc + "/moment/**/*").pipe(gulp.dest("./wwwroot/libs/moment"));
   return merge(individualLibs, rxjsLibs, angularLoggerLibs);
});

gulp.task("moveToContent", function () {
   return gulp.src(stylesToMove).pipe(gulp.dest(paths.stylesTarget));
});

gulp.task("moveToRoot", function () {
   return gulp.src(appRootFilesToMove).pipe(gulp.dest(paths.indexTarget));
});
