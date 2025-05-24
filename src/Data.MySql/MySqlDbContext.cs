using Hyprship.Data.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hyprship.Data.MySql;

public class MySqlDbContext : AppDbContext
{
    public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
            var server = Environment.GetEnvironmentVariable("HYPRSHIP_DB_SERVER") ??
                Environment.GetEnvironmentVariable("MYSQL_HOST") ??
                "localhost";

            var database = Environment.GetEnvironmentVariable("HYPRSHIP_DB_NAME") ??
                Environment.GetEnvironmentVariable("MYSQL_DATABASE") ??
                "hyprship";

            var port = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PORT") ??
                Environment.GetEnvironmentVariable("MYSQL_PORT") ??
                "3306";

            var user = Environment.GetEnvironmentVariable("HYPRSHIP_DB_USER") ??
                Environment.GetEnvironmentVariable("MYSQL_USER") ??
                "hyprship_user";

            var password = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PASSWORD") ??
                Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ??
                "changeM3!";

            var cs = $"Server={server};Database={database};Port={port};User={user};Password={password};";
            if (Environment.GetEnvironmentVariable("HYPRSHIP_DB_SSL") == "true" ||
                Environment.GetEnvironmentVariable("MYSQL_SSL") == "true")
            {
                cs += "SslMode=Preferred;";
            }

            optionsBuilder.UseMySql(cs, ServerVersion.AutoDetect(cs))
                .UseSnakeCaseNamingConvention();
        }

        base.OnConfiguring(optionsBuilder);
    }
}

public class MySqlDbContextFactory : IDesignTimeDbContextFactory<MySqlDbContext>
{
    public MySqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MySqlDbContext>();
        var server = Environment.GetEnvironmentVariable("HYPRSHIP_DB_SERVER") ??
                Environment.GetEnvironmentVariable("MYSQL_HOST") ??
                "localhost";

        var database = Environment.GetEnvironmentVariable("HYPRSHIP_DB_NAME") ??
                Environment.GetEnvironmentVariable("MYSQL_DATABASE") ??
                "hyprship";

        var port = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PORT") ??
                Environment.GetEnvironmentVariable("MYSQL_PORT") ??
                "3306";

        var user = Environment.GetEnvironmentVariable("HYPRSHIP_DB_USER") ??
                Environment.GetEnvironmentVariable("MYSQL_USER") ??
                "hyprship_user";

        var password = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PASSWORD") ??
                Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ??
                "changeM3!";

        var cs = $"Server={server};Database={database};Port={port};User={user};Password={password};";
        if (Environment.GetEnvironmentVariable("HYPRSHIP_DB_SSL") == "true" ||
            Environment.GetEnvironmentVariable("MYSQL_SSL") == "true")
        {
            cs += "SslMode=Preferred;";
        }

        optionsBuilder.UseMySql(cs, ServerVersion.Create(new Version(11, 4, 0), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb))
            .UseSnakeCaseNamingConvention();

        return new MySqlDbContext(optionsBuilder.Options);
    }
}