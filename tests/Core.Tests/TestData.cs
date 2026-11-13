using Core.Domain;

namespace Core.Tests;

public static class TestData
{
    public static IEnumerable<Product> Items()
    {
        yield return Product.Create("P-1", "SKU-BIG", "Великий", "шт", 100);
        yield return Product.Create("P-2", "SKU-MID", "Середній", "шт", 50);
        yield return Product.Create("P-3", "SKU-SML", "Малий", "шт", 5);
    }
}