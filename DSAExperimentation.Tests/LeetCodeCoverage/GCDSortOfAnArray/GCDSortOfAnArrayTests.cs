using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GCDSortOfAnArray;

// LeetCode 1998. GCD Sort of an Array: DisjointSet over VALUES (not indices) up to
// max(nums), unioned with each of their own prime factors via the same trial-division
// PrimeFactors idiom LargestComponentSizeByCommonFactorTests uses - just unioning
// value<->factor directly instead of index<->owner, since here it's the values
// themselves (not their positions) that need a shared component. Swapping gcd(a,b) > 1
// pairs any number of times can reach any permutation within a connected component (any
// two values sharing a chain of common prime factors), so the array can be sorted via
// allowed swaps iff every position's original value and this repo's own MergeSort-sorted
// value already land in the same component.
public sealed partial class GCDSortOfAnArrayTests
{
    [Fact]
    public void CanBeSortedByGcdSwaps_AllValuesChainThroughSharedFactors_ReturnsTrue()
    {
        int[] nums = [7, 21, 3];

        Assert.True(CanBeSortedByGcdSwaps(nums));
    }

    [Fact]
    public void CanBeSortedByGcdSwaps_FiveIsolatedFromEvenComponent_ReturnsFalse()
    {
        int[] nums = [5, 2, 6, 2];

        Assert.False(CanBeSortedByGcdSwaps(nums));
    }

    [Fact]
    public void CanBeSortedByGcdSwaps_OneLargeComponentAcrossAllValues_ReturnsTrue()
    {
        int[] nums = [10, 5, 9, 3, 15];

        Assert.True(CanBeSortedByGcdSwaps(nums));
    }

    private static bool CanBeSortedByGcdSwaps(int[] nums)
    {
        var components = BuildFactorComponents(nums);
        var sorted = SortedCopy(nums);

        return IsSortedByGcdSwaps(nums, sorted, components);
    }

    private static DisjointSet BuildFactorComponents(int[] nums)
    {
        var maxValue = nums.Max();
        var components = new DisjointSet(maxValue + 1);

        foreach (var value in nums)
        {
            foreach (var factor in PrimeFactors(value))
            {
                components.Union(value, factor);
            }
        }

        return components;
    }

    private static int[] SortedCopy(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));
        return sorted;
    }

    private static bool IsSortedByGcdSwaps(int[] nums, int[] sorted, DisjointSet components)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            if (!components.IsConnected(nums[i], sorted[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = 2; factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }
}
