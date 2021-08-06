using Discord.Commands;
using System;
using Discord.WebSocket;
using System.Threading.Tasks;


namespace Modules{
    [Group("sample")]
    public class SampleModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
        => ReplyAsync('\u200B' + echo);

        [Command("square")]
        [Summary("Squares a number.")]
        public async Task SquareAsync(
                [Summary("The number to square.")]
                int num)
        {
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }

        [Command("userinfo")]
        [Summary 
        ("Returns info about the current user, or the user paramter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync(
            [Summary("The (optional) user to get info from.")]
            SocketUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }
        
    }
}
