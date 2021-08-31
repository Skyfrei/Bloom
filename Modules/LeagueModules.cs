using System.IO;
using System.Threading.Tasks;
using System;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace _02_commands_framework.Services
{
    public class LeagueOfLegends : ModuleBase<SocketCommandContext>
    {
        JObject data = JObject.Parse(File.ReadAllText("champions.json"));

        [Command("build")]
        [Alias("herobuild", "champbuild")]
        public async Task ChampionBuildAsync(string champion = null, IUser user = null)
        {  
            // Getting user that's typing the message 
            try 
            {
                champion = char.ToUpper(champion[0]) + champion.Substring(1);
            }
            catch (Exception)
            {
                await ReplyAsync("No champion selected.");
                return;
            }
            
            user = user ?? Context.User;

            // Creating the embeded message. It makes the bot's message looky pretty when its posted on the channel
            // The error catching is used so the user understands that he wrote a wrong champion's name or the champion doesn't exist.
            // The champions name is formated so it matches the name of the champion in the json file. Capital letter on the first letter. 
            var embed = new EmbedBuilder()
            {
                Color = Color.Blue,
                Description = $"{champion} builds and statistics.",
                Title = champion,
                Url = $"https://u.gg/lol/champions/{champion}/build",
                ThumbnailUrl = $"https://static.u.gg/assets/lol/riot_static/11.15.1/img/champion/{champion}.png",
                Timestamp = DateTime.UtcNow,
                Footer = new EmbedFooterBuilder()
                        .WithText($"{user.Username}")
                        .WithIconUrl($"{user.GetAvatarUrl()}")
            };
            embed.AddField("Runes", $"https://u.gg/lol/champions/{champion}/runes", true);
            embed.AddField("Summoner Spells", $"https://u.gg/lol/champions/{champion}/runes", true);

            try
            {
                if ($"{data["data"][champion]["name"]}".ToString().ToLower() == champion.ToLower())
                    await ReplyAsync("", false, embed.Build());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                await ReplyAsync("Champion doesn't exist.");
            }
        }

        [Command("profile")]
        [Alias("me", "aboutme")]
        public async Task Profile(string profile = null)
        {
            
            
        }
    }
}

