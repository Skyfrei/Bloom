using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Bloom
{
    class Program
    {
        private DiscordSocketClient client;
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            
            client.Log += Log;
            string token = "ODcyNzY2OTMyMzk3NDg2MTUw.YQupiw.5NasIh63nbNasF5Kq2vwea8YFKY";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());

            return Task.CompletedTask;
        }
    }
}
