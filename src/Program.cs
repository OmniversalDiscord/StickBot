using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StickBot;
using StickBot.CommandModules;
using StickBot.Models;
using StickBot.Services;

#if DEBUG
const string token = Credentials.DiscordToken;
#else
const string token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
#endif

var services = new ServiceCollection();

services.AddDbContext<BotDbContext>();
services.AddScoped<SettingsService>();

var discord = new DiscordClient(new DiscordConfiguration
{
    Token = token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.AllUnprivileged
});

discord.Ready += (sender, _) =>
{
    sender.Logger.LogInformation("Connected as {}#{}!", sender.CurrentUser.Username, sender.CurrentUser.Discriminator);
    return Task.CompletedTask;
};

discord.GuildCreated += async (sender, eventArgs) =>
{
    // Check database for existing settings, otherwise create defaults
    await using var dbContext = new BotDbContext();
    var settingService = new SettingsService(dbContext);

    var role = await eventArgs.Guild.CreateRoleAsync("Stick", Permissions.None, new DiscordColor(0x94, 0x6c, 0x2e));

    await settingService.CreateDefaultSettings(eventArgs.Guild.Id, role.Id);
    sender.Logger.LogInformation("Joined a new server: \"{}\"", eventArgs.Guild.Name);
    // TODO: Welcome message
};

discord.GuildDeleted += async (sender, eventArgs) =>
{
    await using var dbContext = new BotDbContext();
    var settingService = new SettingsService(dbContext);

    await settingService.DeleteSettings(eventArgs.Guild.Id);
    sender.Logger.LogInformation("Left a server: \"{}\"", eventArgs.Guild.Name);
};

var slash = discord.UseSlashCommands(new SlashCommandsConfiguration
{
    Services = services.BuildServiceProvider()
});

#if DEBUG
slash.RegisterCommands<StickCommands>(Credentials.TestServer);
slash.RegisterCommands<SettingCommands>(Credentials.TestServer);
#else
slash.RegisterCommands<StickCommands>();
slash.RegisterCommands<SettingCommands>();
#endif

await discord.ConnectAsync();
await Task.Delay(-1);