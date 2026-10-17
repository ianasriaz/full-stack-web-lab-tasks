using Ecommerce.Application.Dtos;
using Ecommerce.Application.Services;

namespace Ecommerce.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (ProductService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:int}", async (int id, ProductService service, CancellationToken cancellationToken) =>
        {
            var product = await service.GetByIdAsync(id, cancellationToken);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapPost("/", async (
            CreateProductRequest request,
            ProductService service,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var created = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/products/{created.Id}", created);
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        group.MapPut("/{id:int}", async (
            int id,
            UpdateProductRequest request,
            ProductService service,
            CancellationToken cancellationToken) =>
        {
            try
            {
                return await service.UpdateAsync(id, request, cancellationToken)
                    ? Results.NoContent()
                    : Results.NotFound();
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        group.MapDelete("/{id:int}", async (int id, ProductService service, CancellationToken cancellationToken) =>
            await service.DeleteAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}
