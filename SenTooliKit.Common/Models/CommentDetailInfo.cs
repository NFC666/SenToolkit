using Newtonsoft.Json;

namespace SenTooliKit.Common.Models
{

    public class Reply
    {
        [JsonProperty("rpid")]
        public long RpId { get; set; }
        [JsonProperty("mid_str")]
        public string SenderId { get; set; }
        [JsonProperty("ctime")]
        public long SendTime { get; set; }
        public int Like { get; set; }
        public Content Content { get; set; }
        public List<Reply> Replies { get; set; }
    }

    public class Content
    {
        public string Message { get; set; }
    }
}