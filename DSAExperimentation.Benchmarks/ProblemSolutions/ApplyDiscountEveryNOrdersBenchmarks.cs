using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ApplyDiscountEveryNOrders;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ApplyDiscountEveryNOrdersSolution's, the same strategies
// ApplyDiscountEveryNOrdersTests proves correct. Setup builds ProductCount catalogue
// entries, hands each strategy its own cashier - charging catalogue construction to
// [GlobalSetup] the way TimeBasedKeyValueStoreBenchmarks charges its history - and
// builds one bill that requests every product id in reverse catalogue order, so the
// linear scan is forced through its full worst-case pass per lookup instead of an
// early exit making it look artificially competitive.
[MemoryDiagnoser]
public class ApplyDiscountEveryNOrdersBenchmarks
{
    private const int PricePerUnit = 10;
    private const int DiscountEvery = 3;
    private const int DiscountPercent = 50;

    [Params(200, 5_000)]
    public int ProductCount;

    private ApplyDiscountEveryNOrdersSolution.ICashier _linearCatalogScan = null!;
    private ApplyDiscountEveryNOrdersSolution.ICashier _hashMapLookup = null!;
    private int[] _billProductIds = null!;
    private int[] _billAmounts = null!;

    [GlobalSetup]
    public void Setup()
    {
        var catalogProductIds = Enumerable.Range(1, ProductCount).ToArray();
        var catalogPrices = Enumerable.Range(1, ProductCount).Select(i => i * PricePerUnit).ToArray();

        _linearCatalogScan = ApplyDiscountEveryNOrdersSolution.CreateByLinearCatalogScan(
            DiscountEvery, DiscountPercent, catalogProductIds, catalogPrices);
        _hashMapLookup = ApplyDiscountEveryNOrdersSolution.CreateByHashMapLookup(
            DiscountEvery, DiscountPercent, catalogProductIds, catalogPrices);

        _billProductIds = [.. catalogProductIds.Reverse()];
        _billAmounts = Enumerable.Repeat(1, ProductCount).ToArray();
    }

    [Benchmark(Baseline = true)]
    public double LinearScanLookup() => _linearCatalogScan.GetBill(_billProductIds, _billAmounts);

    [Benchmark]
    public double HashMapLookup() => _hashMapLookup.GetBill(_billProductIds, _billAmounts);
}
