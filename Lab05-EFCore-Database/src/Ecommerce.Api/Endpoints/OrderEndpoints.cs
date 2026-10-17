using Ecommerce.Application.Dtos;
using Ecommerce.Application.Services;

namespace Ecommerce.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", async (OrderService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapPost("/", async (
            CreateOrderRequest request,
            OrderService service,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var created = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/orders/{created.Id}", created);
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        return app;
    }
}
