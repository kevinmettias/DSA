using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PredictTheWinner;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PredictTheWinnerSolution's, the same methods
// PredictTheWinnerTests proves correct. N is kept modest specifically because the
// un-memoized baseline's blowup is real, the same reasoning FibonacciBenchmarks
// documents.
[MemoryDiagnoser]
public class PredictTheWinnerBenchmarks
{
    private const int MaxScoreValueExclusive = 100;

    private int[] _nums = [];

    [Params(22, 26)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, N).Select(_ => random.Next(1, MaxScoreValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursion() => PredictTheWinnerSolution.CanWinByUnmemoizedRecursion(_nums);

    [Benchmark]
    public bool MemoizedRecursion() => PredictTheWinnerSolution.CanWinByMemoizedRecursion(_nums);
}
