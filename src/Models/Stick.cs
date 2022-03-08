using System.ComponentModel.DataAnnotations;

namespace StickBot.Models;

public class Stick
{
    public int StickId { get; set; }
    public ulong ServerId { get; set; }
    public DateTime BonkDate { get; set; }
    public ulong HoldingUserId { get; set; }
}