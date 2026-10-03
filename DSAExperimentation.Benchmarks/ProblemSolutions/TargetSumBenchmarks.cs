using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TargetSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TargetSumSolution's, the same methods TargetSumSolutionTests
// proves correct. ElementCount is kept modest specifically because the unmemoized
// baseline's 2^N blowup is real, the same reasoning FibonacciNumberBenchmarks documents,
// and stops at LC 494's 20 elements.
public class TargetSumBenchmarks
{
    private const int RandomSeed = 494; // LC problem number
    private const int RandomValueUpperBoundExclusive = 10;
    private const int SignFlipMultiplier = 2; private int[] _nums = [];

    private int _target;
    // flips nums[0] from + to - in the target sum

    [Params(18, 20)]
    public int ElementCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(ElementCount, 1, RandomValueUpperBoundExclusive, random);
        _target = _nums.Sum() - (SignFlipMultiplier * _nums[0]);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => TargetSumSolution.WaysByUnmemoizedRecursion(_nums, _target);

    [Benchmark]
    public int MemoizedRecursion() => TargetSumSolution.WaysByMemoizedRecursion(_nums, _target);
}
