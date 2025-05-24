using Hyprship.Data.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hyprship.Data.Sqlite;

public class SqliteDbContext : AppDbContext
{
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options)
        : base(options)
    {
    }

    public static string GetConnectionString()
    {
        var cs = Environment.GetEnvironmentVariable("HYPRSHIP_DB_CONNECTION_STRING") ??
                 Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");

        if (!string.IsNullOrEmpty(cs))
        {
            return cs;
        }

        var db = Environment.GetEnvironmentVariable("HYPRSHIP_DB_NAME") ??
                 Environment.GetEnvironmentVariable("SQLITE_DATABASE") ??
                 "hyprship.db";

        return $"Data Source={db};Cache=Shared";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.UseSqlite(GetConnectionString());
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        base.OnConfiguring(optionsBuilder);
    }
}

public class SqliteDbContextFactory : IDesignTimeDbContextFactory<SqliteDbContext>
{
    public SqliteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseSqlite(SqliteDbContext.GetConnectionString());
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        return new SqliteDbContext(optionsBuilder.Options);
    }
}