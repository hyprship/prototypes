using Hyprship.Data.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hyprship.Data.MySql;

public class PgSqlDbContext : AppDbContext
{
    public PgSqlDbContext(DbContextOptions<PgSqlDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.UseNpgsql("Host=localhost;Database=hyprship;Username=hyprship;Password=hyprship");
        }

        base.OnConfiguring(optionsBuilder);
    }
}

public class PgSqlDbContextFactory : IDesignTimeDbContextFactory<PgSqlDbContext>
{
    public PgSqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PgSqlDbContext>();
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.UseNpgsql("Host=localhost;Database=hyprship;Username=hyprship;Password=hyprship");

        return new PgSqlDbContext(optionsBuilder.Options);
    }
}