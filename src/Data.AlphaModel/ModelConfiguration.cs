using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyprship.Data.AlphaModel;

public static class ModelConfiguration
{
    public static void Configure(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder, string? schema = "hyprship")
    {
        // Configure your entity mappings here
        // Example: modelBuilder.Entity<YourEntity>().ToTable("YourTableName", schema);

        // Example for Tag entity
        ConfigureTags(modelBuilder.Entity<Tag>(), schema);
        ConfigureTagValues(modelBuilder.Entity<TagValue>(), schema);
    }

    public static void ConfigureTags(EntityTypeBuilder<Tag> builder, string? schema = "hyprship")
    {
        // Configure the Tag entity
        builder.HasKey(t => t.Id);
        builder.ToTable("tags", schema);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.SyncId).IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(t => t.Description)
            .HasMaxLength(512);

        builder.HasIndex(t => t.SyncId)
            .IsUnique()
            .HasDatabaseName("ix_tags_sync_id");

        builder.HasIndex(t => t.Slug)
            .IsUnique()
            .HasDatabaseName("ix_tags_slug");
    }

    public static void ConfigureTagValues(EntityTypeBuilder<TagValue> builder, string? schema = "hyprship")
    {
        // Configure the TagValue entity
        builder.HasKey(tv => tv.Id);
        builder.ToTable("tag_values", schema);
        builder.Property(tv => tv.Id).ValueGeneratedOnAdd();
        builder.Property(tv => tv.SyncId).IsRequired();

        builder.Property(tv => tv.Value)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(tv => tv.CreatedAt)
            .IsRequired();

        builder.HasOne(tv => tv.Tag)
            .WithMany(t => t.Values)
            .HasForeignKey(tv => tv.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tv => tv.SyncId)
            .IsUnique()
            .HasDatabaseName("ix_tag_values_sync_id");

        builder.HasIndex(tv => new { tv.TagId, tv.Value })
            .IsUnique()
            .HasDatabaseName("ix_tag_values_tag_id_value");
    }
}