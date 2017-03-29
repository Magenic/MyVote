/**
 * System configuration for Angular samples
 * Adjust as necessary for your application needs.
 */
(function (global) {
   System.config({
      //baseURL: '/',
      
      paths: {
         // paths serve as alias
         'npm:': 'node_modules/',
         'libs:': 'libs/'
      },
      // map tells the System loader where to look for things
      map: {
         // our app is within the app folder
         app: 'app',

         //Angular
         '@angular/core': 'libs:core.umd.js',
         '@angular/common': 'libs:common.umd.js',
         '@angular/compiler': 'libs:compiler.umd.js',
         '@angular/platform-browser': 'libs:platform-browser.umd.js',
         '@angular/platform-browser-dynamic': 'libs:platform-browser-dynamic.umd.js',
         '@angular/http': 'libs:http.umd.js',
         '@angular/router': 'libs:router.umd.js',
         '@angular/router/upgrade': 'libs:router-upgrade.umd.js',
         '@angular/forms': 'libs:forms.umd.js',
         '@angular/upgrade/index': 'libs:upgrade.umd.js',
         '@angular/upgrade': 'libs:upgrade.umd.js',
         '@angular/upgrade/static': 'libs:upgrade-static.umd.js',
         'angular2-logger': 'libs:angular2-logger',

         // angular bundles MOVE TO THE WWWROOT FOLDER AND ACCESS FROM HERE!!!
         //'@angular/core': 'libs:@angular/core/bundles/core.umd.js',
         //'@angular/common': 'libs:@angular/common/bundles/common.umd.js',
         //'@angular/compiler': 'libs:@angular/compiler/bundles/compiler.umd.js',
         //'@angular/platform-browser': 'libs:@angular/platform-browser/bundles/platform-browser.umd.js',
         //'@angular/platform-browser-dynamic': 'libs:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',
         //'@angular/http': 'libs:@angular/http/bundles/http.umd.js',
         //'@angular/router': 'libs:@angular/router/bundles/router.umd.js',
         //'@angular/router/upgrade': 'libs:@angular/router/bundles/router-upgrade.umd.js',
         //'@angular/forms': 'libs:@angular/forms/bundles/forms.umd.js',
         //'@angular/upgrade/index': 'libs:@angular/upgrade/bundles/upgrade.umd.js',
         //'@angular/upgrade': 'libs:@angular/upgrade/bundles/upgrade.umd.js',
         //'@angular/upgrade/static': 'libs:@angular/upgrade/bundles/upgrade-static.umd.js',

         //// angular bundles MOVE TO THE WWWROOT FOLDER AND ACCESS FROM HERE!!!
         //'@angular/core': 'npm:@angular/core/bundles/core.umd.js',
         //'@angular/common': 'npm:@angular/common/bundles/common.umd.js',
         //'@angular/compiler': 'npm:@angular/compiler/bundles/compiler.umd.js',
         //'@angular/platform-browser': 'npm:@angular/platform-browser/bundles/platform-browser.umd.js',
         //'@angular/platform-browser-dynamic': 'npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',
         //'@angular/http': 'npm:@angular/http/bundles/http.umd.js',
         //'@angular/router': 'npm:@angular/router/bundles/router.umd.js',
         //'@angular/router/upgrade': 'npm:@angular/router/bundles/router-upgrade.umd.js',
         //'@angular/forms': 'npm:@angular/forms/bundles/forms.umd.js',
         //'@angular/upgrade': 'npm:@angular/upgrade/bundles/upgrade.umd.js',
         //'@angular/upgrade/static': 'npm:@angular/upgrade/bundles/upgrade-static.umd.js',

         // other libraries
         //'rxjs': 'npm:rxjs'
         'rxjs': 'libs:rxjs'        
      },
      // packages tells the System loader how to load when no filename and/or no extension
      packages: {
         app: {
            main: './main.js',  //Angular entry point of the application where our application will be bootstrapped
            defaultExtension: 'js'
         },
         rxjs: {
            defaultExtension: 'js'
         },
         'angular2-logger': {
            defaultExtension: 'js'
         }
      }
   });
})(this);
