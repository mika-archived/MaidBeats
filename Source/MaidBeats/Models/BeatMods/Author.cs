using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MaidBeats.Models.BeatMods
{
    public class Author
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("lastLogin")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime LastLogin { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}