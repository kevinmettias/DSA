using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ProductOfTheLastKNumbers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ProductOfTheLastKNumbersSolution's, the same
// strategies ProductOfTheLastKNumbersSolutionTests proves correct. Setup adds Length
// non-zero numbers to each store (no resets), charging that construction to
// [GlobalSetup], and each arm answers GetProduct(Length) - the full window - forcing
// the raw-replay strategy through its full O(Length) worst case instead of an early
// exit making it look artificially competitive, TwoSumBenchmarks' convention.
//
// LC 1352 guarantees the product of the stream fits in 32 bits at every point, so the
// numbers come from BoundedProductDraws: ones, with a few planted factors from 2 to 9
// whose product stays inside an int.
public class ProductOfTheLastKNumbersBenchmarks
{
    private const int MaxFactorValueExclusive = 10;

    // Fixed so every run measures the same stream of factors.
    private const int FactorSeed = 1;

    private ProductOfTheLastKNumbersSolution.IProductOfNumbers _rawStreamReplay = null!;

    private ProductOfTheLastKNumbersSolution.IProductOfNumbers _prefixProductDivision = null!;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = BoundedProductDraws.Factors(Length, MaxFactorValueExclusive, new Random(FactorSeed));

        _rawStreamReplay = Seed(ProductOfTheLastKNumbersSolution.CreateByRawStreamReplay(), values);
        _prefixProductDivision =
            Seed(ProductOfTheLastKNumbersSolution.CreateByPrefixProductDivision(), values);
    }

    private static ProductOfTheLastKNumbersSolution.IProductOfNumbers Seed(
        ProductOfTheLastKNumbersSolution.IProductOfNumbers numbers, int[] values)
    {
        foreach (var value in values)
        {
            numbers.Add(value);
        }

        return numbers;
    }

    [Benchmark(Baseline = true)]
    public int ReplayLastKFromRawStream() => _rawStreamReplay.GetProduct(Length);

    [Benchmark]
    public int PrefixProductDivision() => _prefixProductDivision.GetProduct(Length);
}
