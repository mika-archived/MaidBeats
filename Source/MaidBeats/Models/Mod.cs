using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MaidBeats.Models
{
    public class Mod
    {
        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("dependencies")]
        public IEnumerable<Mod> Dependencies { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("downloads")]
        public IEnumerable<Download> Downloads { get; set; }

        [JsonProperty("gameVersion")]
        public string GameVersion { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("required")]
        public bool IsRequired { get; set; }

        [JsonProperty("updatedDate")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("uploadDate")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime UploadDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}