using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChampionData
{
    public class ChampionData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
        
        [JsonProperty("data")]
        public Dictionary<string, Champion> data;    
    }
}
