using Microsoft.EntityFrameworkCore;
using StickBot.Models;

namespace StickBot.Services;

public class SettingsService
{
    private readonly BotDbContext _dbContext;
    
    public SettingsService(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Settings> GetServerSettings(ulong serverId)
    {
        var settings = await _dbContext.Settings.Where(x => x.ServerId == serverId).FirstOrDefaultAsync();

        if (settings == null) { throw new ServerNotFoundException(); }
        
        return settings;
    }

    public async Task CreateDefaultSettings(ulong serverId, ulong stickRole)
    {
        var settings = new Settings
        {
            ServerId = serverId,
            BonkMin = 86400000, // One day
            BonkMax = 259200000, // Three days
            StealSuccessChance = 0.05,
            StealBonkChance = 0.25,
            StealCooldown = 3600000, // One hour
            StickRole = stickRole,
            BonkedRole = 0 // Must be set by admin
        };
        
        await _dbContext.Settings.AddAsync(settings);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteSettings(ulong serverId)
    {
        var settings = await GetServerSettings(serverId);

        _dbContext.Settings.Remove(settings);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateServerSetting(ulong serverId, Setting setting, dynamic value)
    {
        var settings = await GetServerSettings(serverId);
    
        // Is this really the best way????
        switch (setting)
        {
            case Setting.BonkMin: settings.BonkMin = value; break;
            case Setting.BonkMax: settings.BonkMax = value; break;
            case Setting.StealSuccessChance: settings.StealSuccessChance = value; break;
            case Setting.StealBonkChance: settings.StealBonkChance = value; break;
            case Setting.StealCooldown: settings.StealCooldown = value; break;
            case Setting.StickRole: settings.StickRole = value; break;
            case Setting.BonkedRole: settings.BonkedRole = value; break;
            default: throw new ArgumentException("Setting not found");
        }
        
        await _dbContext.SaveChangesAsync();
    }
}