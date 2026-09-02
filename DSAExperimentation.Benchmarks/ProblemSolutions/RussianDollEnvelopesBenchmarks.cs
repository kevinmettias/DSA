using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Russian Doll Envelopes (LC 354): sort by width ascending / height descending on ties, then
// find the Longest Increasing Subsequence of heights. NaiveSortAndDp uses BCL Array.Sort
// followed by the textbook O(n^2) DP for LIS. SortThenPatienceSorting instead composes this
// repo's own MergeSort.Sort<Element,TSequence> for the O(n log n) sort and
// BinarySearch.LowerBound-driven patience sorting for the O(n log n) LIS step - the same pair
// of primitives RussianDollEnvelopesTests.cs exercises for correctness.
[MemoryDiagnoser]
public class RussianDollEnvelopesBenchmarks
{
    private const int RandomSeed = 11;

    [Params(200, 3_000)]
    public int Length;

    private (int Width, int Height)[] _envelopes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _envelopes = Enumerable.Range(0, Length)
            .Select(_ => (Width: random.Next(1, Length), Height: random.Next(1, Length)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveSortAndDp()
    {
        var items = ((int Width, int Height)[])_envelopes.Clone();
        Array.Sort(items, (a, b) => a.Width != b.Width ? a.Width.CompareTo(b.Width) : b.Height.CompareTo(a.Height));

        var dp = new int[items.Length];
        var best = 0;

        for (var i = 0; i < items.Length; i++)
        {
            dp[i] = 1;

            for (var j = 0; j < i; j++)
            {
                if (items[j].Height < items[i].Height && dp[j] + 1 > dp[i])
                {
                    dp[i] = dp[j] + 1;
                }
            }

            best = Math.Max(best, dp[i]);
        }

        return best;
    }

    [Benchmark]
    public int SortThenPatienceSorting()
    {
        var items = ((int Width, int Height)[])_envelopes.Clone();
        SortByWidthAscendingHeightDescending(items);

        var tails = BuildPatienceSortTails(items);

        return tails.Count;
    }

    private static void SortByWidthAscendingHeightDescending((int Width, int Height)[] items)
    {
        var byWidthThenHeightDescending = Comparer<(int Width, int Height)>.Create(
            (a, b) => a.Width != b.Width ? a.Width.CompareTo(b.Width) : b.Height.CompareTo(a.Height));

        MergeSort.Sort<(int Width, int Height), ArrayIndexedSequence<(int Width, int Height)>>(
            new ArrayIndexedSequence<(int Width, int Height)>(items), byWidthThenHeightDescending);
    }

    private static DynamicArray<int> BuildPatienceSortTails((int Width, int Height)[] items)
    {
        var tails = new DynamicArray<int>();

        foreach (var (_, height) in items)
        {
            var position = BinarySearch.LowerBound(new DynamicArraySequence<int>(tails), height);

            if (position == tails.Count)
            {
                tails.Add(height);
            }
            else
            {
                tails.Set(position, height);
            }
        }

        return tails;
    }
}
