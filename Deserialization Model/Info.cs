using Newtonsoft.Json;

namespace ChampionData
{
    public class Info 
    {
        [JsonProperty("attack")]
        public double Attack { get; set; }
        
        [JsonProperty("defense")]
        public double Defense { get; set; }

        [JsonProperty("magic")]
        public double Magic { get; set; }

        [JsonProperty("difficulty")]
        public double Difficulty { get; set; }
    }
}