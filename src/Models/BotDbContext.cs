using Microsoft.EntityFrameworkCore;

namespace StickBot.Models;

public class BotDbContext : DbContext
{
    public DbSet<Stick> Sticks { get; set; }
    public DbSet<StealAttempt> StealAttempts { get; set; }
    public DbSet<Settings> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        #if DEBUG
        optionsBuilder.UseSqlite("Data Source=stickbot.db");
        #else
        // Some sort of PostgreSQL string
        #endif
    }
}