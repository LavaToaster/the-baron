using Microsoft.EntityFrameworkCore;
using TheBaron.Entities;

namespace TheBaron.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        DbPath = Path.Join(Directory.GetCurrentDirectory(), "app.db");
    }

    public DbSet<User> Users { get; set; } = null!;

    public string DbPath { get; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}