using System.Collections.Generic;
using System.Threading.Tasks;
using WriteAs.NET.Client.Models;
using WriteAs.NET.Shared;

namespace WriteAs.NET
{
    public interface IWriteAsClient
    {
        void Dispose();
        Task<List<Post>> GetAllPosts(string alias, SortOrder sortOrder = SortOrder.Descending);
        Task<Post> GetPostById(string postId);
        Task<Post> GetPostBySlug(string alias, string slug);
        Task<List<Post>> GetPostsByPageNumber(string alias, int pageNumber, SortOrder sortOrder = SortOrder.Descending, bool saveToCache = false);
        Task<List<Post>> Search(string alias, string searchKey, SortOrder sortOrder = SortOrder.Descending);
    }
}