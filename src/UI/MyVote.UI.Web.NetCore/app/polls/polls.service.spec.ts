import { async, inject, TestBed } from '@angular/core/testing';
import { BaseRequestOptions, Http, HttpModule, Response, ResponseOptions } from '@angular/http';
import { MockBackend } from '@angular/http/testing';
import { PollsService } from "./polls.service";
import { Logger } from 'angular2-logger/core';

describe('polls service unit tests', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                PollsService,
                Logger,
                MockBackend,
                BaseRequestOptions,
                {
                    provide: Http,
                    useFactory: (backend, options) => new Http(backend, options),
                    deps: [MockBackend, BaseRequestOptions]
                }
            ],
            imports: [
                HttpModule
            ]
        });
    });

    it('should construct the service', async(inject(
        [PollsService, MockBackend], (service, mockBackend) => {
            expect(service).toBeDefined();
        })));

    describe('getPoll()', () => {

        const mockResponse = {
            PollID: 1,
            PollDescription: 'Poll 1'
        };

        it('should parse poll response correctly', async(inject(
            [PollsService, MockBackend], (service, mockBackend) => {

                mockBackend.connections.subscribe(conn => {
                    conn.mockRespond(new Response(new ResponseOptions({ body: JSON.stringify(mockResponse) })));
                });

                service.getPoll(1).then(res => {
                    expect(res.PollDescription).toEqual('Poll 1');
                });
            })));
    });
});


////import { ComponentFixture, TestBed } from '@angular/core/testing';
////import { PollsService } from "./polls.service";
////describe("Sanity Check", () => {
////    it("should expect 2 + 2 to equal 4", () => {
////        expect(2 + 2).toEqual(4);
////    });
////});


//import { Injectable, Injector } from '@angular/core';
//import { inject, TestBed } from "@angular/core/testing";
//import { PollsService } from "./polls.service";
//import { HttpModule, ResponseOptions, XHRBackend } from "@angular/http";
//import { MockBackend } from "@angular/http/testing";
//import { Logger } from 'angular2-logger/core';

////import { Injector } from '@angular/core';
//////import { async, fakeAsync, tick } from '@angular/core/testing';
////import { BaseRequestOptions, ConnectionBackend, Http, RequestOptions } from '@angular/http';
//////import { Response, ResponseOptions } from '@angular/http';
////import { MockBackend } from '@angular/http/testing';



////class MockLogger {
    
////}

////class MockPollsService {

////    public getPoll(id: number): any {
        
////    }

////}

//describe("polls service unit tests", () => {

//    beforeEach(() => {
//        TestBed.configureTestingModule({
//            imports: [HttpModule],
//            providers: [
//                { provide: XHRBackend, useClass: MockBackend },
//                Logger,
//                PollsService                   
//            ]
//        });
//    });

//    describe("getPoll()", () => {



//        it("should return a Promise<MyVote.Services.AppServer.Models.Poll>",
//            inject([PollsService, XHRBackend], (pollsService, mockBackend) => {

//            const mockResponse = {
//                data: [
//                    { PollID: 0, PollDescription: 'Poll 0' },
//                    { PollID: 1, PollDescription: 'Poll 1' },
//                    { PollID: 2, PollDescription: 'Poll 2' },
//                    { PollID: 3, PollDescription: 'Poll 3' }
//                ]
//            };

//            //mockBackend.connections.subscribe((connection) => {
//            //    connection.mockRespond(new Response(new ResponseOptions({
//            //        body: JSON.stringify(mockResponse)
//            //    })));
//            //});

//                mockBackend.connections.subscribe((connection) => {
//                    connection.mockRespond(new Response(new ResponseOptions({
//                        body: mockResponse
//                    })));
//                });

//            pollsService.getPoll(1).then((polls) => {
//                expect(polls.length).toBe(4);
//                expect(polls[0].name).toEqual('Poll 0');
//                expect(polls[1].name).toEqual('Poll 1');
//                expect(polls[2].name).toEqual('Poll 2');
//                expect(polls[3].name).toEqual('Poll 3');
//            });

//        }));

//    });

////    it("should be true", function () {
////        expect(true).toBe(true);
////    });
////
////    describe("using beforeEach and afterEach", function () {
////
////        var personTest;
////        var Person = function () {
////            this.name = "";
////        };
////
////        beforeEach(function () {
////            personTest = new Person();
////            personTest.name = "Allen Conway";
////        });
////
////        it("the Person object should not be null", function () {
////            expect(personTest).not.toBe(null);
////        });
////
////        it("the Person name should be Allen Conway", function () {
////            expect(personTest.name).toEqual("Allen Conway");
////        });
////
////        afterEach(function () {
////            //Some required cleanup
////            personTest = null;
////        });
////
////    });

//});