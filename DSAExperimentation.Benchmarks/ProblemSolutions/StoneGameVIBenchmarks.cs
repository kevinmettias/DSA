using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game VI (LC 1686): both strategies run the identical O(n log n) greedy -
// sort stones by (aliceValue + bobValue) descending, alternate picks starting with
// Alice - the same "same algorithm, different sort primitive" pairing
// TwoCitySchedulingBenchmarks already uses. ArraySortGreedy uses BCL Array.Sort;
// MergeSortGreedy instead composes this repo's own MergeSort.Sort<Element,TSequence>
// over an ArrayIndexedSequence.
[MemoryDiagnoser]
public class StoneGameVIBenchmarks
{
    private const int RandomSeed = 1686; // LC problem number
    private const int MaxStoneValue = 1_000; // exclusive upper bound passed to Random.Next

    [Params(200, 4_000)]
    public int Length;

    private (int Alice, int Bob)[] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stones = Enumerable.Range(0, Length)
            .Select(_ => (Alice: random.Next(1, MaxStoneValue), Bob: random.Next(1, MaxStoneValue)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArraySortGreedy()
    {
        var stones = ((int Alice, int Bob)[])_stones.Clone();
        Array.Sort(stones, (x, y) => (y.Alice + y.Bob).CompareTo(x.Alice + x.Bob));

        return ScoreDifference(stones);
    }

    [Benchmark]
    public int MergeSortGreedy()
    {
        var stones = ((int Alice, int Bob)[])_stones.Clone();
        var bySwingDescending = Comparer<(int Alice, int Bob)>.Create(
            (x, y) => (y.Alice + y.Bob).CompareTo(x.Alice + x.Bob));

        MergeSort.Sort<(int Alice, int Bob), ArrayIndexedSequence<(int Alice, int Bob)>>(
            new ArrayIndexedSequence<(int Alice, int Bob)>(stones), bySwingDescending);

        return ScoreDifference(stones);
    }

    private const int TurnParityDivisor = 2; // even index -> Alice's turn, odd index -> Bob's turn

    private static int ScoreDifference((int Alice, int Bob)[] stones)
    {
        var aliceScore = 0;
        var bobScore = 0;

        for (var i = 0; i < stones.Length; i++)
        {
            if (i % TurnParityDivisor == 0)
            {
                aliceScore += stones[i].Alice;
            }
            else
            {
                bobScore += stones[i].Bob;
            }
        }

        return aliceScore - bobScore;
    }
}
