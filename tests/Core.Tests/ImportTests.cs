using Core;
using Core.Services;
using Core.Storage;
using Xunit;

namespace Core.Tests;

public class ImportTests
{
    [Fact]
    public async Task ImportAsync_WhenAlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var service = new CatalogService(new InMemoryCatalogStore());
        
        // ВИПРАВЛЕНО: Створюємо тимчасовий файл, щоб FileStream міг його відкрити
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "[]");

        try
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => service.ImportAsync(new JsonCatalogImporter(), tempFile, cts.Token));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
    
    [Fact]
    public async Task ImportAsync_WithInvalidFile_ThrowsDomainException()
    {
        var tempFile = Path.GetTempFileName();
        // Записуємо зіпсований JSON (порожній ID)
        await File.WriteAllTextAsync(tempFile, "[{\"Id\":\"\", \"Sku\":\"S\", \"Name\":\"Test\", \"Unit\":\"шт\", \"Quantity\":10}]");
        
        var service = new CatalogService(new InMemoryCatalogStore());
        
        try
        {
            await Assert.ThrowsAsync<DomainException>(
                () => service.ImportAsync(new JsonCatalogImporter(), tempFile));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}