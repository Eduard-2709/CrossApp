using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

/// <summary>
/// Сховище в оперативній пам'яті.
/// Дані зникають після завершення роботи програми.
/// </summary>
public sealed class InMemoryCatalogStore : ICatalogStore
{
    private readonly Dictionary<string, Product> _items;

    public InMemoryCatalogStore(IEnumerable<Product>? seed = null)
    {
        // Ініціалізуємо словник. Якщо seed == null, використовуємо порожню колекцію.
        _items = (seed ?? []).ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<Product> List() => _items.Values.ToList();

    public Product? GetById(string id) => _items.GetValueOrDefault(id);

    public void Add(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_items.ContainsKey(item.Id))
            throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");
        
        _items.Add(item.Id, item);
    }

    public void Update(Product item) => _items[item.Id] = item;

    public bool Remove(string id) => _items.Remove(id);
}