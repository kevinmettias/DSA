using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGameVI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameVISolution's, the same methods
// StoneGameVITests proves correct. Both run the identical O(n log n) greedy and
// differ only in the sort primitive - the BCL's Array.Sort against this repo's own
// MergeSort over an ArrayIndexedSequence - so the seeded value arrays are built
// once in [GlobalSetup] and only the sort-and-tally is measured.
[MemoryDiagnoser]
public class StoneGameVIBenchmarks
{
    private const int RandomSeed = 1686; // LC problem number
    private const int MaxStoneValue = 1_000; // exclusive upper bound passed to Random.Next

    [Params(200, 4_000)]
    public int Length;

    private int[] _aliceValues = null!;
    private int[] _bobValues = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _aliceValues = new int[Length];
        _bobValues = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _aliceValues[i] = random.Next(1, MaxStoneValue);
            _bobValues[i] = random.Next(1, MaxStoneValue);
        }
    }

    [Benchmark(Baseline = true)]
    public int ArraySortGreedy() => StoneGameVISolution.WinnerByArraySortGreedy(_aliceValues, _bobValues);

    [Benchmark]
    public int MergeSortGreedy() => StoneGameVISolution.WinnerByMergeSortGreedy(_aliceValues, _bobValues);
}
