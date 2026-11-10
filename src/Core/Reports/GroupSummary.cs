namespace Core.Reports;

/// <summary>
/// DTO для зведення по групі (наприклад, по одиницях виміру).
/// </summary>
public sealed record GroupSummary(
    string Key, 
    int Count, 
    int TotalQuantity
);