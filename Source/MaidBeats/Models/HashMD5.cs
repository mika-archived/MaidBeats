using Newtonsoft.Json;

namespace MaidBeats.Models
{
    // ReSharper disable once InconsistentNaming
    public class HashMD5
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }
    }
}