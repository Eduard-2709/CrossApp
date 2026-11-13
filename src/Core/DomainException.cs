namespace Core;

/// <summary>
/// Виняток для порушення бізнес-правил (на відміну від ArgumentException).
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    
    public DomainException(string message, Exception inner) : base(message, inner) { }
}