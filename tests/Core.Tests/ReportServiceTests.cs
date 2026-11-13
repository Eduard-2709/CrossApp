using Core.Reports;
using Xunit;

namespace Core.Tests;

public class ReportServiceTests
{
    [Theory]
    [InlineData(10, 1)] // Менше 10: тільки 1 (SKU-SML)
    [InlineData(60, 2)] // Менше 60: 2 (SKU-SML, SKU-MID)
    public void LowStock_CountsOnlyBelowThreshold(int threshold, int expected)
    {
        var result = ReportService.LowStock(TestData.Items(), threshold);
        Assert.Equal(expected, result.Count);
    }

    [Fact]
    public void TopByQuantity_IsDeterministic()
    {
        // Сортує за кількістю (100, 50). Перевіряє SKU.
        var result = ReportService.TopByQuantity(TestData.Items(), 2);
        Assert.Equal(new[] { "SKU-BIG", "SKU-MID" }, result.Select(x => x.Sku));
    }
}