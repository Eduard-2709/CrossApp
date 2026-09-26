namespace Core.Dto;

/// <summary>
/// DTO для товару з CSV-файлу.
/// Record обрано тому, що це незмінні дані з файлу.
/// Id, Sku, Name, Unit — обов'язкові (не можуть бути null).
/// Note — необов'язкове поле (може бути відсутнім).
/// </summary>
public record ProductDto(
    string Id,
    string Sku,
    string Name,
    string Unit,
    int Quantity,
    string? Note = null
);