using Core.Domain;

namespace Core;

/// <summary>
/// Початкові дані для демонстрації (15-20 записів).
/// </summary>
public static class SampleData
{
    public static IEnumerable<Product> Products()
    {
        yield return Product.Create("P-001", "SKU-001", "Цемент М400 25кг", "шт", 120);
        yield return Product.Create("P-002", "SKU-002", "Пісок будівельний", "т", 18);
        yield return Product.Create("P-003", "SKU-003", "Цегла червона", "шт", 4200);
        yield return Product.Create("P-004", "SKU-004", "Фарба водоемульсійна 10л", "шт", 36);
        yield return Product.Create("P-005", "SKU-005", "Шпаклівка фінішна", "кг", 250);
        yield return Product.Create("P-006", "SKU-006", "Профіль CD-60", "м", 800);
        yield return Product.Create("P-007", "SKU-007", "Саморізи 3.5х25", "уп", 140);
        yield return Product.Create("P-008", "SKU-008", "Ґрунтовка глибокого проникнення", "л", 60);
        yield return Product.Create("P-009", "SKU-009", "Плитка керамічна", "м2", 310);
        yield return Product.Create("P-010", "SKU-010", "Клей плитковий", "кг", 540);
        yield return Product.Create("P-011", "SKU-011", "Гіпсокартон 12.5мм", "лист", 85);
        yield return Product.Create("P-012", "SKU-012", "Стрічка армуюча", "м", 200);
        yield return Product.Create("P-013", "SKU-013", "Бетоноконтакт", "л", 45);
        yield return Product.Create("P-014", "SKU-014", "Піна монтажна", "шт", 72);
        yield return Product.Create("P-015", "SKU-015", "Герметик силіконовий", "шт", 110);
    }
}