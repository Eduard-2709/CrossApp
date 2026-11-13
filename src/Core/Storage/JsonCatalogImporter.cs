using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class JsonCatalogImporter : ICatalogImporter
{
    public async Task<IReadOnlyList<Product>> LoadAsync(string path, CancellationToken ct = default)
    {
        // Перевіряємо скасування ДО відкриття файлу (виправляє помилку в тесті)
        ct.ThrowIfCancellationRequested();

        // Використовуємо FileStream з useAsync: true для справжнього асинхронного читання
        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        var dtos = await JsonSerializer.DeserializeAsync<List<ProductDto>>(
            stream, cancellationToken: ct) 
            ?? throw new DomainException($"Файл {Path.GetFileName(path)} не містить списку.");

        var result = new List<Product>(dtos.Count);

        for (var i = 0; i < dtos.Count; i++)
        {
            // Кооперативне скасування: перевіряємо токен у циклі
            ct.ThrowIfCancellationRequested();

            try
            {
                result.Add(Product.FromDto(dtos[i]));
            }
            catch (ArgumentException ex)
            {
                // Технічну помилку перетворюємо на доменну з номером запису
                throw new DomainException($"Запис {i + 1}: {ex.Message}", ex);
            }
        }

        return result;
    }
}