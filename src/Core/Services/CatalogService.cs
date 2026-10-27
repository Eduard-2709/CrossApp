using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

/// <summary>
/// Сервіс бізнес-операцій.
/// НЕ знає, де зберігаються дані (файл чи пам'ять), лише викликає ICatalogStore.
/// </summary>
public sealed class CatalogService(ICatalogStore store)
{
    private readonly ICatalogStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public Product Add(string sku, string name, string unit, int quantity)
    {
        // Генеруємо короткий ID (перші 8 символів GUID)
        var product = Product.Create(Guid.NewGuid().ToString("N")[..8], sku, name, unit, quantity);
        _store.Add(product);
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        
        product.RegisterArrival(quantity); // Інваріант перевіряється в сутності!
        _store.Update(product);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        
        product.Issue(quantity); // Інваріант перевіряється в сутності!
        _store.Update(product);
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);
}