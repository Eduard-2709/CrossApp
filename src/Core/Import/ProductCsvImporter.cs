using Core.Dto;

namespace Core.Import;

/// <summary>
/// Імпортер товарів з CSV-файлу.
/// Роздільник — крапка з комою (не конфліктує з комою в назвах).
/// </summary>
public static class ProductCsvImporter
{
    // Роздільник — крапка з комою
    private const char Separator = ';';

    /// <summary>
    /// Завантажує товари з файлу.
    /// Повертає ImportResult з даними та помилками.
    /// НЕ кидає виняток на першій помилці — продовжує обробку.
    /// </summary>
    public static ImportResult<ProductDto> Load(string path)
    {
        var items = new List<ProductDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            // Пропускаємо порожні рядки та коментарі
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            // Пропускаємо рядок заголовків
            if (number == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                continue;

            // Розбір рядка через switch expression
            switch (ParseLine(line))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {number}: {failed.Reason}");
                    break;
            }
        }

        return new ImportResult<ProductDto>(items, errors);
    }

    /// <summary>
    /// Розбирає один рядок CSV.
    /// Використовує switch expression з патернами:
    /// - патерн властивостей { Length: < 5 }
    /// - патерни списків [_, "", _, _, _]
    /// - логічні патерни or
    /// - охоронна умова when
    /// </summary>
    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            // Патерн властивостей + реляційний патерн: мало колонок
            { Length: < 5 } =>
                new ParseFailed($"очікую 5 колонок, отримав {parts.Length}"),

            // Патерни списків + константний патерн + логічний or: порожній SKU або Name
            [_, "", _, _, _] or [_, _, "", _, _] =>
                new ParseFailed("SKU або назва порожні"),

            // Охоронна умова when + out-параметр: нечислова кількість
            [_, _, _, _, var qty] when !int.TryParse(qty, out int q) || q < 0 =>
                new ParseFailed($"кількість '{qty}' не є невід'ємним числом"),

            // Успішний розбір: 5 колонок
            [var id, var sku, var name, var unit, var qty] =>
                new ParseOk(new ProductDto(id, sku, name, unit, int.Parse(qty))),

            // Занадто багато колонок
            _ =>
                new ParseFailed($"занадто багато колонок: {parts.Length}")
        };
    }

    // Ієрархія record для результату розбору
    private abstract record ParseOutcome;
    private sealed record ParseOk(ProductDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}