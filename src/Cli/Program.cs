// // using System.Runtime.InteropServices;

// // Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
// // Console.WriteLine("Студент: Боднар Едуард, група ФЕІ-34");
// // Console.WriteLine(new string('-', 52));
// // Console.WriteLine($"ОС (OSDescription)  : {RuntimeInformation.OSDescription}");
// // Console.WriteLine($"ОС (Environment)    : {Environment.OSVersion}");
// // Console.WriteLine($"Архітектура процесу : {RuntimeInformation.ProcessArchitecture}");
// // Console.WriteLine($"Версія .NET (CLR)   : {Environment.Version}");
// // Console.WriteLine($"Runtime             : {RuntimeInformation.FrameworkDescription}");
// // Console.WriteLine($"Каталог застосунку  : {AppContext.BaseDirectory}");
// // Console.WriteLine($"Поточний каталог    : {Environment.CurrentDirectory}");
// // Console.WriteLine(new string('-', 52));
// // Console.WriteLine("Предметна область: Склад (товари, партії, залишки, переміщення)");

// using System.Runtime.InteropServices;
// using System.Text;
// using System.Text.Json;

// // Перевіряємо чи цей аргумент є json
// bool isJsonOutput = args.Length > 0 && args[0] == "--json";

// if (isJsonOutput)
// {
//     // Друге додаткове завдання JSON ВИВІД
//     var info = new
//     {
//         student = new
//         {
//             name = "Едуард",
//             surname = "Боднар",
//             group = "ФЕІ-34с"
//         },
//         os = new
//         {
//             description = RuntimeInformation.OSDescription,
//             version = Environment.OSVersion.ToString(),
//             architecture = RuntimeInformation.ProcessArchitecture.ToString()
//         },
//         dotnet = new
//         {
//             clr_version = Environment.Version.ToString(),
//             runtime = RuntimeInformation.FrameworkDescription,
//             rid = RuntimeInformation.RuntimeIdentifier
//         },
//         paths = new
//         {
//             app_directory = AppContext.BaseDirectory,
//             current_directory = Environment.CurrentDirectory
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
//     Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
//     Console.WriteLine("Студент: Боднар Едуард, група ФЕІ-34с");
//     Console.WriteLine(new string('-', 52));
//     Console.WriteLine($"ОС (OSDescription)   : {RuntimeInformation.OSDescription}");
//     Console.WriteLine($"ОС (Environment)     : {Environment.OSVersion}");
//     Console.WriteLine($"Архітектура процесу  : {RuntimeInformation.ProcessArchitecture}");
//     Console.WriteLine($"Версія .NET (CLR)    : {Environment.Version}");
//     Console.WriteLine($"Runtime              : {RuntimeInformation.FrameworkDescription}");
//     Console.WriteLine($"Каталог застосунку   : {AppContext.BaseDirectory}");
//     Console.WriteLine($"Поточний каталог     : {Environment.CurrentDirectory}");
//     Console.WriteLine(new string('-', 52));
//     Console.WriteLine("Предметна область: Склад (товари, партії, залишки, переміщення)");
// }




using System.Text;
using System.Text.Json;
using Core;

// Встановлюємо кодування для правильної роботи з кирилицею
Console.OutputEncoding = Encoding.UTF8;

// Збираємо інформацію про середовище (вся логіка — в Core)
EnvironmentReport report = EnvironmentInfo.Collect();

// Перевіряємо аргумент --json
bool isJsonOutput = args.Length > 0 && args[0] == "--json";

if (isJsonOutput)
{
    // JSON вивід
    var info = new
    {
        student = new
        {
            name = "Едуард",
            surname = "Боднар",
            group = "ФЕІ-34с"
        },
        environment = new
        {
            os = report.OsDescription,
            runtime = report.FrameworkDescription,
            architecture = report.ProcessArchitecture,
            rid_detected = report.DetectedRid,
            rid_reported = report.ReportedRid,
            clr_version = report.ClrVersion,
            base_directory = report.BaseDirectory,
            current_directory = report.CurrentDirectory
        },
        domain = new
        {
            name = "Склад",
            entities = new[] { "товари", "партії", "залишки", "переміщення" },
            purpose = "облік залишків товарів по партіях"
        }
    };

    string json = JsonSerializer.Serialize(info, new JsonSerializerOptions
    {
        WriteIndented = false,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
    Console.WriteLine(json);
}
else
{
    // Табличний вивід (лише форматування, без логіки)
    Console.WriteLine("CrossApp – інформація про середовище");
    Console.WriteLine("Студент: Боднар Едуард, група ФЕІ-34с");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"ОС               : {report.OsDescription}");
    Console.WriteLine($"Runtime          : {report.FrameworkDescription}");
    Console.WriteLine($"Архітектура      : {report.ProcessArchitecture}");
    Console.WriteLine($"Версія .NET (CLR): {report.ClrVersion}");
    Console.WriteLine($"RID (визначено)  : {report.DetectedRid}");
    Console.WriteLine($"RID (від .NET)   : {report.ReportedRid}");
    Console.WriteLine($"Каталог застосунку: {report.BaseDirectory}");
    Console.WriteLine($"Поточний каталог : {report.CurrentDirectory}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine("Предметна область: Склад (товари, партії, залишки, переміщення)");
}