using Ecommerce.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name).IsRequired().HasMaxLength(100);
        builder.Property(category => category.Slug).IsRequired().HasMaxLength(100);
        builder.HasIndex(category => category.Slug).IsUnique();

        builder.HasData(
            new Category { Id = 1, Name = "Electronics", Slug = "electronics" },
            new Category { Id = 2, Name = "Books", Slug = "books" },
            new Category { Id = 3, Name = "Apparel", Slug = "apparel" });
    }
}
