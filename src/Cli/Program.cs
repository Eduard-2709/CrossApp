using System.Reflection;
using Core;
using Core.Abstractions;
using Core.Configuration;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;
using Microsoft.Extensions.Configuration;

// 1. Побудова конфігурації (порядок джерел: файл → профіль → user-secrets → env → CLI)
var envName = Environment.GetEnvironmentVariable("CROSSAPP_ENVIRONMENT") ?? "Development";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{envName}.json", optional: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables(prefix: "CROSSAPP_")
    .AddCommandLine(args)
    .Build();

var options = config.Get<AppOptions>() ?? new AppOptions();

// Підтримка простої змінної CROSSAPP_DATA_PATH (перекриває Storage:DataPath)
var legacyPath = Environment.GetEnvironmentVariable("CROSSAPP_DATA_PATH");
var storage = new StorageOptions
{
    DataPath = string.IsNullOrWhiteSpace(legacyPath) ? options.Storage.DataPath : legacyPath,
    FileName = options.Storage.FileName
};

// 2. Composition Root — шлях до файлу тепер із конфігурації
bool useFile = args.Contains("--file");
string dataPath = storage.ResolveFullPath(AppContext.BaseDirectory);

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

// 3. Діагностика (як у методичці)
var apiKey = config["Integration:ApiKey"];
Console.WriteLine($"Файл даних : {dataPath}");
Console.WriteLine(apiKey is null
    ? "ApiKey: не заданий"
    : $"ApiKey: присутній, довжина {apiKey.Length}");

var items = service.All();

// 4. Визначаємо команду (ігноруємо прапорці типу --file та ключі конфігурації --Key=Value)
var command = args.FirstOrDefault(a => !a.StartsWith("--") && !a.Contains('=')) ?? "report-top";