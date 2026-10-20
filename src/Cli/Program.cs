// using Core.Dto;
// using Core.Import;

// // Шлях до файлу: з args[0] або за замовчуванням data/sample.csv
// string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

// // Перевірка існування файлу
// if (!File.Exists(path))
// {
//     Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
//     return 1;
// }

// // Імпорт
// ImportResult<ProductDto> result = ProductCsvImporter.Load(path);

// // Вивід кількості
// Console.WriteLine($"Завантажено записів: {result.Items.Count}");

// // Вивід перших 5 записів
// foreach (ProductDto p in result.Items.Take(5))
// {
//     Console.WriteLine($"  {p.Id,-6} {p.Sku,-10} {p.Name,-26} {p.Quantity,5} {p.Unit}");
// }

// // Вивід помилок
// if (result.Errors.Count > 0)
// {
//     Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
//     foreach (string e in result.Errors)
//     {
//         Console.WriteLine($"  ! {e}");
//     }
// }

// return 0;

using Core.Domain;

Console.WriteLine("=== Сценарій 1: успіх ===");
Product product = Product.Create("P-001", "sku-001", "Цемент M400 25кг", "шт", 100);
Console.WriteLine(product);

product.RegisterArrival(50);
product.Issue(30);
Console.WriteLine(product);

Console.WriteLine();
Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

TryDo("видача більша за залишок", () => product.Issue(1000));
TryDo("порожній SKU", () => Product.Create("P-002", " ", "Пісок", "т", 10));
TryDo("від'ємний залишок", () => Product.Create("P-003", "SKU-003", "Цегла", "шт", -5));

Console.WriteLine();
Console.WriteLine($"Поточний стан після всіх відмов: {product}");

// Локальна функція для демонстрації
static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        // Виводимо ТІЛЬКИ Message та тип, без StackTrace
        Console.WriteLine($"  {title}: {ex.GetType().Name} — {ex.Message}");
    }
}