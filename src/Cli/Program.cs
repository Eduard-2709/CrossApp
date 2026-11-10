// using Core;
// using Core.Abstractions;
// using Core.Events;
// using Core.Services;
// using Core.Storage;

// // 1. Composition Root
// bool useFile = args.Contains("--file");
// string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

// ICatalogStore store = useFile
//     ? new FileCatalogStore(dataPath)
//     : new InMemoryCatalogStore(SampleData.Products());

// var service = new CatalogService(store);
// Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");

// // 2. Налаштування логування
// var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
// Directory.CreateDirectory(Path.GetDirectoryName(logPath)!); // Створюємо папку, якщо її немає

// // 3. Перший підписник (Локальний метод)
// void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
// {
//     // Форматування: +0;-0 додає плюс для додатних чисел
//     var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
    
//     Console.WriteLine(line);
//     File.AppendAllText(logPath, line + Environment.NewLine);
// }

// // Підписка першим підписником
// service.Changed += OnCatalogChanged;

// // 4. Другий підписник (Лямбда-вираз із замиканням)
// int changes = 0;
// service.Changed += (_, _) => changes++;


// // ==========================================
// // СЦЕНАРІЙ: Успішні операції
// // ==========================================
// Console.WriteLine("\n-- Успішні операції --");
// var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
// service.Receive(created.Id, 25);
// service.Issue(created.Id, 10); // Додали видачу для повноти

// Console.WriteLine($"\n[Підсумок] Подій отримано лічильником: {changes}");


// // ==========================================
// // СЦЕНАРІЙ: Відписка (Операція без логування)
// // ==========================================
// Console.WriteLine("\n-- Відписка першого обробника --");
// service.Changed -= OnCatalogChanged; // Відписуємо метод, що писав у консоль та файл

// service.Receive(created.Id, 5); // "Тиха" операція

// Console.WriteLine($"\n[Підсумок] Подій отримано лічильником: {changes}"); // Лічильник все ще працює (+1)


// // ==========================================
// // СЦЕНАРІЙ: Відмова (Інваріант порушено)
// // ==========================================
// Console.WriteLine("\n-- Сценарій відмови (Інваріант) --");
// try
// {
//     // Намагаємось видалити 1000 одиниць, коли їх лише 70
//     service.Issue(created.Id, 1000); 
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"[Помилка] {ex.Message}");
// }


// // ==========================================
// // СЦЕНАРІЙ: Використання Func
// // ==========================================
// Console.WriteLine("\n-- Пошук через Func (FindAll) --");
// // Шукаємо всі товари, де залишок менше 100
// var lowStock = service.FindAll(p => p.Quantity < 100);
// foreach (var p in lowStock)
// {
//     Console.WriteLine($"  {p.Sku} - {p.Quantity} {p.Unit}");
// }





using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;

// 1. Composition Root
bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);
var items = service.All();

// 2. Визначаємо команду (ігноруємо прапорці типу --file)
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

// 3. Демонстрація відкладеного виконання (Deferred Execution)
if (command == "demo-deferred")
{
    Console.WriteLine("=== Демонстрація відкладеного виконання ===");
    // Беремо локальний список, щоб мати можливість його змінювати
    var source = service.All().ToList(); 
    
    // Запит побудовано, але ВІН ЩЕ НЕ ВИКОНАВСЯ
    var query = source.Where(p => p.Quantity < 10); 
    
    Console.WriteLine($"Дефіцитних до змін: {query.Count()}");
    
    // Змінюємо джерело (додаємо новий товар у список)
    source.Add(Product.Create("tmp00001", "SKU-999", "Скоба", "шт", 3));
    
    Console.WriteLine($"Дефіцитних після змін: {query.Count()}"); // Число змінилося!
    
    // Матеріалізуємо результат (фіксуємо знімок)
    var snapshot = query.ToList(); 
    source.Clear(); // Очищаємо джерело
    
    Console.WriteLine($"У знімку лишилось: {snapshot.Count}"); // Знімок не змінився
    return;
}

// 4. Виконання звітів
switch (command)
{
    case "report-top":
        Console.WriteLine("=== Топ-5 за залишком ===");
        Print(ReportService.TopByQuantity(items, 5));
        break;

    case "report-low":
        Console.WriteLine("=== Дефіцит (менше 10) ===");
        Print(ReportService.LowStock(items, 10));
        break;

    case "report-groups":
        Console.WriteLine("=== Групування за одиницями ===");
        foreach (var g in ReportService.ByUnit(items))
        {
            Console.WriteLine($"  {g.Key,-6} позицій: {g.Count,3} сумарно: {g.TotalQuantity,6}");
        }
        break;

    case "report-sum":
        Console.WriteLine("=== Загальна статистика ===");
        var t = ReportService.Totals(items);
        Console.WriteLine($"  Позицій: {t.Items}, Всього: {t.Total}, Середнє: {t.Average:F2}, Макс: {t.Max}");
        break;

    case "report-join":
        Console.WriteLine("=== Демонстрація Join (товари + переміщення) ===");
        // Штучні переміщення для демонстрації
        var movements = new List<MovementDto>
        {
            new("SKU-001", 50, DateTime.Now.AddDays(-1)),
            new("SKU-003", 200, DateTime.Now.AddDays(-2)),
            new("SKU-005", 10, DateTime.Now.AddDays(-3))
        };
        var joined = ReportService.JoinMovements(items, movements);
        foreach (var line in joined)
        {
            Console.WriteLine($"  {line}");
        }
        break;

    default:
        Console.WriteLine("Доступні команди:");
        Console.WriteLine("  report-top      - Топ-5 за залишком");
        Console.WriteLine("  report-low      - Дефіцитні товари (< 10)");
        Console.WriteLine("  report-groups   - Групування за одиницями");
        Console.WriteLine("  report-sum      - Загальна статистика");
        Console.WriteLine("  demo-deferred   - Демонстрація відкладеного виконання");
        break;
}

// Допоміжний метод для гарного виводу
static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines)
    {
        Console.WriteLine($"  {l.Sku,-10} {l.Name,-30} {l.Quantity,6} {l.Unit}");
    }
}