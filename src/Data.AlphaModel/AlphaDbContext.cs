using Microsoft.EntityFrameworkCore;

namespace Hyprship.Data.AlphaModel;

public class AlphaDbContext : DbContext
{
    public AlphaDbContext(DbContextOptions<AlphaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        string? schema = "hyprship";
        if (this.Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true)
        {
            schema = null; // SQLite does not support schemas
        }

        ModelConfiguration.Configure(modelBuilder, schema);
    }
}