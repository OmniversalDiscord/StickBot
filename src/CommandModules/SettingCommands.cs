using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using StickBot.Models;
using StickBot.Services;

using Humanizer;

namespace StickBot.CommandModules;

using DSharpPlus.SlashCommands;

public class SettingCommands : SlashCommandModule
{
    private readonly SettingsService _settingsService;
    
    public SettingCommands(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    private async Task<Settings?> GetSettingsOrReturnError(InteractionContext ctx)
    {
        try
        {
            var settings = await _settingsService.GetServerSettings(ctx.Guild.Id);
            return settings;
        }
        catch (ServerNotFoundException)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("**An error occured**\nDiscord server not found."));
            return null;
        }
    }

    private static string GetRolesPage(Settings settings, DiscordGuild guild)
    {
        var page = new StringBuilder();
        if (guild.Roles.TryGetValue(settings.StickRole, out var stickRole))
        {
            page.AppendLine($"**Stick**: {stickRole.Mention}");
        }
        else
        {
            page.AppendLine("**Stick**: :warning: **None set!**");
        }
        page.AppendLine("Set with ``/set role stick <role>``\n");
        
        if (guild.Roles.TryGetValue(settings.BonkedRole, out var bonkedRole))
        {
            page.AppendLine($"**Bonk**: {bonkedRole.Mention}");
        }
        else
        {
            page.AppendLine("**Bonk**: :warning: **None set!**");
        }
        page.AppendLine("Set with ``/set role bonked <role>``\n");

        return page.ToString();
    }

    private static string GetBonksPage(Settings settings)
    {
        var page = new StringBuilder();
        var bonkMin = TimeSpan.FromMilliseconds(settings.BonkMin);
        var bonkMax = TimeSpan.FromMilliseconds(settings.BonkMax);
        
        page.AppendLine($"**Minimum time until bonk:** {bonkMin.Humanize(3)}");
        page.AppendLine("Set with ``/set bonkmin <time>``\n");
        page.AppendLine($"**Maximum time until bonk:** {bonkMax.Humanize(3)}");
        page.AppendLine("Set with ``/set bonkmax <time>``\n");
        return page.ToString();
    }

    private static string GetStealingPage(Settings settings)
    {
        var page = new StringBuilder();
        var stealCooldown = TimeSpan.FromMilliseconds(settings.StealCooldown);
        
        page.AppendLine($"**Chance to steal a stick:** {settings.StealSuccessChance * 100}%");
        page.AppendLine("Set with ``/set stealchance success <chance>``\n");
        page.AppendLine($"**Chance to be bonked by stick:** {settings.StealBonkChance * 100}%");
        page.AppendLine("Set with ``/set stealchance bonk <chance>``\n");
        page.AppendLine($"**Cooldown between steals:** {stealCooldown.Humanize(3)}");
        page.AppendLine("Set with ``/set stealcooldown <time>``\n");
        
        return page.ToString();
    }

    [SlashCommand("settings", "List all settings for StickBot")]
    public async Task ListSettings(InteractionContext ctx)
    {
        var settings = await GetSettingsOrReturnError(ctx);

        if (settings == null)
            return;

        var page = new StringBuilder();
        page.AppendLine(":brown_circle: **Roles**");
        page.AppendLine(GetRolesPage(settings, ctx.Guild));
        page.AppendLine(":anger: **Bonks**");
        page.AppendLine(GetBonksPage(settings));
        page.AppendLine(":moneybag: **Stick stealing**");
        page.AppendLine(GetStealingPage(settings));
        
        var embed = new DiscordEmbedBuilder().WithTitle("StickBot Settings").WithDescription(page.ToString());
        
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()));
    }
}