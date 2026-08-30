using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Apply Discount Every n Orders (LC 1357): TwoSumBenchmarks' exact "O(n) linear scan
// vs. O(1) HashMap<TKey,TValue> lookup" contrast, applied to Cashier's own per-line-
// item price lookup instead of a pair-sum check. Setup builds ProductCount catalog
// entries and one bill that requests every product id, in reverse catalog order, so
// the linear scan is forced through its full worst-case pass per lookup instead of an
// early exit making it look artificially competitive. Only the repeated price-lookup
// pass is measured - both benchmarks reuse a pre-built catalog the same way
// TimeBasedKeyValueStoreBenchmarks reuses a pre-built history instead of re-timing
// construction.
[MemoryDiagnoser]
public class ApplyDiscountEveryNOrdersBenchmarks
{
    [Params(200, 5_000)]
    public int ProductCount;

    private int[] _catalogProductIds = null!;
    private int[] _catalogPrices = null!;
    private HashMap<int, int> _priceByProduct = null!;
    private int[] _billProductIds = null!;
    private int[] _billAmounts = null!;

    [GlobalSetup]
    public void Setup()
    {
        _catalogProductIds = Enumerable.Range(1, ProductCount).ToArray();
        _catalogPrices = Enumerable.Range(1, ProductCount).Select(i => i * 10).ToArray();

        _priceByProduct = new HashMap<int, int>();
        for (var i = 0; i < _catalogProductIds.Length; i++)
        {
            _priceByProduct.Set(_catalogProductIds[i], _catalogPrices[i]);
        }

        _billProductIds = [.. _catalogProductIds.Reverse()];
        _billAmounts = Enumerable.Repeat(1, ProductCount).ToArray();
    }

    [Benchmark(Baseline = true)]
    public double LinearScanLookup()
    {
        double total = 0;

        for (var i = 0; i < _billProductIds.Length; i++)
        {
            var productId = _billProductIds[i];
            for (var p = 0; p < _catalogProductIds.Length; p++)
            {
                if (_catalogProductIds[p] == productId)
                {
                    total += _catalogPrices[p] * _billAmounts[i];
                    break;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public double HashMapLookup()
    {
        double total = 0;

        for (var i = 0; i < _billProductIds.Length; i++)
        {
            if (_priceByProduct.TryGetValue(_billProductIds[i], out var price))
            {
                total += price * _billAmounts[i];
            }
        }

        return total;
    }
}
