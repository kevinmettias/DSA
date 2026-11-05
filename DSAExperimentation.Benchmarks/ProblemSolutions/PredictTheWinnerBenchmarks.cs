using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Solutions.PredictTheWinner;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is PredictTheWinnerSolution's, the same methods
// CatalogCaseTests proves correct. The array length stops at LC 486's
// own cap of 20, which is modest enough for the un-memoized baseline's real blowup,
// the same reasoning FibonacciNumberBenchmarks documents.
public class PredictTheWinnerBenchmarks
{
    private const int MaxScoreValueExclusive = 100;

    private int[] _nums = [];

    [Params(10, 20)]
    public int ArrayLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = SeededDraws.Values(ArrayLength, 1, MaxScoreValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public bool CanWinByUnmemoizedRecursion() => PredictTheWinnerSolution.CanWinByUnmemoizedRecursion(_nums);

    [Benchmark]
    public bool CanWinByMemoizedRecursion() => PredictTheWinnerSolution.CanWinByMemoizedRecursion(_nums);

    [Benchmark]
    public bool CanWinByNegamax() => PredictTheWinnerSolution.CanWinByNegamax(_nums);
}
