/// <binding BeforeBuild='cleanRootFiles, defaultBuild' AfterBuild='moveToDistOutput' Clean='cleanRootFiles' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp");
var ts = require('gulp-typescript');
var tsProject = ts.createProject("tsconfig.json");
var sourcemaps = require('gulp-sourcemaps');
var clean = require('gulp-clean');
var merge = require('merge-stream');
var runSequence = require('run-sequence');

var paths = {
   appRoot: "./",
   appMain: "./app/",
   indexTarget: "./wwwroot/",
   npmSrc: "./node_modules/",
   appTarget: "./wwwroot/app",
   libTarget: "./wwwroot/libs/",
   sharedTarget: "./wwwroot/app/shared",
   stylesTarget: "./wwwroot/app/shared/content"
};

var appRootFilesToMove = [
   paths.appRoot + "index.html",
   paths.appRoot + "favicon.ico",
   paths.appRoot + "systemjs.config.js",
   paths.appRoot + "systemjs-angular-loader.js"
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
   paths.npmSrc + "/ms-signalr-client/jquery.signalr.js",
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
    paths.npmSrc + "@angular/core/bundles/core-testing.umd.js",
    paths.npmSrc + "@angular/common/bundles/common-testing.umd.js",
    paths.npmSrc + "@angular/compiler/bundles/compiler-testing.umd.js",
    paths.npmSrc + "@angular/platform-browser/bundles/platform-browser-testing.umd.js",
    paths.npmSrc + "@angular/platform-browser-dynamic/bundles/platform-browser-dynamic-testing.umd.js",
    paths.npmSrc + "@angular/http/bundles/http-testing.umd.js",
    paths.npmSrc + "@angular/router/bundles/router-testing.umd.js",
    paths.npmSrc + "@angular/forms/bundles/forms-testing.umd.js",

   //Not sure how to cherry-pick correct files so pulling the whole package in the task below
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
   paths.npmSrc + "/jquery-ui/themes/base/theme.css",
   paths.npmSrc + "/bootstrap/dist/css/bootstrap.css"
];

//Single task to run for this gulpfile. Organizes task order to execute and increases performance by running in parallel
gulp.task("defaultBuild", function (done) {

   //ensure the clean task and TS compilation are complete prior to running the others in parallel
    runSequence("cleanRootFiles", "tsCompile",
    ["moveToDistOutput"], 
        function () {
         done();
      });
});

gulp.task('cleanRootFiles', function () {
   return gulp.src(paths.indexTarget, { read: false })
       .pipe(clean({ force: true }));
});

//Task will transpile .ts -> .js and also create .map files as well for debugging
gulp.task("tsCompile", function () {
    return tsProject.src()
        .pipe(sourcemaps.init())
        .pipe(tsProject()) //use .tsconfig settings for compilation
        .js.pipe(sourcemaps.write('.'))
        .pipe(gulp.dest(paths.appMain)); //write files to same directory as residing .ts files
});

//Task to move all needed files to wwwroot to run the site
gulp.task("moveToDistOutput", function () {
    //app folder copy
    var appRootFiles = gulp.src(appRootFilesToMove).pipe(gulp.dest(paths.indexTarget));
    var appFiles = gulp.src(["./app/**/*"]).pipe(gulp.dest(paths.appTarget));
    //If not needing browser debugging (i.e. prod, although recommend using a prod script), ignore .ts and .map files
    //var appFiles = gulp.src(["!./app/**/*.ts", "!./app/**/*.map","./app/**/*"]).pipe(gulp.dest(paths.appTarget));
    var appSharedFiles = gulp.src(["./app/shared/**/*"]).pipe(gulp.dest(paths.sharedTarget));
    var appContentFiles = gulp.src(stylesToMove).pipe(gulp.dest(paths.stylesTarget));

    //libs folder copy
    var individualLibs = gulp.src(libsToMove).pipe(gulp.dest(paths.libTarget));
    var rxjsLibs = gulp.src(paths.npmSrc + "rxjs/**/*.js").pipe(gulp.dest("./wwwroot/libs/rxjs/"));
    var signalRLibs = gulp.src(paths.npmSrc + "@aspnet/signalr-client/dist/browser/*.js").pipe(gulp.dest("./wwwroot/libs/signalr/"));
    var angularLoggerLibs = gulp.src(paths.npmSrc + "angular2-logger/**/*").pipe(gulp.dest("./wwwroot/libs/angular2-logger/"));

    //Copy files concurrently
    return merge(appRootFiles, appFiles, appSharedFiles, appContentFiles, individualLibs, rxjsLibs, signalRLibs, angularLoggerLibs);
});
