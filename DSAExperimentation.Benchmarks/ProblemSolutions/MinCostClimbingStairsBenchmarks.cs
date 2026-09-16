using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinCostClimbingStairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are MinCostClimbingStairsSolution's, the same
// methods MinCostClimbingStairsTests proves correct. NaiveRecursive is kept to a
// modest StepCount since its cost blowup (O(2^n)) is real.
[MemoryDiagnoser]
public class MinCostClimbingStairsBenchmarks
{
    private const int RandomSeed = 746; // LC problem number
    private const int CostUpperBound = 100; private int[] _cost = [];

    // exclusive upper bound for generated per-step cost

    [Params(20, 30)]
    public int StepCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _cost = Enumerable.Range(0, StepCount).Select(_ => random.Next(1, CostUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursive() => MinCostClimbingStairsSolution.MinCostByNaiveRecursive(_cost);

    [Benchmark]
    public int MemoizedRecurrence() => MinCostClimbingStairsSolution.MinCostByMemoizedRecurrence(_cost);

    [Benchmark]
    public int IterativeConstantSpace() => MinCostClimbingStairsSolution.MinCostByIterativeConstantSpace(_cost);
}
