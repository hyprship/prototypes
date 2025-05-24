using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyprship.Data.Model.Configuration;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<TProperty> SetMaxLength<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        int maxLength = 256,
        bool isRequired = false)
        where TEntity : class
    {
        return builder.Property(propertyExpression)
            .HasMaxLength(maxLength)
            .IsRequired(isRequired);
    }
}