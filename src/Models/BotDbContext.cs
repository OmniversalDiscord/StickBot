using Microsoft.EntityFrameworkCore;

namespace StickBot.Models;

public class BotDbContext : DbContext
{
    public DbSet<Stick> Sticks { get; set; }
    public DbSet<Settings> Settings { get; set; }
}