using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ClimbingStairsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClimbingStairsIISolution's, the same methods
// ClimbingStairsIISolutionTests proves correct.
//
// Sizes are per arm. BruteForceRecursion's branching factor is up to 3 per step
// (jumps of 1, 2 or 3), so its cost grows like the Tribonacci constant (~1.84^n) and
// it stops at 20 steps; the memoized recurrence is linear and runs on to 1,000 steps.
// LC 3693 allows 10^5, but the memoized recursion is as deep as the staircase is tall,
// so 1,000 keeps its call stack shallow. The two are compared at the sizes both run.
public class ClimbingStairsIIBenchmarks
{
    private const int RandomSeed = 3693; // LC problem number

    // Exclusive upper bound; LC 3693 allows costs up to 1e4.
    private const int CostUpperBound = 10_000;

    private Dictionary<int, int[]> _costsByStepCount = [];

    public static IEnumerable<int> BruteForceSizes => [15, 20];

    public static IEnumerable<int> MemoizedSizes => [.. BruteForceSizes, 100, 1_000];

    // Every step count any arm runs is drawn here, outside the timed region, each from its
    // own generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _costsByStepCount = MemoizedSizes.ToDictionary(
            stepCount => stepCount,
            stepCount => SeededDraws.Values(stepCount, 1, CostUpperBound, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForceRecursion(int stepCount) =>
        ClimbingStairsIISolution.MinCostByBruteForce(stepCount, _costsByStepCount[stepCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public long MemoizedRecurrence(int stepCount) =>
        ClimbingStairsIISolution.MinCostByMemoizedRecurrence(stepCount, _costsByStepCount[stepCount]);
}
