MyVote
------
MyVote is an app developed by [Magenic](http://www.magenic.com) as a comprehensive demo for the [Modern Apps Live!](http://www.modernappslive.com) conference held twice a year in Las Vegas (spring) and Orlando (fall).

The code in this repo has been scrubbed to remove sensitive key values for encryption and services. The following is a list of changes you'll need to make to insert your own keys into the codebase:

**web.config**
```
    <add key="zumoKey" value="<your key here>" />
```

**web.config**
```
      <add key="StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName=myvoteapp;AccountKey=<your key>" />
```

**PollImageController**
```
		const string STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=myvoteapp;AccountKey=<your key>";
```

**Startup.Auth.cs**
```csharp
            var signingKey = new SymmetricSecurityKey(FromHex("<your key>"));            
```

Note that this `SymmetricSecurityKey` value comes from the `WEBSITE_AUTH_SIGNING_KEY` environment variable in Azure.