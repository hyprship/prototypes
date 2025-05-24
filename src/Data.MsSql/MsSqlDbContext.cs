using Hyprship.Data.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hyprship.Data.MsSql;

public class MssqlDbContext : AppDbContext
{
    public MssqlDbContext(DbContextOptions<MssqlDbContext> options)
        : base(options)
    {
    }

    public static string GetConnectionString()
    {
        var cs = Environment.GetEnvironmentVariable("HYPRSHIP_DB_CONNECTION_STRING") ??
                 Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING");

        if (!string.IsNullOrEmpty(cs))
        {
            return cs;
        }

        var server = Environment.GetEnvironmentVariable("HYPRSHIP_DB_HOST") ??
                     Environment.GetEnvironmentVariable("MSSQL_HOST") ??
                     "localhost";

        var database = Environment.GetEnvironmentVariable("HYPRSHIP_DB_NAME") ??
                       Environment.GetEnvironmentVariable("MSSQL_DATABASE") ??
                       "hyprship";

        var user = Environment.GetEnvironmentVariable("HYPRSHIP_DB_USER") ??
                   Environment.GetEnvironmentVariable("MSSQL_USER") ??
                   "sa";

        var password = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PASSWORD") ??
                       Environment.GetEnvironmentVariable("MSSQL_PASSWORD") ??
                       "yourStrong(!)Password";

        return $"Server={server};Database={database};User Id={user};Password={password};";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.UseSqlServer(GetConnectionString());
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        base.OnConfiguring(optionsBuilder);
    }
}

public class MssqlDbContextFactory : IDesignTimeDbContextFactory<MssqlDbContext>
{
    public MssqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MssqlDbContext>();
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseSqlServer(MssqlDbContext.GetConnectionString());
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        return new MssqlDbContext(optionsBuilder.Options);
    }
}