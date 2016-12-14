# MyVote Dev Starter Guide

We're glad you're interested in working on MyVote. The following are some steps that ensure you'll have a productive developer experience to start.

## Prerequisites

Theoretically, our use of `dotnet restore` and NuGet means that all dependencies will be brought in at build time. You shouldn't have to do too much. However, people have had issues without a few simple steps:

* With the emphasis on .NET Core, you'll want to [install it](https://www.microsoft.com/net/core#windowsvs2015). 
* One Visual Studio 2015 user wasn't getting complete TypeScript support until they installed [Web Essentials](http://vswebessentials.com/).

## Configuration

We're still working on flexible, common sense configuration, but the following tips might help you understand what you have to configure to get up and running:

### Database

The database credentials are secret and not found in source control. If you set an environment variable on your machine with the key `SQLCONNSTR_Entities`, that string will be pulled into any project that uses Entities. The easiest way to set this is in project properties on the AppServer and Entities tests projects - the "Debug" tab has a list of environment variables that will exist during debugging. You can get the database information from DevOps (Dan Nordquist). 

If you're using the Azure database, your IP is (likely) not whitelisted for access. Talk to an administrator about getting your IP added to the list. 

### Web Tiers

UI Web points to the AppServer, and since that's not a secret, I don't mind if that value is hard-coded in source. However, if you want to work locally (or in your own Azure or whatever) you'll have to find the string "myvoteapi.azurewebsites.net" and replace it with your values. (At the time of this writing, I think it's actually "localhost" and it need to be changed to "myvoteapi.azurewebsites.net".)

If we want that to be configurable at runtime, I'm not entirely sure how to do that in a container-friendly way for static HTML sites. (Possibly a gulp task?) 

### Azure Mobile Keys

These are static and hard-coded in the source. 

## Git and Pull Requests

Please build any features in their own branches. Once the feature is complete, you can submit a request to have your code pulled in. This will trigger a build request - if your code doesn't build (or the tests don't pass) you'll need to fix that before your PR is considered for merge. 