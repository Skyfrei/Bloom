using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChampionData
{
    public class Champion
    {
        [JsonProperty("Id")]
        public string Version { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Blurb { get; set; }
        public Info Info { get; set; }
        public List<string> Tags { get; set; }
        public string Partype { get; set; }
        public Stats Stats { get; set; }
    }
}