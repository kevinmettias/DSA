using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SplitTheArrayToMakeCoprimeProducts;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SplitTheArrayToMakeCoprimeProductsSolution's, the
// same methods SplitTheArrayToMakeCoprimeProductsTests proves correct. The
// direct-definition arm carries a running BigInteger left product and the
// complementary right product and takes a gcd at every candidate split (values
// overflow long within a handful of elements); the composed arm never multiplies
// anything, because a split at i is coprime exactly when no prime factor seen in
// nums[0..i] reappears past i - so recording each prime's rightmost occurrence in
// one HashMap pass turns the problem into a linear scan of a running max index
// against i. Values are drawn from a small shared prime pool (the same "force
// genuine overlaps" intent LargestComponentSizeByCommonFactorBenchmarks' generator
// already uses) so both arms do real, non-trivial work.
[MemoryDiagnoser]
public class SplitTheArrayToMakeCoprimeProductsBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2584;

    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13, 17, 19];

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BigIntegerProductScan() =>
        SplitTheArrayToMakeCoprimeProductsSolution.FindValidSplitByProductGcd(_nums);

    [Benchmark]
    public int PrimeLastOccurrenceSweep() =>
        SplitTheArrayToMakeCoprimeProductsSolution.FindValidSplitByPrimeLastOccurrence(_nums);
}
