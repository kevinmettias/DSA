using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Product of the Last K Numbers (LC 1352): replaying the last k multiplications from
// the raw stream (O(k) per query) vs. dividing two entries of a running prefix-product
// DynamicArray<long> built incrementally as numbers arrive (O(1) per query) - the same
// "precompute once, answer in O(1)" contrast TimeBasedKeyValueStoreBenchmarks draws
// between its linear floor scan and BinarySearch.UpperBound. Setup adds Length
// non-zero numbers (no resets), and each benchmark answers GetProduct(Length) - the
// full window - forcing the raw-replay strategy through its full O(Length) worst case
// instead of an early exit making it look artificially competitive, TwoSumBenchmarks'
// convention.
[MemoryDiagnoser]
public class ProductOfTheLastKNumbersBenchmarks
{
    private const int MaxFactorValueExclusive = 10;

    [Params(200, 5_000)]
    public int Length;

    private int[] _rawValues = null!;
    private DynamicArray<long> _prefixProducts = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _rawValues = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxFactorValueExclusive)).ToArray();

        _prefixProducts = new DynamicArray<long>();
        _prefixProducts.Add(1);
        foreach (var value in _rawValues)
        {
            _prefixProducts.Add(_prefixProducts.Get(_prefixProducts.Count - 1) * value);
        }
    }

    [Benchmark(Baseline = true)]
    public long ReplayLastKFromRawStream()
    {
        long product = 1;
        for (var i = 0; i < _rawValues.Length; i++)
        {
            product *= _rawValues[i];
        }

        return product;
    }

    [Benchmark]
    public long PrefixProductDivision()
    {
        var n = _prefixProducts.Count - 1;
        return _prefixProducts.Get(n) / _prefixProducts.Get(0);
    }
}
