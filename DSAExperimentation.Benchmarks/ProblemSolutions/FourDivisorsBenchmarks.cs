using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FourDivisors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FourDivisorsSolution's, the same methods
// FourDivisorsTests proves correct - the textbook O(num) full-range trial division per
// number against BinarySearch.LowerBound anchoring each number's scan at
// floor(sqrt(num)), then a short downward walk that bails out the moment a fifth
// divisor appears. The random numbers are built once in [GlobalSetup], so generation
// is not charged to either arm.
[MemoryDiagnoser]
public class FourDivisorsBenchmarks
{
    // LC problem number, reused as the deterministic workload seed.
    private const int RandomSeed = 1390;
    private const int MaxGeneratedNumber = 20_000;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxGeneratedNumber)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRangeScan() => FourDivisorsSolution.SumFourDivisorsByFullRangeScan(_nums);

    [Benchmark]
    public int BinarySearchAnchored() => FourDivisorsSolution.SumFourDivisorsByBinarySearchAnchor(_nums);
}
