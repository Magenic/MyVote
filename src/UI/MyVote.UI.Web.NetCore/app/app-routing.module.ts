import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
    //{ path: '', pathMatch: 'full', redirectTo: 'landing' },
    //{ path: 'landing', component: LandingComponent }

];

@NgModule({
    imports: [RouterModule.forRoot(routes)],  //Any imported Angular module must be decorated with @NgModule
    exports: [RouterModule]
})

export class AppRoutingModule {

}