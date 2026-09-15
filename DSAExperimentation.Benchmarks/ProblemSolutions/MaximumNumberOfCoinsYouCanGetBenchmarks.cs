using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfCoinsYouCanGet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfCoinsYouCanGetSolution's - the O(n^2)
// round-by-round simulation against this repo's own MergeSort plus the picking
// arithmetic. Pile counts are multiples of three, as the problem guarantees.
[MemoryDiagnoser]
public class MaximumNumberOfCoinsYouCanGetBenchmarks
{
    private const int RandomSeed = 1561; // LC problem number
    private const int MaxPileValueExclusive = 10_000;

    private int[] _piles = [];

    [Params(300, 3_000)]
    public int PileCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _piles = Enumerable.Range(0, PileCount).Select(_ => random.Next(1, MaxPileValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SimulateRoundsWithLinearScans() =>
        MaximumNumberOfCoinsYouCanGetSolution.MaxCoinsByRoundSimulation(_piles);

    [Benchmark]
    public int SortAscendingThenSumEveryOtherFromMiddle() =>
        MaximumNumberOfCoinsYouCanGetSolution.MaxCoinsByMergeSort(_piles);
}
