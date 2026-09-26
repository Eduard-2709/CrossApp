namespace Core.Dto;

/// <summary>
/// Результат імпорту: успішно прочитані дані + список помилок.
/// Узагальнений тип T — для різних DTO (ProductDto, BookDto тощо).
/// IReadOnlyList — щоб ніхто не міг змінити результат ззовні.
/// </summary>
public sealed record ImportResult<T>(
    IReadOnlyList<T> Items,
    IReadOnlyList<string> Errors
);