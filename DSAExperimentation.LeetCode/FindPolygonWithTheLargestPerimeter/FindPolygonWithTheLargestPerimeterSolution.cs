using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindPolygonWithTheLargestPerimeter;

// LeetCode 2971. Find Polygon With the Largest Perimeter: choose >= 3 side
// lengths from nums (any subset, not necessarily contiguous) whose longest
// side is smaller than the sum of the rest, maximizing total perimeter, or
// report -1 if no such choice exists.
internal static class FindPolygonWithTheLargestPerimeterSolution
{
    // Textbook baseline: try every subset of size >= 3 directly against the
    // problem's own definition (longest side < sum of the rest), tracking
    // the largest valid perimeter seen. Correct without any sorting insight
    // at all - O(2^n * n) - the same shape as
    // CountTheNumberOfGoodPartitionsSolution's brute-force arm, and the one
    // the composed strategy below has to justify itself against.
    public static long LargestPerimeterByBruteForceSubsets(int[] nums)
    {
        long best = LeetCodeAnswer.None;

        for (var mask = 1; mask < (1 << nums.Length); mask++)
        {
            if (TryPerimeter(nums, mask, out var perimeter) && perimeter > best)
            {
                best = perimeter;
            }
        }

        return best;
    }

    private static bool TryPerimeter(int[] nums, int mask, out long perimeter)
    {
        perimeter = 0;
        var count = 0;
        var longest = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            perimeter += nums[i];
            longest = Math.Max(longest, nums[i]);
            count++;
        }

        return count >= 3 && longest < perimeter - longest;
    }

    // Sort with this repo's own MergeSort over an ArrayIndexedSequence<int> -
    // the same composition SortAnArrayTests already proves out for LC 912 -
    // then a single greedy scan from the largest side down: dropping
    // sorted[i] only happens when it is at least half of what's left, so
    // every drop at least halves the remaining total and the scan itself is
    // O(log(sum nums)) steps regardless of n. O(n log n) sorting is what's
    // left to dominate either arm's running time.
    public static long LargestPerimeterBySortedRunningSum(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var total = 0L;

        foreach (var side in sorted)
        {
            total += side;
        }

        for (var i = sorted.Length - 1; i >= 2; i--)
        {
            var sumOfSmaller = total - sorted[i];

            if (sorted[i] < sumOfSmaller)
            {
                return total;
            }

            total = sumOfSmaller;
        }

        return LeetCodeAnswer.None;
    }
}
