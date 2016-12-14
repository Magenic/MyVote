/// <binding AfterBuild='defaultBuild' />
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

   paths.npmSrc + "core-js/client/shim.js",
   paths.npmSrc + "zone.js/dist/zone.js",
   paths.npmSrc + "reflect-metadata/Reflect.js",
   paths.npmSrc + "systemjs/dist/system.src.js"

];

var stylesToMove = [
   paths.npmSrc + "/jquery-ui/themes/base/jquery-ui.css",
   paths.npmSrc + "/bootstrap/dist/css/bootstrap.css"
];

//Single task to run for this gulpfile. Organizes task order to execute and increases performance by running in parallel
gulp.task("defaultBuild", function(done) {
   
   //ensure the clean task is complete prior to running the others in parallel
   runSequence("cleanRootFiles", 
      ["moveToApp", "moveToLibs", "moveToRoot", "moveToShared", "moveToContent"], function() {
         done();
      });
});

gulp.task('cleanRootFiles', function () {
   return gulp.src(paths.indexTarget, { read: false })
       .pipe(clean());
});

gulp.task("moveToApp", function () {
   var jsFiles = gulp.src(["./app/angular1x/**/*.js"]).pipe(gulp.dest("./wwwroot/app/angular1x"));
   var tsFiles = gulp.src(["./app/angular1x/**/*.ts"]).pipe(gulp.dest("./wwwroot/app/angular1x"));   
   var htmlFiles = gulp.src(["./app/angular1x/**/*.html"]).pipe(gulp.dest("./wwwroot/app/angular1x"));
   return merge(jsFiles, tsFiles, htmlFiles);
});

gulp.task("moveToShared", function () {
   return gulp.src(["./app/shared/**/*.*"]).pipe(gulp.dest("./wwwroot/app/shared"));
});

gulp.task("moveToLibs", function () {
   return gulp.src(libsToMove).pipe(gulp.dest(paths.libTarget));
});

gulp.task("moveToContent", function () {
   return gulp.src(stylesToMove).pipe(gulp.dest(paths.stylesTarget));
});

gulp.task("moveToRoot", function () {
   return gulp.src(appRootFilesToMove).pipe(gulp.dest(paths.indexTarget));
});