/// <reference path="../typings/index.d.ts" />

//Our application was previously bootstrapped using the Angular 1 ng-app directive attached to the <html> element of the host page. 
//This will no longer work with Angular 2. We should switch to a JavaScript-driven bootstrap instead.

//So, remove the ng-app attribute from index.html, and instead boostrap via app/main.ts.
//This file has been configured as the application entry point in systemjs.config.js, so it is already being loaded by the browser

import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
//import { UpgradeModule } from '@angular/upgrade/static';

import { AppModule } from './app.module';

//platformBrowserDynamic().bootstrapModule(AppModule).then(platformRef => {
//    const upgrade = platformRef.injector.get(UpgradeModule) as UpgradeModule;
//    upgrade.bootstrap(document.body, ['MyVoteApp']); 
//    //upgrade.bootstrap(document.body, [App.name]); 
//});


platformBrowserDynamic().bootstrapModule(AppModule);