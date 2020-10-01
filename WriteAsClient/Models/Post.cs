using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WriteAs.Client.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Appearance { get; set; }
        public string Language { get; set; }
        public bool Rtl { get; set; }

        [JsonPropertyName("created")]
        public DateTime CreateDate { get; set; }
        [JsonPropertyName("updated")]
        public DateTime LastUpdatedDate { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public List<string> Tags { get; set; }
        public int Views { get; set; }
    }
}
