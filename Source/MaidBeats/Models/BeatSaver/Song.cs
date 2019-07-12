using System.Collections.Generic;

using MaidBeats.Models.BeatSaver;

using Newtonsoft.Json;

namespace MaidBeats.Models
{
    // see also : https://github.com/Kylemc1413/SongCore#infodat-explanation
    public class Song
    {
        [JsonProperty("_beatsPerMinute")]
        public long BeatsPerMinute { get; set; }

        [JsonProperty("_coverImageFilename")]
        public string CoverImageFilename { get; set; }

        [JsonProperty("_customData")]
        public SongCustomData CustomData { get; set; }

        [JsonProperty("_difficultyBeatmapSets")]
        public IEnumerable<DifficultyBeatmapSet> DifficultyBeatmapSets { get; set; }

        [JsonProperty("_environmentName")]
        public string EnvironmentName { get; set; }

        [JsonProperty("_levelAuthorName")]
        public string LevelAuthorName { get; set; }

        [JsonProperty("_previewDuration")]
        public long PreviewDuration { get; set; }

        [JsonProperty("_previewStartTime")]
        public long PreviewStartTime { get; set; }

        [JsonProperty("_songAuthorName")]
        public string SongAuthorName { get; set; }

        [JsonProperty("_songImageFilename")]
        public string SongImageFilename { get; set; }

        [JsonProperty("_songFilename")]
        public string SongFilename { get; set; }

        [JsonProperty("_songName")]
        public string SongName { get; set; }

        [JsonProperty("_songSubName")]
        public string SongSubName { get; set; }

        [JsonProperty("_songTimeOffset")]
        public long SongTimeOffset { get; set; }

        [JsonProperty("_shuffle")]
        public long Shuffle { get; set; }

        [JsonProperty("_shufflePeriod")]
        public double ShufflePeriod { get; set; }

        [JsonProperty("_version")]
        public string Version { get; set; }
    }
}