using System.Text;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;

using StickBot.Models;
using StickBot.Services;

namespace StickBot.CommandModules;

// TODO: HEAVY REFACTORING
public class SettingCommands : SlashCommandModule
{
    private readonly SettingsService _settingsService;
    
    public SettingCommands(SettingsService settingsService)
    {
        _settingsService = settingsService;
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
            page.AppendLine($"**Bonked**: {bonkedRole.Mention}");
        }
        else
        {
            page.AppendLine("**Bonked**: :warning: **None set!**");
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
        page.AppendLine("Set with ``/set bonk min <time>``\n");
        page.AppendLine($"**Maximum time until bonk:** {bonkMax.Humanize(3)}");
        page.AppendLine("Set with ``/set bonk max <time>``\n");
        return page.ToString();
    }

    private static string GetStealingPage(Settings settings)
    {
        var page = new StringBuilder();
        var stealCooldown = TimeSpan.FromMilliseconds(settings.StealCooldown);
        
        page.AppendLine($"**Chance to steal a stick:** {settings.StealSuccessChance * 100}%");
        page.AppendLine("Set with ``/set steal chance success <chance>``\n");
        page.AppendLine($"**Chance to be bonked by stick:** {settings.StealBonkChance * 100}%");
        page.AppendLine("Set with ``/set steal chance bonk <chance>``\n");
        page.AppendLine($"**Cooldown between steals:** {stealCooldown.Humanize(3)}");
        page.AppendLine("Set with ``/set steal cooldown <time>``\n");
        
        return page.ToString();
    }

    [SlashCommand("settings", "List all settings for StickBot")]
    public async Task ListSettings(InteractionContext ctx)
    {
        var settings = await _settingsService.GetSettingsOrReturnError(ctx);
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

    [SlashCommandGroup("set", "Set a setting for StickBot")]
    public class Setters : SlashCommandModule
    {
        [SlashCommandGroup("role", "Set the roles used by StickBot")]
        public class Roles : SlashCommandModule
        {
            private readonly SettingsService _settingsService;
            
            public Roles(SettingsService settingsService)
            {
                _settingsService = settingsService;
            }
            
            [SlashCommand("stick", "Set the role given to users with sticks")]
            public async Task SetStickRole(InteractionContext ctx, [Option("role", "The role to use")] DiscordRole role)
            {
                var settings = await _settingsService.GetSettingsOrReturnError(ctx);
                if (settings == null)
                    return;

                await _settingsService.UpdateServerSetting(ctx.Guild.Id, Setting.StickRole, role.Id);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Set the stick role to {role.Mention}"));
            }
            
            [SlashCommand("bonked", "Set the role given to bonked users")]
            public async Task SetBonkedRole(InteractionContext ctx, [Option("role", "The role to use")] DiscordRole role)
            {
                var settings = await _settingsService.GetSettingsOrReturnError(ctx);
                if (settings == null)
                    return;
                
                await _settingsService.UpdateServerSetting(ctx.Guild.Id, Setting.BonkedRole, role.Id);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Set the bonked role to {role.Mention}"));
            }
        }

        [SlashCommandGroup("bonk", "Set the settings for bonks")]
        public class Bonks : SlashCommandModule
        {
            private readonly SettingsService _settingsService;
            
            public Bonks(SettingsService settingsService)
            {
                _settingsService = settingsService;
            }

            [SlashCommand("min", "Set the minimum time before a stick might bonk")]
            public async Task SetBonkMin(InteractionContext ctx,
                [Option("time", "Minimum time before bonk (d, h, m, s)")] string time)
            {
                var settings = await _settingsService.GetSettingsOrReturnError(ctx);
                if (settings == null)
                    return;

                var timeSpan = TimeSpanConverter.Convert(time);
                if (!timeSpan.HasValue)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("Invalid time format, use ``xd xh xm xs``. For example ``1d 2h 3m 4s`` or ``1d``")
                            .AsEphemeral(true));
                    return;
                }

                var minTime = (int) timeSpan.Value.TotalMilliseconds;

                if (minTime > settings.BonkMax)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("The minimum time cannot be greater than the maximum time!")
                            .AsEphemeral(true));
                    return;
                }
                
                if (minTime == settings.BonkMax) {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("The minimum time cannot be the same as the maximum time!")
                            .AsEphemeral(true));
                }

                await _settingsService.UpdateServerSetting(ctx.Guild.Id, Setting.BonkMin, minTime);
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent($"Set the minimum time before a bonk to {timeSpan.Value.Humanize(3)}")
                        .AsEphemeral(true));
            }
            
            [SlashCommand("max", "Set the maximum time before a stick might bonk")]
            public async Task SetBonkMax(InteractionContext ctx,
                [Option("time", "Maximum time before bonk (d, h, m, s)")] string time)
            {
                var settings = await _settingsService.GetSettingsOrReturnError(ctx);
                if (settings == null)
                    return;

                var timeSpan = TimeSpanConverter.Convert(time);
                if (!timeSpan.HasValue)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("Invalid time format, use ``xd xh xm xs``. For example ``1d 2h 3m 4s`` or ``1d``")
                            .AsEphemeral(true));
                    return;
                }

                var maxTime = (int) timeSpan.Value.TotalMilliseconds;

                if (maxTime > settings.BonkMax)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("The maximum time cannot be greater than the minimum time!")
                            .AsEphemeral(true));
                    return;
                }
                
                if (maxTime == settings.BonkMax) {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("The maximum time cannot be the same as the minimum time!")
                            .AsEphemeral(true));
                }

                await _settingsService.UpdateServerSetting(ctx.Guild.Id, Setting.BonkMax, maxTime);
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent($"Set the maximum time before a bonk to {timeSpan.Value.Humanize(3)}")
                        .AsEphemeral(true));
            }
        }
    }
}