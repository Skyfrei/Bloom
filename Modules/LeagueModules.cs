using System.IO;
using System.Threading.Tasks;
using System;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.SQLite;
using System.Net.Http;
using System.Collections.Generic;
using ChampionData;


namespace _02_commands_framework.Services
{
    public class LeagueOfLegends : ModuleBase<SocketCommandContext>
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string apiKey = File.ReadAllText("Data Dragon/ApiToken.txt");
        JObject data = JObject.Parse(File.ReadAllText("Data Dragon/champions.json"));

        [Command("build")]
        [Alias("herobuild", "champbuild")]
        public async Task ChampionBuildAsync(string champion = null, IUser user = null)
        {  
            // Getting Discord user ID so it shows their league of legends profile
            user = user ?? Context.User;
            try 
            {
                champion = char.ToUpper(champion[0]) + champion.Substring(1);
            }
            catch (Exception)
            {
                await ReplyAsync("No champion selected.");
                return;
            }
            // Creating the embeded message. It makes the bot's message looky pretty when its posted on the channel
            // The error catching is used so the user understands that he wrote a wrong champion's name or the champion doesn't exist.
            // The champions name is formated so it matches the name of the champion in the json file. Capital letter on the first letter. 
            var embed = new EmbedBuilder()
            {
                Color = Color.Blue,
                Description = $"{champion} builds and statistics.",
                Title = champion,
                Url = $"https://u.gg/lol/champions/{champion}/build",
                ThumbnailUrl = $"https://static.u.gg/assets/lol/riot_static/11.19.1/img/champion/{champion}.png",
                Timestamp = DateTime.UtcNow,
                Footer = new EmbedFooterBuilder()
                        .WithText($"{user.Username}")
                        .WithIconUrl($"{user.GetAvatarUrl()}")
            };
            embed.AddField("Runes", $"[Link](https://u.gg/lol/champions/{champion}/runes)", true);
            embed.AddField("Items", $"[Link](https://u.gg/lol/champions/{champion}/items)", true);

            try
            {
                if ($"{data["data"][champion]["id"]}".ToString().ToLower() == champion.ToLower())
                    await ReplyAsync("", false, embed.Build());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                await ReplyAsync("Champion doesn't exist.");
            }
        }

