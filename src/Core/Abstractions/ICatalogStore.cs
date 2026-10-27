using Core.Domain;

namespace Core.Abstractions;

/// <summary>
/// Контракт сховища товарів.
/// Сервіс знає лише про цей інтерфейс, а не про конкретну реалізацію (файл чи пам'ять).
/// </summary>
public interface ICatalogStore
{
    // Повертає список лише для читання, щоб ніхто не зміг змінити колекцію ззовні
    IReadOnlyList<Product> List();

    // Повертає Product? (nullable), бо запису може не бути
    Product? GetById(string id);

    void Add(Product item);
    void Update(Product item);
    bool Remove(string id);
}