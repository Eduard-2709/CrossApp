namespace Core.Events;

/// <summary>
/// Види змін, які можуть відбуватися в каталозі.
/// </summary>
public enum CatalogChangeKind
{
    Added,
    Received,
    Issued,
    Removed
}

/// <summary>
/// Дані, які передаються підписникам при виникненні події.
/// Успадковується від EventArgs (стандарт .NET).
/// </summary>
public sealed class CatalogChangedEventArgs(
    string id, 
    CatalogChangeKind kind, 
    int delta, 
    int quantityAfter) : EventArgs
{
    public string Id { get; } = id;
    
    public CatalogChangeKind Kind { get; } = kind;
    
    // На скільки змінилась кількість (+50, -25)
    public int Delta { get; } = delta; 
    
    public int QuantityAfter { get; } = quantityAfter;
    
    // Час події фіксується в момент створення
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}