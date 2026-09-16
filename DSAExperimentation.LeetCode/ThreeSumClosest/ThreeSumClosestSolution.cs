using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ThreeSumClosest;

// LeetCode 16. 3Sum Closest: find the triplet sum nearest a target rather than one
// that equals it exactly, so the sweep tracks "closest so far" - though an exact
// hit still short-circuits, since nothing can beat a difference of zero.
//
// The two strategies differ in whether the array gets sorted first: a cubic scan
// over every triplet in original order, or MergeSort followed by a linear
// two-pointer sweep per fixed first element.
internal static class ThreeSumClosestSolution
{
    // Index of the third element when seeding `best` from the first three values.
    private const int ThirdElementIndex = 2;

    // Need at least 2 more elements after i for the two-pointer sweep.
    private const int RemainingPairSize = 2;

    // The textbook answer: three nested loops, no sorting, scanning every triplet.
    // Deliberately BCL-only - it is the arm the sorted two-pointer sweep below has
    // to justify itself against.
    public static int ClosestSumByBruteForce(int[] nums, int target)
    {
        var best = nums[0] + nums[1] + nums[ThirdElementIndex];

        for (var i = 0; i < nums.Length - RemainingPairSize; i++)
        {
            for (var j = i + 1; j < nums.Length - 1; j++)
            {
                best = ClosestSumForPair(nums, target, (i, j), best);
            }
        }

        return best;
    }

    private static int ClosestSumForPair(int[] nums, int target, (int I, int J) pair, int best)
    {
        for (var k = pair.J + 1; k < nums.Length; k++)
        {
            var sum = nums[pair.I] + nums[pair.J] + nums[k];

            if (Math.Abs(target - sum) < Math.Abs(target - best))
            {
                best = sum;
            }
        }

        return best;
    }

    // Sort with this repo's MergeSort, then fix the first element and sweep the
    // remaining range with two pointers, narrowing from whichever side under- or
    // overshoots the target.
    public static int ClosestSumByMergeSortTwoPointers(int[] nums, int target)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var best = sorted[0] + sorted[1] + sorted[ThirdElementIndex];

        for (var i = 0; i < sorted.Length - RemainingPairSize; i++)
        {
            var (updatedBest, foundExact) = ScanForClosest(sorted, target, i, best);
            best = updatedBest;

            if (foundExact)
            {
                return target;
            }
        }

        return best;
    }

    private static (int Best, bool FoundExact) ScanForClosest(int[] sorted, int target, int firstIndex, int best)
    {
        var left = firstIndex + 1;
        var right = sorted.Length - 1;

        while (left < right)
        {
            var sum = sorted[firstIndex] + sorted[left] + sorted[right];
            best = UpdateBest(target, best, sum);

            if (sum == target)
            {
                return (target, true);
            }

            (left, right) = AdvancePointers(target, sum, left, right);
        }

        return (best, false);
    }

    private static int UpdateBest(int target, int best, int sum)
        => IsNearerToTarget(target, sum, best) ? sum : best;

    // Whether this triplet's sum gets closer to the target than the best so far.
    private static bool IsNearerToTarget(int target, int sum, int best)
        => Math.Abs(target - sum) < Math.Abs(target - best);

    private static (int Left, int Right) AdvancePointers(int target, int sum, int left, int right)
        => sum < target ? WithLargerLeft(left, right) : WithSmallerRight(left, right);

    // A sum below the target can only be raised by taking a larger left value.
    private static (int Left, int Right) WithLargerLeft(int left, int right) => (left + 1, right);

    // A sum above the target can only be lowered by taking a smaller right value.
    private static (int Left, int Right) WithSmallerRight(int left, int right) => (left, right - 1);
}
