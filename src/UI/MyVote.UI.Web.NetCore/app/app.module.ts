//app.module.ts is the root module for the application

//ES6 style module imports
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser'; //allows execution of this app in the browser; required
import { FormsModule } from '@angular/forms';
import { UpgradeModule } from '@angular/upgrade/static'; //To bootstrap a hybrid application, we need to import UpgradeModule in our AppModule, and override it's bootstrap method:
import { HttpModule } from '@angular/http';
import { HashLocationStrategy, Location, LocationStrategy } from '@angular/common';
import { Logger } from "angular2-logger/core";
//import { AppRoutingModule } from './app-routing.module';
//import { AuthService } from './angular1x/services/auth'

import { PollsService } from './polls/polls.service';
import { PollsListComponent, ValuesPipe } from './polls/polls-list.component';
import { PollsViewComponent } from './polls/polls-view.component';
import { SignalRService } from './shared/signalr.service';

@NgModule({
    imports: [BrowserModule, FormsModule, UpgradeModule, HttpModule],  //Any imported Angular module must be decorated with @NgModule
    declarations: [PollsListComponent, ValuesPipe, PollsViewComponent],
    providers: [
        Location, PollsService, Logger, SignalRService,
        { provide: LocationStrategy, useClass: HashLocationStrategy },
        { provide: 'authService', useFactory: ($injector: any) => $injector.get('authService'), deps: ['$injector'] },
        { provide: '$routeParams', useFactory: ($injector: any) => $injector.get('$routeParams'), deps: ['$injector'] }
    ], //Add providers here that will be available to entire app as this is the root app module
    entryComponents: [PollsListComponent, PollsViewComponent] //add to entryComponents since we downgraded it. Specifies a list of components that should be compiled when this module is defined. For each component listed here, Angular will create a ComponentFactory and store it in the ComponentFactoryResolver
})

export class AppModule {
    constructor(private upgrade: UpgradeModule) { }
    ngDoBootstrap() {
        this.upgrade.bootstrap(document.documentElement, ['MyVoteApp']);
    }
}