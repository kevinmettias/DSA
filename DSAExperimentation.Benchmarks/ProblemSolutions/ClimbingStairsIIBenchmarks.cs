using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ClimbingStairsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClimbingStairsIISolution's, the same methods
// ClimbingStairsIITests proves correct. StepCount is kept modest for BruteForceRecursion -
// its branching factor is up to 3 per step (jumps of 1, 2 or 3), so its cost grows
// like the Tribonacci constant (~1.84^n) rather than merely exponential-by-2.
[MemoryDiagnoser]
public class ClimbingStairsIIBenchmarks
{
    private const int RandomSeed = 3693; // LC problem number
    private const int CostUpperBound = 10_000; private int[] _costs = [];

    // exclusive upper bound; LC 3693 allows costs up to 1e4

    [Params(15, 20)]
    public int StepCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _costs = Enumerable.Range(0, StepCount).Select(_ => random.Next(1, CostUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForceRecursion() => ClimbingStairsIISolution.MinCostByBruteForce(StepCount, _costs);

    [Benchmark]
    public long MemoizedRecurrence() => ClimbingStairsIISolution.MinCostByMemoizedRecurrence(StepCount, _costs);
}
