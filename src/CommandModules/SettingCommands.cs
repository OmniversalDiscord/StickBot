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
    
    [SlashCommand("settings", "List all settings for StickBot")]
    public async Task ListSettings(InteractionContext ctx)
    {
        var settings = await GetSettingsOrReturnError(ctx);

        if (settings == null)
            return;

        var settingsList = new StringBuilder();
        var bonkMin = TimeSpan.FromMilliseconds(settings.BonkMin);
        var bonkMax = TimeSpan.FromMilliseconds(settings.BonkMax);
        var stealCooldown = TimeSpan.FromMilliseconds(settings.StealCooldown);

        settingsList.AppendLine("*Roles*\n");
        if (ctx.Guild.Roles.TryGetValue(settings.StickRole, out var stickRole))
        {
            settingsList.AppendLine($"**Stick**: {stickRole.Mention}");
        }
        else
        {
            settingsList.AppendLine("**Stick**: :warning: **None set!**");
        }
        settingsList.AppendLine("Set with ``/set role stick``\n");
        
        if (ctx.Guild.Roles.TryGetValue(settings.BonkedRole, out var bonkedRole))
        {
            settingsList.AppendLine($"**Bonk**: {bonkedRole.Mention}");
        }
        else
        {
            settingsList.AppendLine("**Bonk**: :warning: **None set!**");
        }
        settingsList.AppendLine("Set with ``/set role bonked``\n");
        settingsList.AppendLine("*Bonks*\n");
        settingsList.AppendLine($"**Minimum time until bonk:** {bonkMin.Humanize(3)}");
        settingsList.AppendLine("Set with ``/set bonkmin <time>``\n");
        settingsList.AppendLine($"**Maximum time until bonk:** {bonkMax.Humanize(3)}");
        settingsList.AppendLine("Set with ``/set bonkmax <time>``\n");
        settingsList.AppendLine("*Stealing*\n");
        settingsList.AppendLine($"**Chance to steal a stick:** {settings.StealSuccessChance * 100}%");
        settingsList.AppendLine("Set with ``/set stealchance success <chance>``\n");
        settingsList.AppendLine($"**Chance to be bonked by stick:** {settings.StealBonkChance * 100}%");
        settingsList.AppendLine("Set with ``/set stealchance bonk <chance>``\n");
        settingsList.AppendLine($"**Cooldown between steals:** {stealCooldown.Humanize(3)}");
        settingsList.AppendLine("Set with ``/set stealcooldown <time>``\n");

        var embed = new DiscordEmbedBuilder()
            .WithTitle("StickBot Settings")
            .WithDescription(settingsList.ToString())
            .Build();
        
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }
}