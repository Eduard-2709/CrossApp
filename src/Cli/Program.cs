// using System.Text;
// using System.Text.Json;
// using Core;

// // Встановлюємо кодування для правильної роботи з кирилицею
// Console.OutputEncoding = Encoding.UTF8;

// // Збираємо інформацію про середовище (вся логіка — в Core)
// EnvironmentReport report = EnvironmentInfo.Collect();

// // Перевіряємо аргумент --json
// bool isJsonOutput = args.Length > 0 && args[0] == "--json";

// if (isJsonOutput)
// {
//     // JSON вивід
//     var info = new
//     {
//         student = new
//         {
//             name = "Едуард",
//             surname = "Боднар",
//             group = "ФЕІ-34с"
//         },
//         environment = new
//         {
//             os = report.OsDescription,
//             runtime = report.FrameworkDescription,
//             architecture = report.ProcessArchitecture,
//             rid_detected = report.DetectedRid,
//             rid_reported = report.ReportedRid,
//             clr_version = report.ClrVersion,
//             base_directory = report.BaseDirectory,
//             current_directory = report.CurrentDirectory
//         },
//         domain = new
//         {
//             name = "Склад",
//             entities = new[] { "товари", "партії", "залишки", "переміщення" },
//             purpose = "облік залишків товарів по партіях"
//         }
//     };

//     string json = JsonSerializer.Serialize(info, new JsonSerializerOptions
//     {
//         WriteIndented = false,
//         Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
//     });
//     Console.WriteLine(json);
// }
// else
// {
//     // Табличний вивід (лише форматування, без логіки)
//     Console.WriteLine("CrossApp – інформація про середовище");
//     Console.WriteLine("Студент: Боднар Едуард, група ФЕІ-34с");
//     Console.WriteLine(new string('-', 52));
//     Console.WriteLine($"ОС               : {report.OsDescription}");
//     Console.WriteLine($"Runtime          : {report.FrameworkDescription}");
//     Console.WriteLine($"Архітектура      : {report.ProcessArchitecture}");
//     Console.WriteLine($"Версія .NET (CLR): {report.ClrVersion}");
//     Console.WriteLine($"RID (визначено)  : {report.DetectedRid}");
//     Console.WriteLine($"RID (від .NET)   : {report.ReportedRid}");
//     Console.WriteLine($"Каталог застосунку: {report.BaseDirectory}");
//     Console.WriteLine($"Поточний каталог : {report.CurrentDirectory}");
//     Console.WriteLine(new string('-', 52));
//     Console.WriteLine("Предметна область: Склад (товари, партії, залишки, переміщення)");
// }




using Core.Dto;
using Core.Import;

// Шлях до файлу: з args[0] або за замовчуванням data/sample.csv
string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

// Перевірка існування файлу
if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

// Імпорт
ImportResult<ProductDto> result = ProductCsvImporter.Load(path);

// Вивід кількості
Console.WriteLine($"Завантажено записів: {result.Items.Count}");

// Вивід перших 5 записів
foreach (ProductDto p in result.Items.Take(5))
{
    Console.WriteLine($"  {p.Id,-6} {p.Sku,-10} {p.Name,-26} {p.Quantity,5} {p.Unit}");
}

// Вивід помилок
if (result.Errors.Count > 0)
{
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
    {
        Console.WriteLine($"  ! {e}");
    }
}

return 0;