// using Core.Abstractions;
// using Core.Domain;

// namespace Core.Services;

// /// <summary>
// /// Сервіс бізнес-операцій.
// /// НЕ знає, де зберігаються дані (файл чи пам'ять), лише викликає ICatalogStore.
// /// </summary>
// public sealed class CatalogService(ICatalogStore store)
// {
//     private readonly ICatalogStore _store = store ?? throw new ArgumentNullException(nameof(store));

//     public Product Add(string sku, string name, string unit, int quantity)
//     {
//         // Генеруємо короткий ID (перші 8 символів GUID)
//         var product = Product.Create(Guid.NewGuid().ToString("N")[..8], sku, name, unit, quantity);
//         _store.Add(product);
//         return product;
//     }

//     public void Receive(string id, int quantity)
//     {
//         var product = _store.GetById(id) 
//             ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        
//         product.RegisterArrival(quantity); // Інваріант перевіряється в сутності!
//         _store.Update(product);
//     }

//     public void Issue(string id, int quantity)
//     {
//         var product = _store.GetById(id) 
//             ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        
//         product.Issue(quantity); // Інваріант перевіряється в сутності!
//         _store.Update(product);
//     }

//     public IReadOnlyList<Product> All() => _store.List();

//     public Product? Find(string id) => _store.GetById(id);
// }





using Core.Abstractions;
using Core.Domain;
using Core.Events; // <-- Додано

namespace Core.Services;

public sealed class CatalogService(ICatalogStore store)
{
    private readonly ICatalogStore _store = store ?? throw new ArgumentNullException(nameof(store));

    // Оголошення події. event EventHandler<...> — стандарт .NET.
    public event EventHandler<CatalogChangedEventArgs>? Changed;

    // Приватний метод для підняття події. Викликається ТІЛЬКИ після успішної зміни!
    private void OnChanged(Product p, CatalogChangeKind kind, int delta)
    {
        // ?.Invoke — безпечний виклик (якщо підписників немає, нічого не станеться)
        Changed?.Invoke(this, new CatalogChangedEventArgs(p.Id, kind, delta, p.Quantity));
    }

    public Product Add(string sku, string name, string unit, int quantity)
    {
        var product = Product.Create(Guid.NewGuid().ToString("N")[..8], sku, name, unit, quantity);
        _store.Add(product);
        
        // Зміна успішна — піднімаємо подію
        OnChanged(product, CatalogChangeKind.Added, quantity); 
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        
        product.RegisterArrival(quantity); // Якщо інваріант впаде, полетить виняток, і код нижче не виконається!
        
        _store.Update(product);
        
        // Зміна успішна — піднімаємо подію
        OnChanged(product, CatalogChangeKind.Received, quantity);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        
        product.Issue(quantity); // Перевірка інваріанту
        
        _store.Update(product);
        
        OnChanged(product, CatalogChangeKind.Issued, -quantity); // Delta від'ємна
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);

    // Використання делегата Func (предикат для пошуку)
    public IReadOnlyList<Product> FindAll(Func<Product, bool> predicate)
    {
        return _store.List().Where(predicate).ToList();
    }
}