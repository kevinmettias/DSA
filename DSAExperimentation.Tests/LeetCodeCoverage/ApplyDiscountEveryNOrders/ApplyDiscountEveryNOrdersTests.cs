using DSAExperimentation.LeetCode.ApplyDiscountEveryNOrders;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ApplyDiscountEveryNOrders;

// Harness only: both strategies live in ApplyDiscountEveryNOrdersSolution and are
// replayed against the same call scripts - LeetCode's published seven-call sequence
// (whose third and sixth calls are the discounted ones), an n = 1 cashier that
// discounts every call, and an n = 2 cashier with a 100% discount so every
// discounted bill is free.
//
// A row is one cashier: the arguments its constructor is published with, plus the
// script of GetBill(Product, Amount) calls and the totals LeetCode says they return.
public sealed class ApplyDiscountEveryNOrdersTests
{
    public static TheoryData<
        (int N, int Discount, int[] Products, int[] Prices),
        (int[] Product, int[] Amount, double Expected)[]> Examples =>
        new()
        {
            {
                (3, 50, [1, 2, 3, 4, 5, 6, 7], [100, 200, 300, 400, 300, 200, 100]),
                [
                    ([1, 2], [1, 2], 500.0),
                    ([3, 7], [10, 10], 4000.0),
                    ([1, 2, 3, 4, 5, 6, 7], [1, 1, 1, 1, 1, 1, 1], 800.0),
                    ([4], [10], 4000.0),
                    ([7, 3], [10, 10], 4000.0),
                    ([7, 5, 3, 1, 6, 4, 2], [10, 10, 10, 9, 9, 9, 7], 7350.0),
                    ([2, 3, 5], [5, 3, 2], 2500.0),
                ]
            },
            {
                (1, 25, [1, 2], [100, 200]),
                [
                    ([1], [1], 75.0),
                    ([2], [1], 150.0),
                ]
            },
            {
                (2, 100, [1, 2], [100, 200]),
                [
                    ([1], [1], 100.0),
                    ([2], [2], 0.0),
                    ([1, 2], [1, 1], 300.0),
                    ([2], [1], 0.0),
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByHashMapLookup_LeetCodeExamples_ReturnsBillWithDiscountEveryNthCall(
        (int N, int Discount, int[] Products, int[] Prices) catalog,
        (int[] Product, int[] Amount, double Expected)[] calls)
    {
        var cashier = ApplyDiscountEveryNOrdersSolution.CreateByHashMapLookup(
            catalog.N, catalog.Discount, catalog.Products, catalog.Prices);

        AssertScript(cashier, calls);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLinearCatalogScan_LeetCodeExamples_ReturnsBillWithDiscountEveryNthCall(
        (int N, int Discount, int[] Products, int[] Prices) catalog,
        (int[] Product, int[] Amount, double Expected)[] calls)
    {
        var cashier = ApplyDiscountEveryNOrdersSolution.CreateByLinearCatalogScan(
            catalog.N, catalog.Discount, catalog.Products, catalog.Prices);

        AssertScript(cashier, calls);
    }

    private static void AssertScript(
        ApplyDiscountEveryNOrdersSolution.ICashier cashier,
        (int[] Product, int[] Amount, double Expected)[] calls)
    {
        foreach (var (product, amount, expected) in calls)
        {
            var bill = cashier.GetBill(product, amount);

            Assert.Equal(expected, bill);
        }
    }
}
