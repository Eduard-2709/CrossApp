using System.Security.Cryptography;
using Api.Contracts;
using Core.Abstractions;
using Core.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProducts(this WebApplication app)
    {
        // >>> Lab10, Завдання 2: група маршрутів під /api/products
        var group = app.MapGroup("/api/products").WithTags("Products");

        // GET /api/products — список усіх товарів
        group.MapGet("/", (ICatalogStore store) =>
            TypedResults.Ok(store.List().Select(p => ToDto(p)).ToList()));

        // GET /api/products/{id} — один товар або 404
        group.MapGet("/{id}", Results<Ok<ProductDto>, NotFound>
            (string id, ICatalogStore store) =>
            store.GetById(id) is { } found
                ? TypedResults.Ok(ToDto(found))
                : TypedResults.NotFound());

        // POST /api/products — створення, 201 + Location
        group.MapPost("/", Results<Created<ProductDto>, ValidationProblem>
            (SaveProductRequest request, ICatalogStore store) =>
            {
                // >>> Lab10, Завдання 4: порушення валідації -> 400, не 500
                if (!request.TryValidate(out var errors))
                    return TypedResults.ValidationProblem(errors);

                var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
                var created = Product.Create(id, request.Sku, request.Name, request.Unit, request.Quantity);
                store.Add(created);

                // >>> Lab10, Завдання 2: 201 Created із Location
                return TypedResults.Created($"/api/products/{created.Id}", ToDto(created));
            });

        // PUT /api/products/{id} — повна заміна, 200 або 404 або 400
        group.MapPut("/{id}", Results<Ok<ProductDto>, NotFound, ValidationProblem>
            (string id, SaveProductRequest request, ICatalogStore store) =>
            {
                if (!request.TryValidate(out var errors))
                    return TypedResults.ValidationProblem(errors);

                if (store.GetById(id) is null)
                    return TypedResults.NotFound();

                // PUT замінює стан: створюємо валідний Product з тим самим id
                var replacement = Product.Create(id, request.Sku, request.Name, request.Unit, request.Quantity);
                store.Update(replacement);
                return TypedResults.Ok(ToDto(replacement));
            });

        // DELETE /api/products/{id} — 204 або 404
        group.MapDelete("/{id}", Results<NoContent, NotFound>
            (string id, ICatalogStore store) =>
            store.Remove(id)
                ? TypedResults.NoContent()
                : TypedResults.NotFound());
    }

    // Мапінг домен -> DTO (щоб не тягнути залежність Api -> Core.Dto, якщо її немає)
    private static ProductDto ToDto(Core.Domain.Product p) =>
        new(p.Id, p.Sku, p.Name, p.Unit, p.Quantity);
}