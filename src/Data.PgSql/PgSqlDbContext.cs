using Hyprship.Data.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hyprship.Data.PgSql;

public class PgSqlDbContext : AppDbContext
{
    public PgSqlDbContext(DbContextOptions<PgSqlDbContext> options)
        : base(options)
    {
    }

    public static string GetConnectionString()
    {
        var cs = Environment.GetEnvironmentVariable("HYPRSHIP_DB_CONNECTION_STRING") ??
                 Environment.GetEnvironmentVariable("PGSQL_CONNECTION_STRING");

        if (!string.IsNullOrEmpty(cs))
        {
            return cs;
        }

        var server = Environment.GetEnvironmentVariable("HYPRSHIP_DB_HOST") ??
                     Environment.GetEnvironmentVariable("POSTGRES_HOST") ??
                     "localhost";

        var port = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PORT") ??
                        Environment.GetEnvironmentVariable("POSTGRES_PORT") ??
                        "5432";

        var user = Environment.GetEnvironmentVariable("HYPRSHIP_DB_USER") ??
                        Environment.GetEnvironmentVariable("POSTGRES_USER") ??
                        "hyprship";

        var password = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PASSWORD") ??
                        Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ??
                        "hyprship";

        var db = Environment.GetEnvironmentVariable("HYPRSHIP_DB_NAME") ??
                 Environment.GetEnvironmentVariable("POSTGRES_DATABASE") ??
                 "hyprship";

        return $"Host={server};Port={port};Database={db};Username={user};Password={password}";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.UseNpgsql(GetConnectionString());
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
        optionsBuilder.UseNpgsql(PgSqlDbContext.GetConnectionString());

        return new PgSqlDbContext(optionsBuilder.Options);
    }
}