using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WriteAs.NET.Client.Models
{
    public class CollectionData
    {
        public string Alias { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonPropertyName("style_sheet")]
        public string StyleSheet { get; set; }

        [JsonPropertyName("private")]
        public bool IsPrivate { get; set; }

        [JsonPropertyName("total_posts")]
        public int TotalNumberOfPosts { get; set; }

        public List<Post> Posts { get; set; }
    }
}
