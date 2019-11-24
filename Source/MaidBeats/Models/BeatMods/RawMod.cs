using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MaidBeats.Models.BeatMods
{
    public class RawMod
    {
        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("dependencies")]
        public List<DependencyMod> Dependencies { get; set; }

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

        //
        // helper properties
        //
        public bool HasDependencies => Dependencies?.Count > 0;

        //
        // helper methods
        //
        public override bool Equals(object obj)
        {
            return obj is RawMod other && other.Name == Name && other.Version == Version;
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return Name.GetHashCode() + Version.GetHashCode();
        }

        #region Version

        private Version _version;

        [JsonProperty("version")]
        public string Version
        {
            get => $"{_version.Major}.{_version.Minor}.{_version.Build}";
            set
            {
                if (value.StartsWith("v"))
                    _version = new Version(value.Substring(1));
                else if (value.EndsWith("."))
                    _version = new Version(value.Substring(0, value.Length - 1));
                else
                    _version = new Version(value);
            } // patch for version "2.0.0.", why?
        }

        #endregion
    }

    public class DependencyMod : RawMod
    {
        [JsonProperty("dependencies")]
        public new IEnumerable<string> Dependencies { get; set; }

        public DependencyMod()
        {
            //
        }

        public DependencyMod(RawMod mod)
        {
            Author = mod.Author;
            AuthorId = mod.AuthorId;
            Category = mod.Category;
            Dependencies = mod.Dependencies.Select(w => w.Id).ToList();
            Downloads = mod.Downloads;
            GameVersion = mod.GameVersion;
            Id = mod.Id;
            Link = mod.Link;
            Name = mod.Name;
            IsRequired = mod.IsRequired;
            UpdatedDate = mod.UpdatedDate;
            UploadDate = mod.UploadDate;
            Status = mod.Status;
            Version = mod.Version;
        }
    }
}