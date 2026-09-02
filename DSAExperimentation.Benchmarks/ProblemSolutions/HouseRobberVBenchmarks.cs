using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HouseRobberV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HouseRobberVSolution's, the same methods
// HouseRobberVTests proves correct. HouseCount is kept well under LC's own 1e5
// bound: MaxAmountByMemoization recurses one call deep per house, and this repo's
// own Memoizer doc comment already flags unbounded recursion depth as an
// inherent trade-off of that primitive - WordBreakBenchmarks' own memoized arm
// already exercises a 3_000-deep chain safely, so this stays at or under that
// proven-safe depth rather than chasing LC's full bound.
[MemoryDiagnoser]
public class HouseRobberVBenchmarks
{
    private const int Seed = 3840;
    private const int MaxValueExclusive = 100_001;
    private const int ColorPoolSize = 5;

    [Params(500, 3_000)]
    public int HouseCount;

    private int[] _nums = null!;
    private int[] _colors = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, HouseCount).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _colors = Enumerable.Range(0, HouseCount).Select(_ => random.Next(1, ColorPoolSize + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long Tabulation() => HouseRobberVSolution.MaxAmountByTabulation(_nums, _colors);

    [Benchmark]
    public long Memoized() => HouseRobberVSolution.MaxAmountByMemoization(_nums, _colors);
}
