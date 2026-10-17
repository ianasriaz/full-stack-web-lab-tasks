using Ecommerce.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name).IsRequired().HasMaxLength(150);
        builder.Property(product => product.Description).HasMaxLength(1000);
        builder.Property(product => product.Price).HasPrecision(18, 2);

        builder
            .HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop Pro 14",
                Description = "14-inch developer laptop with 16 GB RAM.",
                Price = 1299.99m,
                Stock = 12,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Ergonomic 2.4 GHz wireless mouse.",
                Price = 25.50m,
                Stock = 100,
                CategoryId = 1
            },
            new Product
            {
                Id = 3,
                Name = "Mechanical Keyboard",
                Description = "Compact mechanical keyboard with hot-swap switches.",
                Price = 89.99m,
                Stock = 40,
                CategoryId = 1
            },
            new Product
            {
                Id = 4,
                Name = "Clean Code",
                Description = "A handbook of agile software craftsmanship.",
                Price = 32.00m,
                Stock = 25,
                CategoryId = 2
            },
            new Product
            {
                Id = 5,
                Name = "The Pragmatic Programmer",
                Description = "Your journey to mastery, 20th anniversary edition.",
                Price = 39.95m,
                Stock = 18,
                CategoryId = 2
            },
            new Product
            {
                Id = 6,
                Name = "Cotton T-Shirt",
                Description = "Plain cotton t-shirt available in several sizes.",
                Price = 15.75m,
                Stock = 200,
                CategoryId = 3
            });
    }
}
