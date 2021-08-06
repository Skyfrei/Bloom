using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Threading;
using Discord.Commands;
using Modules;
using System.Reflection;

namespace Bloom
{
    class Program
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            client.Log += LogASync;
            client.Ready += ReadyAsync;
            client.MessageReceived += MessageReceivedAsync;
            
            
        }

        public async Task MainAsync()
        {
            
            string token = "ODcyNzY2OTMyMzk3NDg2MTUw.YQupiw.5NasIh63nbNasF5Kq2vwea8YFKY";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private Task LogASync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            string description = "I am Bloom, a creature whose lifespan ";
            // The bot should never respond to itself.
            if (message.Author.Id == client.CurrentUser.Id)
                return;

            if (message.Content.ToLower()== "!ping")
                await message.Channel.SendMessageAsync("Pong");

            if (message.Content.ToLower() == "!bloom")
                await message.Channel.SendMessageAsync(".");

            if (message.Content.ToLower() == "!name")
                await message.Channel.SendMessageAsync("Are you blind kid?");

        }
    }
}
