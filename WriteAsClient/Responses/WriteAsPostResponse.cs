using WriteAs.NET.Client.Models;

namespace WriteAs.NET.Client.Responses
{
    public class WriteAsPostResponse
    {
        public int Code { get; set; }
        public Post Data { get; set; }
    }
}
