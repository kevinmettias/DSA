using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ThreeSum;

// LeetCode 15. 3Sum: every unique triplet of values that sums to zero.
//
// The two strategies differ in how they avoid the cubic brute force - a
// duplicate-filtering set keyed on the sorted triplet, versus this repo's own
// MergeSort followed by the usual sorted two-pointer sweep per fixed first
// element, which never revisits a triplet in the first place.
internal static class ThreeSumSolution
{
    // Need at least 2 more elements after the first index for the two-pointer sweep.
    private const int RemainingPairSize = 2;

    // Index of the third element in a 3-element triplet.
    private const int ThirdElementIndex = 2;

    // The textbook baseline: check every triplet and de-duplicate by sorting
    // each match into a HashSet.
    public static List<(int First, int Second, int Third)> FindTripletsByBruteForce(int[] nums)
    {
        var found = new HashSet<(int First, int Second, int Third)>();

        for (var i = 0; i < nums.Length - RemainingPairSize; i++)
        {
            for (var j = i + 1; j < nums.Length - 1; j++)
            {
                CollectZeroSumTriplets(nums, i, j, found);
            }
        }

        return found.ToList();
    }

    private static void CollectZeroSumTriplets(
        int[] nums, int firstIndex, int secondIndex, HashSet<(int First, int Second, int Third)> found)
    {
        for (var k = secondIndex + 1; k < nums.Length; k++)
        {
            if (nums[firstIndex] + nums[secondIndex] + nums[k] == 0)
            {
                int[] triplet = [nums[firstIndex], nums[secondIndex], nums[k]];
                Array.Sort(triplet);
                found.Add((triplet[0], triplet[1], triplet[ThirdElementIndex]));
            }
        }
    }

    // Sort with this repo's MergeSort over ArrayIndexedSequence, then sweep a
    // two-pointer window per fixed first element, skipping duplicates on all
    // three positions so no triplet is ever reported twice.
    public static List<(int First, int Second, int Third)> FindTripletsByMergeSortTwoPointers(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var results = new List<(int First, int Second, int Third)>();

        for (var i = 0; i < sorted.Length - RemainingPairSize; i++)
        {
            SweepForFixedFirst(results, sorted, i);
        }

        return results;
    }

    // The work for one fixed first element `sorted[firstIndex]`: skip a duplicate
    // first element, then sweep the usual two-pointer window over the remainder.
    private static void SweepForFixedFirst(
        List<(int First, int Second, int Third)> results, int[] sorted, int firstIndex)
    {
        if (firstIndex > 0 && sorted[firstIndex] == sorted[firstIndex - 1])
        {
            return;
        }

        var left = firstIndex + 1;
        var right = sorted.Length - 1;

        while (left < right)
        {
            var sum = sorted[firstIndex] + sorted[left] + sorted[right];

            if (sum == 0)
            {
                (left, right) = RecordTripletAndSkipDuplicates(results, sorted, firstIndex, (left, right));
            }
            else if (sum < 0)
            {
                left++;
            }
            else
            {
                right--;
            }
        }
    }

    // Records the zero-sum triplet at (firstIndex, left, right) then advances past
    // any duplicate values on both sides, so the caller's two-pointer sweep never
    // reports the same triplet twice.
    private static (int Left, int Right) RecordTripletAndSkipDuplicates(
        List<(int First, int Second, int Third)> results,
        int[] sorted,
        int firstIndex,
        (int Left, int Right) window)
    {
        var (left, right) = window;
        results.Add((sorted[firstIndex], sorted[left], sorted[right]));
        left++;
        right--;

        while (left < right && sorted[left] == sorted[left - 1])
        {
            left++;
        }

        while (left < right && sorted[right] == sorted[right + 1])
        {
            right--;
        }

        return (left, right);
    }
}
