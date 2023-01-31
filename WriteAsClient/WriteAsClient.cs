using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WriteAs.NET.Client.Models;
using WriteAs.NET.Client.Responses;
using WriteAs.NET.Shared;

namespace WriteAs.NET
{
    public class WriteAsClient : IDisposable, IWriteAsClient
    {
        private static HttpClient _httpClient;
        private JsonSerializerOptions _jsonSerializerOptions;
        private Cache _cache;

        private readonly string ApiCollectionsPostQueryString = "api/collections/{0}/posts?page={1}";
        private readonly string ApiCollectionsPostSlugQueryString = "api/collections/{0}/posts/{1}";
        private readonly string ApiPostQueryString = "api/posts/{0}";

        public WriteAsClient(string writeAsApiUri, string apiKey = null, int cacheExpirationInSeconds = 300, int cacheSize = 4)
            : base()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(writeAsApiUri)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            }

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            _cache = new Cache(cacheExpirationInSeconds, cacheSize);
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
            if (_cache != null)
            {
                _cache.Dispose();
            }
        }

        public async Task<List<Post>> GetAllPosts(string alias, SortOrder sortOrder = SortOrder.Descending)
        {
            string cacheKey = string.Format("GetAllPosts-{0}", alias);
            List<Post> allPosts = _cache.GetDataFromCache<List<Post>>(cacheKey);

            if (allPosts == null)
            {
                allPosts = new List<Post>();
                // Initial pull - get first 10 posts from page 1
                HttpResponseMessage response = await _httpClient.GetAsync(string.Format(ApiCollectionsPostQueryString, alias, "1"));
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WriteAsCollectionResponse writeAsResponse = JsonSerializer.Deserialize<WriteAsCollectionResponse>(responseBody, _jsonSerializerOptions);

                    if (writeAsResponse != null && writeAsResponse.Data != null)
                    {
                        allPosts.AddRange(writeAsResponse.Data.Posts);

                        // If there are more posts, get the rest
                        if (writeAsResponse.Data.TotalNumberOfPosts > writeAsResponse.Data.Posts.Count)
                        {
                            int totalNumberOfPages = GetTotalNumberOfPages(writeAsResponse.Data.TotalNumberOfPosts);
                            for (int i = 2; i <= totalNumberOfPages; i++)
                            {
                                List<Post> posts = await GetPostsByPageNumber(alias, i);
                                allPosts.AddRange(posts);
                            }
                        }

                        _cache.SaveDataToCache<List<Post>>(cacheKey, allPosts);
                    }
                }
            }

            allPosts = SortPosts(sortOrder, allPosts);

            return allPosts;
        }

        public async Task<List<Post>> GetPostsByPageNumber(string alias, int pageNumber, SortOrder sortOrder = SortOrder.Descending, bool saveToCache = false)
        {
            string cacheKey = string.Format("GetPostsByPageNumber-{0}-{1}", alias, pageNumber);
            List<Post> posts = _cache.GetDataFromCache<List<Post>>(cacheKey);

            if (posts == null)
            {
                posts = new List<Post>();
                HttpResponseMessage response = await _httpClient.GetAsync(string.Format(ApiCollectionsPostQueryString, alias, pageNumber.ToString()));
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WriteAsCollectionResponse writeAsResponse = JsonSerializer.Deserialize<WriteAsCollectionResponse>(responseBody, _jsonSerializerOptions);

                    if (writeAsResponse != null && writeAsResponse.Data != null)
                    {
                        posts.AddRange(writeAsResponse.Data.Posts);

                        // Since this method is used by other methods like GetAllPosts, which already saves data to the cache, I wanted to give the option
                        // to not save data to the cache for this method.
                        if (saveToCache)
                        {
                            _cache.SaveDataToCache<List<Post>>(cacheKey, posts);
                        }
                    }
                }
            }

            posts = SortPosts(sortOrder, posts);

            return posts;
        }

        public async Task<Post> GetPostBySlug(string alias, string slug)
        {
            string cacheKey = string.Format("GetPostBySlug-{0}-{1}", alias, slug);
            Post post = _cache.GetDataFromCache<Post>(cacheKey);

            if (post == null)
            {
                HttpResponseMessage response = await _httpClient.GetAsync(string.Format(ApiCollectionsPostSlugQueryString, alias, slug));
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WriteAsPostResponse writeAsResponse = JsonSerializer.Deserialize<WriteAsPostResponse>(responseBody, _jsonSerializerOptions);

                    if (writeAsResponse != null && writeAsResponse.Data != null)
                    {
                        post = writeAsResponse.Data;
                        _cache.SaveDataToCache<Post>(cacheKey, post);
                    }
                }
            }

            return post;
        }

        public async Task<Post> GetPostById(string postId)
        {
            string cacheKey = string.Format("GetPostById-{0}", postId);
            Post post = _cache.GetDataFromCache<Post>(cacheKey);

            if (post == null)
            {
                HttpResponseMessage response = await _httpClient.GetAsync(string.Format(ApiPostQueryString, postId));
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WriteAsPostResponse writeAsResponse = JsonSerializer.Deserialize<WriteAsPostResponse>(responseBody, _jsonSerializerOptions);

                    if (writeAsResponse != null && writeAsResponse.Data != null)
                    {
                        post = writeAsResponse.Data;
                        _cache.SaveDataToCache<Post>(cacheKey, post);
                    }
                }
            }

            return post;
        }

        public async Task<List<Post>> Search(string alias, string searchKey, SortOrder sortOrder = SortOrder.Descending)
        {
            List<Post> searchResults = new List<Post>();

            List<Post> allPosts = await GetAllPosts(alias, sortOrder);
            if (allPosts != null)
            {
                foreach (var post in allPosts)
                {
                    if ((!string.IsNullOrEmpty(post.Title) && post.Title.Contains(searchKey, StringComparison.OrdinalIgnoreCase)) ||
                        post.Body.Contains(searchKey, StringComparison.OrdinalIgnoreCase))
                    {
                        searchResults.Add(post);
                    }
                }
            }

            return searchResults;
        }

        private int GetTotalNumberOfPages(int totalNumberOfPosts)
        {
            // Write.as has 10 posts per page. So dividing totalNumberOfPosts by 10 and rounding up should give you the total number of pages.
            float tempMax = totalNumberOfPosts / 10f;
            int totalNumberOfPages = (int)Math.Ceiling(tempMax);
            return totalNumberOfPages;
        }

        private List<Post> SortPosts(SortOrder sortOrder, List<Post> allPosts)
        {
            if (sortOrder == SortOrder.Ascending)
            {
                allPosts = allPosts.OrderBy(p => p.CreateDate).ToList();
            }

            return allPosts;
        }
    }
}
