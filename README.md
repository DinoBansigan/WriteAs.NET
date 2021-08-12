# WriteAs.NET
An unofficial .NET Core client for the Write.as API written in C#.

**Update:**  
The latest version of the source code now allows you to enter an API key when initializing a `WriteAsClient` instance. This API key will allow you to bypass the rate limiting checks on the Write.as API.

Some basic in-memory caching has also been added to the client. You can configure some of the cache settings when initializing a `WriteAsClient` instance. The new settings are described below:
- `cacheExpirationInSeconds` determines how long data will stay in the cache before it expires. The default value for this setting is 300 seconds.
- `cacheSize` determines how many objects it can store in the cache. Note that a collection of posts (`List<Post>`) and a single post each count as 1 item. The default value for this setting is 4.

Nuget package is still currently at version 1.1.0. I'll release a new version with the latest changes (api key and caching) after doing some more real world testing.

---

At the moment it only supports very basic "get" methods. I initially wrote this client/wrapper library just for fun. Then I started using it to build out the [Archive Page](https://journal.dinobansigan.com/archive) on my [Write.as](https://write.as/) blog/journal.

Right now this wrapper library is being used to power a number of other Blazor WASM apps I've written:
- [WriteFreely Archive Page Generator app](https://wf-archive.dinobansigan.com/)
- [Search app](https://searchjournal.dinobansigan.com/)
- [Popular Posts app](https://popularposts.dinobansigan.com/?alias=dino)

**Currently Supported Operations**

```
Task<List<Post>> GetAllPosts(string alias, SortOrder sortOrder = SortOrder.Descending);
Task<Post> GetPostById(string postId);
Task<Post> GetPostBySlug(string alias, string slug);
Task<List<Post>> GetPostsByPageNumber(string alias, int pageNumber, SortOrder sortOrder = SortOrder.Descending);
Task<List<Post>> Search(string alias, string searchKey, SortOrder sortOrder = SortOrder.Descending);
```

**Usage Examples**  

*Initialize new instance -- Note: To avoid creating multiple socket connections, initialize a single `WriteAsClient` instance and use it throughout the whole app.*
```
var client = new WriteAsClient("https://write.as/", apiKey, cacheExpirationInSeconds, cacheSize);
```

*GetAllPosts*
```
List<Post> allPosts = await client.GetAllPosts(alias, sortOrder);
```

*GetPostsByPageNumber*
```
List<Post> allPosts = await client.GetPostsByPageNumber(alias, pageNumber, sortOrder);
```

*GetPostBySlug*
```
Post post = await client.GetPostBySlug(alias, slug);
```

*GetPostById*
```
Post post = await client.GetPostById(postId);
```

*Search*
```
List<Post> searchResults = await client.Search(alias, searchKey);
```

---
  
**Installing WriteAs.NET**

Note: Nuget package is still currently at version 1.1.0. I'll release a new version with the latest changes (api key and caching) after doing some more real world testing.

You can install via nuget:  

`Install-Package WriteAs.NET -Version 1.1.0`

Or via the .NET Core command line interface:  

`dotnet add package WriteAs.NET --version 1.1.0`


