# WriteAs.NET
An unofficial .NET Core client for the Write.as API written in C#.

**Update:**  
Nuget package has been updated to version 1.1.0. This includes support for .NET Standard 2.1, which should allow you to use this package in a Blazor WASM app. And updated Search logic to include post titles when searching.

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

*GetAllPosts*
```
using (var client = new WriteAsClient("https://write.as/"))
{
    List<Post> allPosts = await client.GetAllPosts(alias, sortOrder);
}
```

*GetPostsByPageNumber*
```
using (var client = new WriteAsClient("https://write.as/"))
{
    List<Post> allPosts = await client.GetPostsByPageNumber(alias, pageNumber, sortOrder);
}
```

*GetPostBySlug*
```
using (var client = new WriteAsClient("https://write.as/"))
{
    Post post = await client.GetPostBySlug(alias, slug);
}
```

*GetPostById*
```
using (var client = new WriteAsClient("https://write.as/"))
{
    Post post = await client.GetPostById(postId);
}
```

*Search*
```
using (var client = new WriteAsClient("https://write.as/"))
{
    List<Post> searchResults = await client.Search(alias, searchKey);
}
```

  
**Installing WriteAs.NET**

You can install via nuget:  

`Install-Package WriteAs.NET -Version 1.1.0`

Or via the .NET Core command line interface:  

`dotnet add package WriteAs.NET --version 1.1.0`


