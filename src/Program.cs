using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using StickBot;
using StickBot.CommandModules;

#if DEBUG
const string token = Credentials.DiscordToken;
#else
const string token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
#endif

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.AllUnprivileged
});

discord.Ready += (sender, eventArgs) =>
{
    sender.Logger.LogInformation("Connected as {}#{}!", sender.CurrentUser.Username, sender.CurrentUser.Discriminator);
    return Task.CompletedTask;
};

discord.GuildCreated += async (sender, eventArgs) =>
{
    // TODO: Check database for existing stick role
    // await eventArgs.Guild.CreateRoleAsync("Stick", Permissions.None, DiscordColor.Brown);
    // TODO: Welcome message
};

var slash = discord.UseSlashCommands();

#if DEBUG
slash.RegisterCommands<StickCommands>(Credentials.TestServer);
slash.RegisterCommands<ConfigCommands>(Credentials.TestServer);
#else
slash.RegisterCommands<StickCommands>();
slash.RegisterCommands<ConfigCommands>();
#endif

await discord.ConnectAsync();
await Task.Delay(-1);