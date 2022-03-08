using System.ComponentModel.DataAnnotations;

namespace StickBot.Models;

public class StealAttempt
{
    public int StealAttemptId { get; set; }
    public int ServerId { get; set; }
    public int UserId { get; set; }
    public DateTime Time { get; set; }
}