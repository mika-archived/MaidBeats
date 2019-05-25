using System.Collections.Generic;

using MaidBeats.Models.BeatMods;

using Newtonsoft.Json;

namespace MaidBeats.Models
{
    public class Song
    {
        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("beatsPerMinute")]
        public long BeatsPerMinute { get; set; }

        [JsonProperty("coverImagePath")]
        public string CoverImagePath { get; set; }

        [JsonProperty("difficultyLevels")]
        public IEnumerable<DifficultyLevel> DifficultyLevels { get; set; }

        [JsonProperty("environmentName")]
        public string EnvironmentName { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("previewDuration")]
        public long PreviewDuration { get; set; }

        [JsonProperty("previewStartTime")]
        public long PreviewStartTime { get; set; }

        [JsonProperty("songName")]
        public string SongName { get; set; }

        [JsonProperty("songSubName")]
        public string SongSubName { get; set; }
    }
}