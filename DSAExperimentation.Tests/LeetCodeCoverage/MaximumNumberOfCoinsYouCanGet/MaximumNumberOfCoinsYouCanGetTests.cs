using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfCoinsYouCanGet;

// LeetCode 1561. Maximum Number of Coins You Can Get: sort ascending, then the
// provably optimal picking order (Alice always takes the current max, you take the
// next-highest, Bob absorbs a low pile so it can never reach you) leaves you exactly
// indices n, n+2, ..., 3n-2 of the sorted array. Purely this repo's own
// Sorting.MergeSort over an ArrayIndexedSequence<int> - the same "problem *is*
// MergeSort plus arithmetic" shape SortAnArrayTests already proves - with a
// summation over the selected indices added on top.
public sealed partial class MaximumNumberOfCoinsYouCanGetTests
{
    [Fact]
    public void MaxCoins_ClassicSixPileExample_ReturnsNine()
    {
        int[] piles = [2, 4, 1, 2, 7, 8];

        Assert.Equal(9, MaxCoins(piles));
    }

    [Fact]
    public void MaxCoins_ThreePiles_ReturnsTheSecondLargest()
    {
        int[] piles = [2, 4, 5];

        Assert.Equal(4, MaxCoins(piles));
    }

    private static int MaxCoins(int[] piles)
    {
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(piles));

        var n = piles.Length / 3;
        var total = 0;
        for (var i = 0; i < n; i++)
        {
            total += piles[n + (2 * i)];
        }

        return total;
    }
}
