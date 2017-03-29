declare var angularJS: angular.IAngularStatic;

//import * as angular from 'angular';
import { Component, Pipe, PipeTransform, OnInit } from '@angular/core';
import { Location, NgFor, NgIf } from '@angular/common';
import { PollsService } from './polls.service';
import { downgradeComponent } from '@angular/upgrade/static';
//import { App } from '../angular1x/app';

class PollFilters {
    constructor(public value: string, public name: string) { }
}

@Pipe({ name: 'values' })
export class ValuesPipe implements PipeTransform {
    transform(value: any, args?: any[]): Object[] {
        let keyArr: any[] = Object.keys(value),
            dataArr = [],
            keyName = args[0];

        keyArr.forEach((key: any) => {
            value[key][keyName] = key;
            dataArr.push(value[key]);
        });

        if (args[1]) {
            dataArr.sort((a: Object, b: Object): number => {
                return a[keyName] > b[keyName] ? 1 : -1;
            });
        }

        return dataArr;
    }
}

@Component(({
    //module.id variable is available and contains the absolute URL of the component class module file. allows using relative paths vs. absolute paths for commonjs modules + systemjs loader
    moduleId: module.id, //remove node typings to show what will happen (or any other typings example)
    selector: 'polls-list',
    templateUrl: 'polls-list.component.html',
    styleUrls: ['polls-list.component.css']
}) as any)

export class PollsListComponent {

    private pollGroups: any;//MyVote.Services.AppServer.Models.PollSummary[][];
    filterBy: PollFilters;
    message: string;

    filters: PollFilters[] = [
        new PollFilters('Newest', 'Newest'),
        new PollFilters('MostPopular', 'Most Popular')];

    constructor(private location: Location, private pollsService: PollsService) {
        this.filterBy = this.filters[0];
    }

    ngOnInit() {
        this.onChange(this.filterBy);
    }

    onChange(newObj) {
        this.pollGroups = null;
        console.log(newObj);
        this.message = 'Loading polls...';
        this.pollsService.getPolls(newObj.value).then((result: any): any => {
                this.message = result ? null : "There are no polls!";
                this.pollGroups = result;
            });
    }

    responseText = (count: number) => {
        return count === 1 ? "Response" : "Responses";
    };

    addPoll(): void {
        this.location.go('/addPoll');
    };

    viewPoll(id): void {
        this.location.go('/viewPoll/' + id);
    };

    getBackImg(imageLink: string): string {
        return imageLink && imageLink !== '0'
            ? imageLink
            : '/app/shared/content/checkmark.svg';
    };

}

//Instead of registering a component, we register a pollsList directive, a downgraded version of the Angular 2 component.
//The as angular.IDirectiveFactory cast tells the TypeScript compiler that the return value of the downgradeComponent method is a directive factory.
angular.module('MyVoteApp')
    .directive('pollsList', downgradeComponent({ component: PollsListComponent }) as angular.IDirectiveFactory);