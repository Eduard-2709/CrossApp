// using Core;
// using Core.Abstractions;
// using Core.Services;
// using Core.Storage;

// // 1. Визначаємо, яке сховище використовувати (Ручний DI / Composition Root)
// bool useFile = args.Contains("--file");
// string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

// ICatalogStore store = useFile
//     ? new FileCatalogStore(dataPath)
//     : new InMemoryCatalogStore(SampleData.Products());

// var service = new CatalogService(store);

// // 2. Виводимо, яке сховище використовується
// Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");
// Console.WriteLine();

// // 3. Успішний сценарій
// Console.WriteLine("-- Спроба додати новий товар --");
// var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
// Console.WriteLine($"Створено: {created}");
// service.Receive(created.Id, 25);
// Console.WriteLine($"Після приходу: {created}");
// Console.WriteLine();

// // 4. Вивід усього списку
// Console.WriteLine("-- Список товарів --");
// foreach (var p in service.All())
// {
//     Console.WriteLine($"  {p.Id,-8} {p.Sku,-10} {p.Name,-26} {p.Quantity,5} {p.Unit}");
// }
// Console.WriteLine();

// // 5. Сценарій відмови (помилка)
// Console.WriteLine("-- Сценарій відмови --");
// try
// {
//     service.Add("SKU-102", "Дублікат", "шт", 10); // Додаємо
//     service.Add("SKU-103", "Дублікат", "шт", 10); // Намагаємось створити з тим самим ID? Ні, ID генерується випадково.
//     // Для демонстрації помилки дубліката викличемо Receive з неіснуючим ID
//     service.Receive("nonexistent-id", 10);
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"  Помилка: {ex.Message}");
// }





using Core;
using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;

// 1. Composition Root
bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);
Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");

// 2. Налаштування логування
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!); // Створюємо папку, якщо її немає

// 3. Перший підписник (Локальний метод)
void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
{
    // Форматування: +0;-0 додає плюс для додатних чисел
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
    
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

// Підписка першим підписником
service.Changed += OnCatalogChanged;

// 4. Другий підписник (Лямбда-вираз із замиканням)
int changes = 0;
service.Changed += (_, _) => changes++;


// ==========================================
// СЦЕНАРІЙ: Успішні операції
// ==========================================
Console.WriteLine("\n-- Успішні операції --");
var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
service.Receive(created.Id, 25);
service.Issue(created.Id, 10); // Додали видачу для повноти

Console.WriteLine($"\n[Підсумок] Подій отримано лічильником: {changes}");


// ==========================================
// СЦЕНАРІЙ: Відписка (Операція без логування)
// ==========================================
Console.WriteLine("\n-- Відписка першого обробника --");
service.Changed -= OnCatalogChanged; // Відписуємо метод, що писав у консоль та файл

service.Receive(created.Id, 5); // "Тиха" операція

Console.WriteLine($"\n[Підсумок] Подій отримано лічильником: {changes}"); // Лічильник все ще працює (+1)


// ==========================================
// СЦЕНАРІЙ: Відмова (Інваріант порушено)
// ==========================================
Console.WriteLine("\n-- Сценарій відмови (Інваріант) --");
try
{
    // Намагаємось видалити 1000 одиниць, коли їх лише 70
    service.Issue(created.Id, 1000); 
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка] {ex.Message}");
}


// ==========================================
// СЦЕНАРІЙ: Використання Func
// ==========================================
Console.WriteLine("\n-- Пошук через Func (FindAll) --");
// Шукаємо всі товари, де залишок менше 100
var lowStock = service.FindAll(p => p.Quantity < 100);
foreach (var p in lowStock)
{
    Console.WriteLine($"  {p.Sku} - {p.Quantity} {p.Unit}");
}