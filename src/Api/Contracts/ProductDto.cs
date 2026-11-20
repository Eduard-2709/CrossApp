namespace Api.Contracts;

// >>> Lab10, Завдання 3: вихідний DTO, доменну сутність у JSON не віддаємо
public sealed record ProductDto(
    string Id,
    string Sku,
    string Name,
    string Unit,
    int Quantity);