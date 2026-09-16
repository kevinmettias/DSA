using DSAExperimentation.LeetCode.ApplyDiscountEveryNOrders;

namespace DSAExperimentation.LeetCode.Tests.ApplyDiscountEveryNOrders;

// The seam between ApplyDiscountEveryNOrdersSolution's two cashiers.
// CreateByLinearCatalogScan keeps the catalogue as the two arrays it arrived in and
// scans them per line item; CreateByHashMapLookup stores it in this repo's own
// DataStructures.HashMap<int, int> and probes it once per line item. The discount
// cadence is shared, so the price lookup is the only thing the two arms can differ
// on - and a price that comes back wrong is invisible in the discount, because the
// cadence applies to whatever total it is handed.
//
// These tests drive complete bills, not single probes: the HashMap is filled once
// by the constructor and read across a whole call lifecycle, so a catalogue that
// resized badly, or a key that collided with another, would show up as a line item
// priced from the wrong entry.
public sealed partial class CashierHashMapCatalogSeamTests
{
    private const int SmallestInterval = 1;
    private const int LargestInterval = 1000;

    [Fact]
    public void GetBill_EveryCustomerDiscounted_MatchesLinearCatalogScan()
    {
        var catalogue = Catalogue(products: 12, priceOf: product => product * 7);
        int[][][] bills = [[[1], [3]], [[2, 3], [4, 5]], [[12], [1]]];

        AssertSameBills(interval: SmallestInterval, discount: 25, catalogue, bills);
    }

    [Fact]
    public void GetBill_NoCustomerDiscountedWithinTheScript_MatchesLinearCatalogScan()
    {
        var catalogue = Catalogue(products: 12, priceOf: product => product * 7);
        int[][][] bills = [[[1], [3]], [[2, 3], [4, 5]], [[12], [1]]];

        AssertSameBills(interval: LargestInterval, discount: 25, catalogue, bills);
    }

    // The two extremes of the discount percentage: a full discount zeroes the bill,
    // and a zero discount has to leave the same total the undiscounted path produced
    // rather than a rounded one.
    [Fact]
    public void GetBill_FullAndZeroDiscount_MatchesLinearCatalogScan()
    {
        var catalogue = Catalogue(products: 8, priceOf: product => (product * 13) + 1);
        int[][][] bills = [[[1, 2], [2, 2]], [[3], [7]], [[8, 1], [5, 5]], [[4, 5, 6], [1, 1, 1]]];

        AssertSameBills(interval: 2, discount: 100, catalogue, bills);
        AssertSameBills(interval: 2, discount: 0, catalogue, bills);
    }

    // The catalogue at the size the problem allows it to reach: one HashMap that has
    // had to grow to hold every product, read by a bill naming all of them.
    [Fact]
    public void GetBill_MaxSizeCatalogue_MatchesIndependentlySummedBill()
    {
        var catalogue = Catalogue(products: 200, priceOf: product => product);
        int[] allProducts = [.. Enumerable.Range(1, 200)];
        int[] allAmounts = [.. Enumerable.Repeat(2, 200)];
        var cashier = ApplyDiscountEveryNOrdersSolution.CreateByHashMapLookup(
            discountInterval: 2, discount: 30, catalogue.Products, catalogue.Prices);

        var firstBill = cashier.GetBill(allProducts, allAmounts);
        var secondBill = cashier.GetBill(allProducts, allAmounts);

        Assert.Equal(2 * SumOfPrices(catalogue), firstBill);
        Assert.Equal(2 * SumOfPrices(catalogue) * 0.7, secondBill, precision: 6);
    }

    // A lifecycle long enough to walk the cadence past several discounts and back,
    // with every call naming a different slice of the catalogue.
    [Fact]
    public void GetBill_CadenceAcrossManyCustomers_MatchesLinearCatalogScan()
    {
        var catalogue = Catalogue(products: 20, priceOf: product => (product * 3) + 2);
        int[][][] bills = [.. Enumerable.Range(0, 30).Select(CyclingBill)];

        AssertSameBills(interval: 4, discount: 15, catalogue, bills);
    }

    private static void AssertSameBills(int interval, int discount, (int[] Products, int[] Prices) catalogue, int[][][] bills)
    {
        var composed = ApplyDiscountEveryNOrdersSolution.CreateByHashMapLookup(
            interval, discount, catalogue.Products, catalogue.Prices);
        var reference = ApplyDiscountEveryNOrdersSolution.CreateByLinearCatalogScan(
            interval, discount, catalogue.Products, catalogue.Prices);

        foreach (var (product, amount) in bills.Select(bill => (bill[0], bill[1])))
        {
            Assert.Equal(reference.GetBill(product, amount), composed.GetBill(product, amount));
        }
    }

    private static (int[] Products, int[] Prices) Catalogue(int products, Func<int, int> priceOf)
        => ([.. Enumerable.Range(1, products)], [.. Enumerable.Range(1, products).Select(priceOf)]);

    private static int[][] CyclingBill(int billIndex)
    {
        var product = (billIndex % 20) + 1;

        return [[product], [(product % 20) + 1]];
    }

    private static double SumOfPrices((int[] Products, int[] Prices) catalogue)
        => catalogue.Prices.Sum(price => (double)price);
}
