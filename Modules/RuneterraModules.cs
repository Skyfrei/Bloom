using Discord.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace _02_commands_framework.Services
{
    public class LeaderBoardInfo
    {
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Lp { get; set; }

        public LeaderBoardInfo(string name, int rank, int lp)
        {
            this.Name = name;
            this.Rank = rank;
            this.Lp = lp;
        }
    }
    public class RuneTerra : ModuleBase<SocketCommandContext>
    {
        private readonly string runeterraApiToken = File.ReadAllText("Data Dragon/Runeterra/RuneterraApiToken.txt");

        [Command("leaderboard")]
        public async Task ShowLeaderboard(string server = "Europe", IUser user = null)
        {
            server = server.ToLower();
            user = user ?? Context.User;
            if (server != "americas" && server != "europe" && server != "asia" && server != "sea")
            {
                await ReplyAsync("Wrong server");
                return;
            }
            JObject matchArray = JObject.Parse((await LeagueOfLegends.client.GetStringAsync($"https://{server}.api.riotgames.com/lor/ranked/v1/leaderboards?api_key={runeterraApiToken}")));
            List<LeaderBoardInfo> listOfPlayers = new List<LeaderBoardInfo>();

            for (int i = 0; i < 10; i++)
            {
                LeaderBoardInfo playerObject = new LeaderBoardInfo(matchArray["players"][i]["name"].ToString(), Int32.Parse(matchArray["players"][i]["rank"].ToString()) + 1, Int32.Parse(matchArray["players"][i]["lp"].ToString()));
                listOfPlayers.Add(playerObject);
            }
            
            var embed = new EmbedBuilder()
            {
                Color = Color.Orange,
                Title = "Leaderboard",
                Timestamp = DateTime.Now,
                ThumbnailUrl = "https://www.rocketreferrals.com/wp-content/uploads/2019/11/Leaderboard-blog.png",
                Footer = new EmbedFooterBuilder()
                        .WithText($"{user.Username}")
                        .WithIconUrl($"{user.GetAvatarUrl()}")
            };
            string playerNames = "";
            string playerRank = "";
            string playerLp = "";
            foreach (LeaderBoardInfo element in listOfPlayers)
            {
                playerNames += $"{element.Name}\n";
                playerRank += $"{element.Rank}\n";
                playerLp += $"{element.Lp}\n";
            }

            embed.AddField("Name", $"{playerNames}", true);
            embed.AddField("Rank", $"{playerRank}", true);
            embed.AddField("League Points", $"{playerLp}", true);
            await ReplyAsync("", false, embed.Build());

        }

    }
}
