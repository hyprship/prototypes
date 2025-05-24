using Hyprship.Data.Model;
using Hyprship.Data.MsSql;
using Hyprship.Data.MySql;
using Hyprship.Data.PgSql;
using Hyprship.Data.Sqlite;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var provider = Environment.GetEnvironmentVariable("HYPRSHIP_DB_PROVIDER") ??
               Environment.GetEnvironmentVariable("DB_PROVIDER") ??
               "sqlite";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSnakeCaseNamingConvention();
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    switch (provider.ToLowerInvariant())
    {
        case "mysql":
            options.UseMySql(MySqlDbContext.GetConnectionString(), ServerVersion.AutoDetect(MySqlDbContext.GetConnectionString()));
            break;
        case "pgsql":
            options.UseNpgsql(PgSqlDbContext.GetConnectionString());
            break;
        case "mssql":
            options.UseSqlServer(MssqlDbContext.GetConnectionString());
            break;
        case "sqlite":
            options.UseSqlite(SqliteDbContext.GetConnectionString());
            break;
        default:
            throw new InvalidOperationException($"Unsupported database provider: {provider}");
    }
});

builder.Services
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddUserManager<UserManager<User>>()
    .AddRoles<Role>()
    .AddRoleManager<RoleManager<Role>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

var sp = builder.Services.BuildServiceProvider();
var db = sp.GetRequiredService<AppDbContext>().Database;
await db.EnsureCreatedAsync();
await db.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

await app.RunAsync();