namespace StickBot.Models;

public enum Setting
{
    BonkMin,
    BonkMax,
    StealSuccessChance,
    StealBonkChance,
    StealCooldown,
    StickRole,
    BonkedRole
}

public class Settings
{
    public int SettingsId { get; set; }
    
    public ulong ServerId { get; set; }
    public long BonkMin { get; set; }
    public long BonkMax { get; set; }
    public double StealSuccessChance { get; set; }
    public double StealBonkChance { get; set; }
    public long StealCooldown { get; set; }
    public ulong StickRole { get; set; }
    public ulong BonkedRole { get; set; }
}