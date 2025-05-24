using Hyprship.Data.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hyprship.Data.MySql;

public class MssqlDbContext : AppDbContext
{
    public MssqlDbContext(DbContextOptions<MssqlDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.UseSqlServer("Server=localhost;Database=hyprship;User Id=sa;Password=yourStrong(!)Password;");
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
        optionsBuilder.UseSqlServer("Server=localhost;Database=hyprship;User Id=sa;Password=yourStrong(!)Password;");
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        return new MssqlDbContext(optionsBuilder.Options);
    }
}