// using System.Text.Json;
// using Core.Abstractions;
// using Core.Domain;
// using Core.Dto;

// namespace Core.Storage;

// /// <summary>
// /// Сховище у JSON-файлі.
// /// Кешує дані в пам'яті, читає файл при першому зверненні, записує після кожної зміни.
// /// </summary>
// public sealed class FileCatalogStore(string path) : ICatalogStore
// {
//     private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
//     private readonly Dictionary<string, Product> _cache = new(StringComparer.OrdinalIgnoreCase);
//     private readonly string _path = Path.GetFullPath(path);
//     private bool _loaded;

//     private void EnsureLoaded()
//     {
//         if (_loaded) return;
        
//         if (File.Exists(_path))
//         {
//             var dtos = JsonSerializer.Deserialize<List<ProductDto>>(File.ReadAllText(_path)) ?? [];
//             foreach (var dto in dtos)
//             {
//                 var p = Product.FromDto(dto); // FromDto проходить всі інваріанти!
//                 _cache[p.Id] = p;
//             }
//         }
//         _loaded = true;
//     }

//     private void Flush()
//     {
//         Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
//         // Серіалізуємо DTO, а не сутність, бо у сутності приватні сетери
//         File.WriteAllText(_path, JsonSerializer.Serialize(_cache.Values.Select(p => p.ToDto()).ToList(), Options));
//     }

//     public IReadOnlyList<Product> List()
//     {
//         EnsureLoaded();
//         return _cache.Values.ToList();
//     }

//     public Product? GetById(string id)
//     {
//         EnsureLoaded();
//         return _cache.GetValueOrDefault(id);
//     }

//     public void Add(Product item)
//     {
//         ArgumentNullException.ThrowIfNull(item);
//         EnsureLoaded();
//         if (_cache.ContainsKey(item.Id))
//             throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");
        
//         _cache.Add(item.Id, item);
//         Flush();
//     }

//     public void Update(Product item)
//     {
//         EnsureLoaded();
//         _cache[item.Id] = item;
//         Flush();
//     }

//     public bool Remove(string id)
//     {
//         EnsureLoaded();
//         if (!_cache.Remove(id)) return false;
//         Flush();
//         return true;
//     }
// }





using System.Text.Encodings.Web;
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

/// <summary>
/// Сховище у JSON-файлі.
/// Кешує дані в пам'яті, читає файл при першому зверненні, записує після кожної зміни.
/// Запис атомарний: .tmp + File.Move(overwrite: true).
/// </summary>
public sealed class FileCatalogStore(string path) : ICatalogStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly Dictionary<string, Product> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _path = Path.GetFullPath(path);
    private bool _loaded;

    private void EnsureLoaded()
    {
        if (_loaded) return;

        if (!File.Exists(_path))
        {
            _loaded = true;
            return;
        }

        try
        {
            var text = File.ReadAllText(_path);
            var dtos = JsonSerializer.Deserialize<List<ProductDto>>(text, Options) ?? [];
            _cache.Clear();
            foreach (var dto in dtos)
            {
                var p = Product.FromDto(dto); // FromDto проходить всі інваріанти!
                _cache[p.Id] = p;
            }
        }
        catch (JsonException ex)
        {
            // Зрозуміле повідомлення замість сирого stack trace
            throw new InvalidOperationException(
                $"Файл каталогу '{_path}' містить некоректний JSON: {ex.Message}", ex);
        }

        _loaded = true;
    }

    private void Flush()
    {
        var dtos = _cache.Values.Select(p => p.ToDto()).ToList();
        var tmp = _path + ".tmp";

        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(tmp, JsonSerializer.Serialize(dtos, Options));
        File.Move(tmp, _path, overwrite: true); // атомарна заміна
    }

    public IReadOnlyList<Product> List()
    {
        EnsureLoaded();
        return _cache.Values.ToList();
    }

    public Product? GetById(string id)
    {
        EnsureLoaded();
        return _cache.GetValueOrDefault(id);
    }

    public void Add(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();
        if (_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");

        _cache.Add(item.Id, item);
        Flush();
    }

    public void Update(Product item)
    {
        EnsureLoaded();
        _cache[item.Id] = item;
        Flush();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();
        if (!_cache.Remove(id)) return false;
        Flush();
        return true;
    }
}