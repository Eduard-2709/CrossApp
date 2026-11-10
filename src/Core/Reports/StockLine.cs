namespace Core.Reports;

/// <summary>
/// DTO для рядка звіту (незалежний від доменної сутності Product).
/// </summary>
public sealed record StockLine(
    string Sku, 
    string Name, 
    int Quantity, 
    string Unit
);