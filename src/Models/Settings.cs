using System.ComponentModel.DataAnnotations;

namespace StickBot.Models;

public class Settings
{
    public int SettingsId { get; set; }

    [Key]
    public ulong Server { get; set; }
    public long BonkMin { get; set; }
    public long BonkMax { get; set; }
    public float StealSuccessChance { get; set; }
    public float StealBonkChance { get; set; }
    public ulong StickRole { get; set; }
    public ulong BonkedRole { get; set; }
}