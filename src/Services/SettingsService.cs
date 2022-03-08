using Microsoft.EntityFrameworkCore;
using StickBot.Models;

namespace StickBot.Services;

public class SettingsService
{
    private BotDbContext _dbContext;
    
    public SettingsService(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Settings> GetServerSettings(ulong serverId)
    {
        var settings = await _dbContext.Settings.Where(x => x.ServerId == serverId).FirstOrDefaultAsync();
        
        if (settings == null) { throw new ArgumentException("Server not found"); }
        
        return settings;
    }

    public async Task UpdateServerSetting(ulong serverId, Setting setting, dynamic value)
    {
        var settings = await _dbContext.Settings.Where(x => x.ServerId == serverId).FirstOrDefaultAsync();
        
        if (settings == null) { throw new ArgumentException("Server not found"); }
    
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