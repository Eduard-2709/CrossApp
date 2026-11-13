using Core.Domain;

namespace Core.Reports;

/// <summary>
/// Сервіс аналітичних звітів (LINQ to Objects).
/// Приймає IEnumerable<Product>, а не сховище — це робить методи чистими та тестованими.
/// </summary>
public static class ReportService
{
    // 1) Фільтр + сортування + проєкція (Where, OrderBy, Select)
    public static IReadOnlyList<StockLine> LowStock(IEnumerable<Product> source, int threshold) =>
        source
            .Where(p => p.Quantity < threshold) // залишає тільки дефіцитні товари
            .OrderBy(p => p.Quantity)
            .ThenBy(p => p.Sku, StringComparer.Ordinal)
            .Select(p => new StockLine(p.Sku, p.Name, p.Quantity, p.Unit)) // перетворюємо сутність на простий Stockline
            .ToList();

    // 2) Топ-N зі стабільним порядком (OrderByDescending, ThenBy, Take)
    public static IReadOnlyList<StockLine> TopByQuantity(IEnumerable<Product> source, int n) =>
        source
            .OrderByDescending(p => p.Quantity) // по спаданню
            .ThenBy(p => p.Sku, StringComparer.Ordinal)
            .Take(n) // бере перші n-елементи
            .Select(p => new StockLine(p.Sku, p.Name, p.Quantity, p.Unit))
            .ToList();

    // 3) Групування з підрахунком і сумою (GroupBy, Count, Sum)
    public static IReadOnlyList<GroupSummary> ByUnit(IEnumerable<Product> source) =>
        source
            .GroupBy(p => p.Unit, StringComparer.OrdinalIgnoreCase)
            .Select(g => new GroupSummary(g.Key, g.Count(), g.Sum(p => p.Quantity)))
            .OrderByDescending(s => s.TotalQuantity)
            .ToList();

    // 4) Агрегати з обробкою порожньої послідовності (Count, Sum, Average, Max)
    public static (int Items, int Total, double Average, int Max) Totals(IEnumerable<Product> source)
    {
        var list = source.ToList();
        
        // Якщо список порожній, Average і Max викинуть виняток, тому обробляємо вручну
        return list.Count == 0
            ? (0, 0, 0d, 0)
            : (list.Count, 
               list.Sum(p => p.Quantity), 
               list.Average(p => p.Quantity), 
               list.Max(p => p.Quantity));
    }

    
    // ==========================================================
    // ДОДАТКОВЕ ЗАВДАННЯ 1: Join
    // ==========================================================
    public static IReadOnlyList<string> JoinMovements(
        IEnumerable<Product> products, 
        IEnumerable<MovementDto> movements)
    {
        // З'єднуємо товари та переміщення за SKU
        var query = from p in products
                    join m in movements on p.Sku equals m.Sku
                    select $"{p.Sku} - {p.Name}: переміщено {m.Amount} ({m.Date:yyyy-MM-dd})";

        return query.ToList();
    }
}

    // // ==========================================================
    // // ДОДАТКОВЕ ЗАВДАННЯ 2: Власний метод-розширення
    // // ==========================================================
    // public static IEnumerable<T> TopBy<T>(
    //     this IEnumerable<T> source, 
    //     Func<T, int> selector, 
    //     int n) =>
    //     source
    //         .OrderByDescending(selector)
    //         .Take(n);