using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RussianDollEnvelopes;

// LeetCode 354. Russian Doll Envelopes: sort by width ascending, height descending on ties
// (so envelopes sharing a width can never chain into each other), then the answer is the
// Longest Increasing Subsequence of the resulting heights.
//
// Both strategies share that same two-step shape and differ only in what each step is built
// from - MaxEnvelopesBySortThenPatience composes this repo's own MergeSort.Sort over an
// ArrayIndexedSequence for the O(n log n) sort and BinarySearch.LowerBound-driven patience
// sorting for the O(n log n) LIS step (the exact approach LongestIncreasingSubsequenceTests.cs
// already exercises, reused here over a DynamicArraySequence<int> "tails" buffer);
// MaxEnvelopesByBruteForceDp is the textbook BCL Array.Sort + O(n^2) DP baseline it has to
// justify itself against.
internal static class RussianDollEnvelopesSolution
{
    public static int MaxEnvelopesBySortThenPatience(int[][] envelopes) =>
        MaxEnvelopesBySortThenPatience(BuildItems(envelopes));

    public static int MaxEnvelopesBySortThenPatience((int Width, int Height)[] envelopes)
    {
        var items = ((int Width, int Height)[])envelopes.Clone();
        SortByWidthAscendingHeightDescending(items);

        return ComputeLongestIncreasingHeightRun(items);
    }

    public static int MaxEnvelopesByBruteForceDp(int[][] envelopes) =>
        MaxEnvelopesByBruteForceDp(BuildItems(envelopes));

    public static int MaxEnvelopesByBruteForceDp((int Width, int Height)[] envelopes)
    {
        var items = ((int Width, int Height)[])envelopes.Clone();
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

    private static (int Width, int Height)[] BuildItems(int[][] envelopes)
        => envelopes.Select(envelope => (Width: envelope[0], Height: envelope[1])).ToArray();

    // Width ascending, height descending on ties - so envelopes sharing a width can
    // never chain into each other.
    private static void SortByWidthAscendingHeightDescending((int Width, int Height)[] items)
    {
        var byWidthThenHeightDescending = Comparer<(int Width, int Height)>.Create(
            (a, b) => a.Width != b.Width ? a.Width.CompareTo(b.Width) : b.Height.CompareTo(a.Height));

        MergeSort.Sort<(int Width, int Height), ArrayIndexedSequence<(int Width, int Height)>>(
            new ArrayIndexedSequence<(int Width, int Height)>(items), byWidthThenHeightDescending);
    }

    // Patience-sorting LIS over the heights, once width ties can no longer chain.
    private static int ComputeLongestIncreasingHeightRun((int Width, int Height)[] items)
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

        return tails.Count;
    }
}
