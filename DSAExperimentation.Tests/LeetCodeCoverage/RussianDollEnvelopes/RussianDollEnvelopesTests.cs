using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RussianDollEnvelopes;

// LeetCode 354. Russian Doll Envelopes: sort by width ascending, height descending on ties
// (so envelopes sharing a width can never chain into each other) via this repo's own
// MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence, then the answer is the
// Longest Increasing Subsequence of the resulting heights - the exact patience-sorting-via-
// BinarySearch.LowerBound approach LongestIncreasingSubsequenceTests.cs already exercises,
// reused here unchanged over a DynamicArraySequence<int> "tails" buffer.
public sealed partial class RussianDollEnvelopesTests
{
    [Fact]
    public void MaxEnvelopes_LeetCodeExample_ReturnsLongestChain()
    {
        int[][] envelopes = [[5, 4], [6, 4], [6, 7], [2, 3]];

        Assert.Equal(3, MaxEnvelopes(envelopes));
    }

    [Fact]
    public void MaxEnvelopes_AllSameSize_CannotDollAnyIntoAnother()
    {
        int[][] envelopes = [[1, 1], [1, 1], [1, 1]];

        Assert.Equal(1, MaxEnvelopes(envelopes));
    }

    private static int MaxEnvelopes(int[][] envelopes)
    {
        var items = envelopes.Select(envelope => (Width: envelope[0], Height: envelope[1])).ToArray();
        var byWidthThenHeightDescending = Comparer<(int Width, int Height)>.Create(
            (a, b) => a.Width != b.Width ? a.Width.CompareTo(b.Width) : b.Height.CompareTo(a.Height));

        MergeSort.Sort<(int Width, int Height), ArrayIndexedSequence<(int Width, int Height)>>(
            new ArrayIndexedSequence<(int Width, int Height)>(items), byWidthThenHeightDescending);

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
