using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CouplesHoldingHands;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CouplesHoldingHandsSolution's, the same methods
// CouplesHoldingHandsTests proves correct. The row is a random permutation of
// every seat so most couples start scattered instead of already paired.
[MemoryDiagnoser]
public class CouplesHoldingHandsBenchmarks
{
    private const int RandomSeed = 765; // LC problem number
    private const int SeatsPerCouple = 2;

    private int[] _row = [];

    [Params(200, 5_000)]
    public int CoupleCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var seatCount = CoupleCount * SeatsPerCouple;
        var row = new int[seatCount];

        for (var i = 0; i < seatCount; i++)
        {
            row[i] = i;
        }

        var random = new Random(RandomSeed);

        for (var i = seatCount - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (row[i], row[j]) = (row[j], row[i]);
        }

        _row = row;
    }

    [Benchmark(Baseline = true)]
    public int GreedySwapSimulation() => CouplesHoldingHandsSolution.MinSwapsByGreedySwap(_row);

    [Benchmark]
    public int DisjointSetComponentCounting() => CouplesHoldingHandsSolution.MinSwapsByDisjointSet(_row);
}
