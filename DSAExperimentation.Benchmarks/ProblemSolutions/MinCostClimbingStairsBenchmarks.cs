using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Min Cost Climbing Stairs (LC 746): the same three-strategy shape as
// FibonacciBenchmarks. NaiveRecursive re-derives minCost(i) = cost[i] +
// min(minCost(i+1), minCost(i+2)) from both starting steps with no cache - O(2^n),
// N kept modest since that blowup is real. TopDownMemoized dogfoods this repo's own
// Memoizer over the exact recurrence MinCostClimbingStairsTests uses, with a virtual
// start state (-1) folding "start at 0 or 1" into one shared cache instead of two
// top-level Memoize calls. IterativeConstantSpace is the O(n)-time O(1)-space answer
// neither of the other two even needs to beat.
[MemoryDiagnoser]
public class MinCostClimbingStairsBenchmarks
{
    private const int RandomSeed = 746; // LC problem number
    private const int CostUpperBound = 100; // exclusive upper bound for generated per-step cost
    private const int TwoStepClimb = 2; // LC 746 allows climbing 1 or 2 steps at a time

    [Params(20, 30)]
    public int N;

    private int[] _cost = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _cost = Enumerable.Range(0, N).Select(_ => random.Next(1, CostUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursive() => Math.Min(CostFrom(0), CostFrom(1));

    private int CostFrom(int step)
        => step >= _cost.Length ? 0 : _cost[step] + Math.Min(CostFrom(step + 1), CostFrom(step + TwoStepClimb));

    [Benchmark]
    public int TopDownMemoized()
        => Memoizer.Memoize<int, int>(-1, (step, minCostFrom) => MinCostFromStep(step, minCostFrom));

    private int MinCostFromStep(int step, Func<int, int> minCostFrom)
    {
        if (step < 0)
        {
            return Math.Min(minCostFrom(0), minCostFrom(1));
        }

        if (step >= _cost.Length)
        {
            return 0;
        }

        return _cost[step] + Math.Min(minCostFrom(step + 1), minCostFrom(step + TwoStepClimb));
    }

    [Benchmark]
    public int IterativeConstantSpace()
    {
        var (twoBack, oneBack) = (0, 0);

        for (var i = TwoStepClimb; i <= _cost.Length; i++)
        {
            var current = Math.Min(oneBack + _cost[i - 1], twoBack + _cost[i - TwoStepClimb]);
            (twoBack, oneBack) = (oneBack, current);
        }

        return oneBack;
    }
}