        // Adding league accounts in SQLite if the server is correct
        [Command("register")]
        [Alias("addProfile")]
        public async Task AddProfile(string profileName, string server, IUser user = null)
        {
            user = user ?? Context.User;
            List<string> serverNames = new List<string>{"euw", "eun", "na", "br", "ru", "oce", "tr", "kr", "lan", "jp"};
            JObject responseString = new JObject(); 

            // Checking for server errors and printing out server list in a discord message if error is found

            SQLiteConnection conn = new SQLiteConnection("Data Source= database.db; Version=3; New=True; Compress=True;");
            try
            {
                responseString = JObject.Parse((await client.GetStringAsync($"https://{server}1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{profileName}?api_key={apiKey}")));
            }
            catch
            {
                string message = "Enter a server from the list.```";
                foreach(string ser in serverNames)
                {
                    message += $"{ser}\n";
                }
                message += "```";
                await ReplyAsync(message);
            }
            
            try
            {
                conn.Open();
                string readString = "";

                SQLiteCommand command = new SQLiteCommand();
                command = conn.CreateCommand();
                command.CommandText = $"SELECT Region FROM Regions WHERE Region = '{server}'";
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    readString = reader.GetString(0);
                }
                command = conn.CreateCommand();
                command.CommandText = $"INSERT INTO Users (Id, Summ_name, Date_added, Region, RiotId, Puuid) VALUES ('{user.Id}', '{profileName}', '{DateTime.UtcNow}', '{readString}', '{responseString["id"]}', '{responseString["puuid"]}')";
                command.ExecuteNonQuery();
                await ReplyAsync("Account added");
            }
            catch
            {
                await ReplyAsync($"{user.Username} has already linked an account.");
            }
        }

        // Showing user profile based on what he has registered on the database
        [Command("profile")]
        [Alias("aboutMe", "me")]
        public async Task ShowProfile(IUser user = null)
        {
            user = user ?? Context.User;
            string summonerName = "";
            string summRegion = "";
            string summonerId = "";
            List<JObject> championMastery = new List<JObject>();
            // Getting equipped player image url
            // Sending embed message that holds player profile information
            // If summoner name is null or "" it means the user hasn't registered an account yet, and is informed of it

            SQLiteConnection conn = new SQLiteConnection("Data Source= database.db; Version=3; New=True; Compress=True;");
            try
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command = conn.CreateCommand();
                command.CommandText = $"SELECT Summ_name, Region, RiotId FROM Users WHERE Id = '{user.Id}'";
                
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    summonerName = reader["Summ_name"].ToString(); 
                    summRegion = reader["Region"].ToString();  
                    summonerId = reader["RiotId"].ToString();
                }
                if (summonerName == "" || summonerName == null ) throw new ArgumentException((await ReplyAsync("This user doesn't have an account. Type !register [accountName] [region] to create one `!register MaxxBurn euw`")).ToString());
                
                JObject responseString = JObject.Parse((await client.GetStringAsync($"https://{summRegion}1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}?api_key={apiKey}")));
                JArray championMasterArray = JArray.Parse((await client.GetStringAsync($"https://{summRegion}1.api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-summoner/{summonerId}?api_key={apiKey}")));
                for (int i = 0; i < 3; i++)
                {
                    championMastery.Add(JObject.Parse(championMasterArray[i].ToString()));
                }

                summonerName = summonerName.Replace(" ", "");
                var embed = new EmbedBuilder()
                {
                    Color = Color.Orange,
                    Title = summonerName,
                    ThumbnailUrl = $"https://ddragon.leagueoflegends.com/cdn/11.19.1/img/profileicon/{responseString["profileIconId"]}.png",
                    Url = $"https://u.gg/lol/profile/{summRegion}1/{summonerName}",
                    
                    Timestamp = DateTime.UtcNow,
                    Footer = new EmbedFooterBuilder()
                            .WithText($"{user.Username}")
                            .WithIconUrl($"{user.GetAvatarUrl()}")
                };
                embed.AddField("Live Game", $"[Link](https://u.gg/lol/profile/{summRegion}1/{summonerName}/live-game)", true);
                embed.AddField("Champion Stats", $"[Link](https://u.gg/lol/profile/{summRegion}1/{summonerName}/champion-stats)", true);


                //Get champion names here !!!!!!!!!!!!!!!!!
                foreach(JObject champ in championMastery)
                {
                    embed.AddField("Played Champion", $"[Link](https://ddragon.leagueoflegends.com/cdn/11.19.1/img/champion/{summonerName}.png)", true);
                }
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Deleting the profile saved on the sqlite database
        [Command("deleteProfile")]
        public async Task DeleteProfile(IUser user = null)
        {
            user = user ?? Context.User;

            SQLiteConnection conn = new SQLiteConnection("Data Source= database.db; Version=3; New=True; Compress=True;");
            try
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command = conn.CreateCommand();
                command.CommandText = $"DELETE FROM Users WHERE Id = '{user.Id}'";
                command.ExecuteNonQuery();
                await ReplyAsync("Done");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Gets weekly champion rotation when user enters !rotation
        //Need to add multiple images here !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        [Command("rotation")]
        public async Task ChampionRotation(IUser user = null)
        {
            user = user ?? Context.User;
            JObject rotationString = JObject.Parse(await client.GetStringAsync($"https://euw1.api.riotgames.com/lol/platform/v3/champion-rotations?api_key={apiKey}"));
            string jsonString = File.ReadAllText("Data Dragon/champions.json");
            ChampionDataModel dataDeserialized = JsonConvert.DeserializeObject<ChampionDataModel>(jsonString);
            
            
            
            var embed = new EmbedBuilder()
            {
                Color = Color.Green,
                Title = "Champion Rotation",
                Timestamp = DateTime.UtcNow,
                Footer = new EmbedFooterBuilder()
                        .WithText($"{user.Username}")
                        .WithIconUrl($"{user.GetAvatarUrl()}")
            };
            foreach (var element in dataDeserialized.Data)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (dataDeserialized.Data[element.Key].Key == rotationString["freeChampionIds"][i].ToString())
                    {
                        embed.AddField($"Champion", element.Value.Name, true);
                    }
                }
            }
            await ReplyAsync("", false, embed.Build());
        }
    }
}

