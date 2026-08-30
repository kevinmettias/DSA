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
    [Params(200, 4_000)]
    public int Length;

    private (int Alice, int Bob)[] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1686);
        _stones = Enumerable.Range(0, Length)
            .Select(_ => (Alice: random.Next(1, 1_000), Bob: random.Next(1, 1_000)))
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

    private static int ScoreDifference((int Alice, int Bob)[] stones)
    {
        var aliceScore = 0;
        var bobScore = 0;

        for (var i = 0; i < stones.Length; i++)
        {
            if (i % 2 == 0)
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
