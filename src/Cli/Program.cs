// using Core.Domain;

// Console.WriteLine("=== Сценарій 1: успіх ===");
// Product product = Product.Create("P-001", "sku-001", "Цемент M400 25кг", "шт", 100);
// Console.WriteLine(product);

// product.RegisterArrival(50);
// product.Issue(30);
// Console.WriteLine(product);

// Console.WriteLine();
// Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

// TryDo("видача більша за залишок", () => product.Issue(1000));
// TryDo("порожній SKU", () => Product.Create("P-002", " ", "Пісок", "т", 10));
// TryDo("від'ємний залишок", () => Product.Create("P-003", "SKU-003", "Цегла", "шт", -5));

// Console.WriteLine();
// Console.WriteLine($"Поточний стан після всіх відмов: {product}");

// // Локальна функція для демонстрації
// static void TryDo(string title, Action action)
// {
//     try
//     {
//         action();
//         Console.WriteLine($"  {title}: виняток НЕ спрацював — інваріант відсутній!");
//     }
//     catch (Exception ex)
//     {
//         // Виводимо ТІЛЬКИ Message та тип, без StackTrace
//         Console.WriteLine($"  {title}: {ex.GetType().Name} — {ex.Message}");
//     }
// }





using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

// 1. Визначаємо, яке сховище використовувати (Ручний DI / Composition Root)
bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

// 2. Виводимо, яке сховище використовується
Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");
Console.WriteLine();

// 3. Успішний сценарій
Console.WriteLine("-- Спроба додати новий товар --");
var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
Console.WriteLine($"Створено: {created}");
service.Receive(created.Id, 25);
Console.WriteLine($"Після приходу: {created}");
Console.WriteLine();

// 4. Вивід усього списку
Console.WriteLine("-- Список товарів --");
foreach (var p in service.All())
{
    Console.WriteLine($"  {p.Id,-8} {p.Sku,-10} {p.Name,-26} {p.Quantity,5} {p.Unit}");
}
Console.WriteLine();

// 5. Сценарій відмови (помилка)
Console.WriteLine("-- Сценарій відмови --");
try
{
    service.Add("SKU-102", "Дублікат", "шт", 10); // Додаємо
    service.Add("SKU-103", "Дублікат", "шт", 10); // Намагаємось створити з тим самим ID? Ні, ID генерується випадково.
    // Для демонстрації помилки дубліката викличемо Receive з неіснуючим ID
    service.Receive("nonexistent-id", 10);
}
catch (Exception ex)
{
    Console.WriteLine($"  Помилка: {ex.Message}");
}