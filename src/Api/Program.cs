using Api.Endpoints;
using Core.Abstractions;
using Core.Storage;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// >>> Lab10, Завдання 5: OpenAPI
builder.Services.AddOpenApi();

// >>> Lab10, Завдання 7: ProblemDetails для структурованих помилок
builder.Services.AddProblemDetails();

// >>> Lab10, Завдання 6: реєстрація сховища в DI (Singleton, потокобезпечне)
builder.Services.AddSingleton<ICatalogStore, InMemoryCatalogStore>();

var app = builder.Build();

// >>> Lab10, Завдання 7: виняток не показує клієнтові stack trace
app.UseExceptionHandler();
app.UseStatusCodePages();

// >>> Lab10, Завдання 5: OpenAPI/Scalar лише в Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                    // /openapi/v1.json
    app.MapScalarApiReference();         // /scalar/v1
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// >>> Lab10, Завдання 2: підключення маршрутів
app.MapProducts();

app.Run();