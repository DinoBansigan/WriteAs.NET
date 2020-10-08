# WriteAs.NET
An unofficial .NET Core client for the Write.as API written in C#.

At the moment it only supports very basic "get" methods. I initially wrote this client/wrapper library just for fun. Then I started using it to build out the [Archive Page](https://journal.dinobansigan.com/archive) on my [Write.as](https://write.as/) blog/journal. I also plan to use this client to power a Blazor search page for my online journal.

**Usage Examples**  

I will be uploading a .NET Core console application that will show more concrete examples of how to use this client. But for now, you can reference the examples below.  

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
  
**Currently Supported Operations**

```
Task<List<Post>> GetAllPosts(string alias, SortOrder sortOrder = SortOrder.Descending);
Task<Post> GetPostById(string postId);
Task<Post> GetPostBySlug(string alias, string slug);
Task<List<Post>> GetPostsByPageNumber(string alias, int pageNumber, SortOrder sortOrder = SortOrder.Descending);
Task<List<Post>> Search(string alias, string searchKey, SortOrder sortOrder = SortOrder.Descending);
```

  
**Installing WriteAs.NET**

You can install via nuget:  

`Install-Package WriteAs.NET -Version 1.0.0`

Or via the .NET Core command line interface:  

`dotnet add package WriteAs.NET --version 1.0.0`


