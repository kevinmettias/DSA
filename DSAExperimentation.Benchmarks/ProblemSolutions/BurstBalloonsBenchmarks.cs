using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BurstBalloons;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BurstBalloonsSolution's, the same methods
// BurstBalloonsTests proves correct. Each arm is handed the prepared
// PaddedBalloons its hoisted overload takes, so the boundary-padding pass is
// charged to [GlobalSetup] rather than to the recursion being measured.
// BalloonCount is kept modest (<=14) specifically because the un-memoized
// baseline's blowup is real, the same reasoning FibonacciBenchmarks.cs's
// NaiveRecursive already documents.
[MemoryDiagnoser]
public class BurstBalloonsBenchmarks
{
    private const int RandomSeed = 1;

    private PaddedBalloons _padded;

    [Params(10, 14)]
    public int BalloonCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var nums = BurstBalloonsWorkloads.BuildBalloons(BalloonCount, seed: RandomSeed);
        _padded = PaddedBalloons.FromNums(nums);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => BurstBalloonsSolution.MaxCoinsByUnmemoizedRecursion(_padded);

    [Benchmark]
    public int MemoizedRecursion() => BurstBalloonsSolution.MaxCoinsByMemoizedRecursion(_padded);
}
