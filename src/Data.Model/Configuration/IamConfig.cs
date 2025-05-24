using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyprship.Data.Model.Configuration;

public static class IamConfig
{
    public static void Configure(ModelBuilder builder, int maxKeyLength, PersonalDataConverter? personalDataConverter)
    {
        ConfigureUsersOnly(builder, maxKeyLength, personalDataConverter);
        builder.Entity<User>(b =>
        {
            b.HasMany<UserRole>()
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .HasConstraintName("fk_users_roles_users")
                .IsRequired();
        });
        ConfigureRole(builder.Entity<Role>());
        ConfigureRoleClaim(builder.Entity<RoleClaim>());
        ConfigureUserRole(builder.Entity<UserRole>());
    }

    public static void ConfigureUsersOnly(ModelBuilder builder, int maxKeyLength, PersonalDataConverter? personalDataConverter)
    {
        ConfigureUser(builder.Entity<User>(), personalDataConverter);
        ConfigureUserClaim(builder.Entity<UserClaim>());
        ConfigureUserLogin(builder.Entity<UserLogin>(), maxKeyLength);
        ConfigureUserToken(builder.Entity<UserToken>(), maxKeyLength, personalDataConverter);
    }

    public static void ConfigureUser(EntityTypeBuilder<User> builder, PersonalDataConverter? personalDataConverter = null)
    {
        var b = builder;
        b.ToTable("users", "iam");
        b.HasKey(u => u.Id).HasName("pk_users");
        b.HasIndex(u => u.NormalizedUserName)
            .HasDatabaseName("ix_users_username")
            .IsUnique();
        b.HasIndex(u => u.NormalizedEmail)
            .HasDatabaseName("ix_users_email")
            .IsUnique();

        b.Property(u => u.ConcurrencyStamp)
            .IsConcurrencyToken();

        b.Property(u => u.SyncId)
            .IsRequired();

        b.SetMaxLength(u => u.UserName, 64);
        b.SetMaxLength(u => u.NormalizedUserName, 64);
        b.SetMaxLength(u => u.Email, 256);
        b.SetMaxLength(u => u.NormalizedEmail, 256);
        b.SetMaxLength(u => u.PhoneNumber, 64);

        if (personalDataConverter != null)
        {
            var personalDataProps = typeof(User).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                if (p.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException("Personal data properties must be of type string.");
                }

                b.Property(typeof(string), p.Name).HasConversion(personalDataConverter);
            }
        }

        b.HasMany<UserClaim>()
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .HasConstraintName("fk_user_claims_users")
            .IsRequired();
        b.HasMany<UserLogin>()
            .WithOne()
            .HasForeignKey(ul => ul.UserId)
            .HasConstraintName("fk_user_logins_users")
            .IsRequired();

        b.HasMany<UserToken>()
            .WithOne()
            .HasForeignKey(ut => ut.UserId)
            .HasConstraintName("fk_user_tokens_users")
            .IsRequired();
    }

    public static void ConfigureUserClaim(EntityTypeBuilder<UserClaim> builder)
    {
        var b = builder;
        b.HasKey(uc => uc.Id).HasName("pk_user_claims");
        b.ToTable("user_claims", "iam");
    }

    public static void ConfigureUserLogin(EntityTypeBuilder<UserLogin> builder, int maxKeyLength = 128)
    {
        var b = builder;
        b.HasKey(l => new { l.LoginProvider, l.ProviderKey }).HasName("pk_user_logins");

        if (maxKeyLength > 0)
        {
            b.Property(l => l.LoginProvider).HasMaxLength(maxKeyLength);
            b.Property(l => l.ProviderKey).HasMaxLength(maxKeyLength);
        }

        b.ToTable("user_logins", "iam");
    }

    public static void ConfigureUserToken(EntityTypeBuilder<UserToken> builder, int maxKeyLength = 128, PersonalDataConverter? converter = null)
    {
        var b = builder;
        b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name }).HasName("pk_user_tokens");
        if (maxKeyLength > 0)
        {
            b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
            b.Property(t => t.Name).HasMaxLength(maxKeyLength);
        }

        if (converter != null)
        {
            var tokenProps = typeof(UserToken).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
            foreach (var p in tokenProps)
            {
                if (p.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException("Personal data properties must be of type string.");
                }

                b.Property(typeof(string), p.Name).HasConversion(converter);
            }
        }

        b.ToTable("user_tokens", "iam");
    }

    public static void ConfigureRole(EntityTypeBuilder<Role> builder)
    {
        var b = builder;
        b.ToTable("iam", "roles");
        b.HasKey(r => r.Id).HasName("pk_roles");
        b.HasIndex(r => r.NormalizedName)
            .HasDatabaseName("ix_roles_name")
            .IsUnique();

        b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
        b.Property(r => r.SyncId).IsRequired();
        b.HasIndex(r => r.SyncId)
            .HasDatabaseName("ix_roles_sync_id")
            .IsUnique();
        b.Property(u => u.Name).HasMaxLength(256);
        b.Property(u => u.NormalizedName).HasMaxLength(256);

        b.ToTable("roles", "iam");

        b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).HasConstraintName("fk_users_roles_roles").IsRequired();
        b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).HasConstraintName("fk_role_claims_roles").IsRequired();
    }

    public static void ConfigureRoleClaim(EntityTypeBuilder<RoleClaim> builder)
    {
        var b = builder;
        b.HasKey(rc => rc.Id).HasName("pk_role_claims");
        b.ToTable("role_claims", "iam");
    }

    public static void ConfigureUserRole(EntityTypeBuilder<UserRole> builder)
    {
        var b = builder;
        b.HasKey(r => new { r.UserId, r.RoleId }).HasName("pk_user_roles");
        b.ToTable("users_roles", "iam");
    }
}