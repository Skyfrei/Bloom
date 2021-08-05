using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using System.Threading.Tasks;

public class CommandHandler
{
    private readonly DiscordSocketClient client;
    private readonly CommandService commands;

    public CommandHandler(DiscordSocketClient _client, CommandService _commands)
    {
        commands = _commands;
        client = _client;
    }

    public async Task InstallCommandsAsync()
    {
        client.MessageReceived += HandleCommandAsync;

        await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        int argPos = 0;

        if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) || message.Author.IsBot ) return;

        var context = new SocketCommandContext(client, message);

        await commands.ExecuteAsync(
            context: context,
            argPos: argPos, 
            services: null
        );
    }

}