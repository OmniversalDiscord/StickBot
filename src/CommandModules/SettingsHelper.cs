using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using StickBot.Models;
using StickBot.Services;

namespace StickBot.CommandModules;

public static class SettingsHelper
{
    public static async Task<Settings?> GetSettingsOrReturnError(this SettingsService settingsService, InteractionContext ctx)
    {
        try
        {
            var settings = await settingsService.GetServerSettings(ctx.Guild.Id);
            return settings;
        }
        catch (ServerNotFoundException)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("**An error occured**\nDiscord server not found."));
            return null;
        }
    }
}