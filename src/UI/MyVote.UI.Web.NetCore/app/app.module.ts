//app.module.ts is the root module for the application

declare var angular: angular.IAngularStatic;

//ES6 style module imports
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';  //allows execution of this app in the browser; required
import { UpgradeModule } from '@angular/upgrade/static'; //To bootstrap a hybrid application, we need to import UpgradeModule in our AppModule, and override it's bootstrap method:
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { Routes, Router, RouterModule, ActivatedRoute } from '@angular/router';
import { APP_BASE_HREF, HashLocationStrategy, Location, LocationStrategy } from '@angular/common';
import { Logger } from "angular2-logger/core";
import { AppRoutingModule } from './app-routing.module';
//import { AuthService } from './angular1x/services/auth'

import { PollsService } from './polls/polls.service';
import { PollsListComponent, ValuesPipe } from './polls/polls-list.component';
import { PollsViewComponent } from './polls/polls-view.component';

@NgModule({
    imports: [BrowserModule, UpgradeModule, HttpModule, FormsModule],  //Any imported Angular module must be decorated with @NgModule
    declarations: [PollsListComponent, ValuesPipe, PollsViewComponent],
    providers: [Location, PollsService, Logger, 
        { provide: LocationStrategy, useClass: HashLocationStrategy },
        { provide: Router, useClass: RouterModule },
        { provide: 'authService', useFactory: ($injector: any) => $injector.get('authService'), deps: ['$injector'] },
        { provide: '$routeParams', useFactory: ($injector: any) => $injector.get('$routeParams'), deps: ['$injector'] },
        //{ provide: '$routeParams', useFactory: ($injector: any) => $injector.get('$routeParams'), deps: ['$routeParams'] }
    ], //Add providers here that will be available to entire app as this is the root app module
    entryComponents: [PollsListComponent, PollsViewComponent] //add to entryComponents since we downgraded it. Specifies a list of components that should be compiled when this module is defined. For each component listed here, Angular will create a ComponentFactory and store it in the ComponentFactoryResolver
})

export class AppModule {
    ngDoBootstrap() { /* this is a placeholder to stop the boostrapper from complaining */ }    
}