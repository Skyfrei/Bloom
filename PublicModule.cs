using System.IO;
using System.Threading.Tasks;
using System;
using Discord;
using Discord.Commands;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using _02_commands_framework.Services;

namespace _02_commands_framework.Modules
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("cat")]
        public async Task CatAsync()
        {
            var stream = await PictureService.GetCatPictureAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }

        [Command("about")]
        [Alias("info", "donate")]
        public async Task BotInfo()
        {
            var description = "My name is Bloom and I am an ongoing project and slave to my creator. According to him I will currently be a multi-purpose human mainly used to handle server decisions and help with video games. My creator needs money so he doesn't live on the streets, he's a c-word sometimes and works hard to finish me everyday :wink:\n\n If you think you want to support me you should donate to my creator on the following link.\n Paypal: \t\thttps://www.paypal.com/paypalme/Klavio";

            await ReplyAsync(description);
        }

        [Command("ban")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            await user.Guild.AddBanAsync(user, reason: reason);
            await ReplyAsync("ok!");
        }
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            => ReplyAsync('\u200B' + text);

        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));

        [Command("guild_only")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public Task GuildOnlyCommand()
            => ReplyAsync("Nothing to see here!");
    }

    [Group("rune")]
    public class RuneTerra : ModuleBase<SocketCommandContext>
    {
        // [Command("randomDeck")]
        // [Alias("")]

    }
    public class LeagueOfLegends : ModuleBase<SocketCommandContext>
    {
        JObject data = JObject.Parse(File.ReadAllText("champions.json"));
        [Command("build")]
        [Alias("herobuild", "champbuild")]

        public async Task ChampionBuildAsync(string champion = null)
        {   
            try
            {
                champion = char.ToUpper(champion[0]) + champion.Substring(1);
                if ($"{data["data"][champion]["name"]}".ToString().ToLower() == champion.ToLower())
                    await ReplyAsync($"https://u.gg/lol/champions/{champion}/build");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                await ReplyAsync("Champion doesn't exist.");
            }
        }
    }
}