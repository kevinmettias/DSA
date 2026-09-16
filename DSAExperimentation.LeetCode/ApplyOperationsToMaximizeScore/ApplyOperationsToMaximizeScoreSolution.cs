using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.ApplyOperationsToMaximizeScore;

// LeetCode 2818. Apply Operations to Maximize Score: each operation picks a subarray
// not picked before and multiplies the running score by that subarray's element of
// highest prime score - the count of its distinct prime factors - taking the leftmost
// such element on a tie. With operationCount operations to spend, report the largest
// score obtainable, modulo 1e9+7.
//
// Since the picked element is a function of the subarray alone, the whole problem is:
// how many subarrays would pick index i? That count is (i - left[i]) * (right[i] - i),
// where left[i] is the nearest index to the left whose prime score is >= this one's
// and right[i] the nearest to the right whose prime score is strictly greater. The
// asymmetry is what makes a tie belong to its leftmost occurrence exactly once. Sort
// the indices by VALUE descending, spend operations on the largest values first, and
// the greedy is optimal because every operation is worth the value it multiplies in.
//
// Both strategies compute the same prime scores and run the same greedy; they differ
// only in how each index finds its two boundaries, which is the whole performance
// question here.
internal static class ApplyOperationsToMaximizeScoreSolution
{
    private const int SmallestPrime = 2;

    // Where a subarray's count comes from, both sentinels are one step outside the array
    // so the two factors need no special case at either end: (i - left[i]) counts the
    // leftward starts with left[i] == -1, and (right[i] - i) counts the rightward ends with
    // right[i] == scores.Length.
    private const int NoBlockingScoreToTheLeft = -1;

    // The textbook answer: walk outward from every index until the score condition
    // breaks, then order by value with a BCL Array.Sort. O(n^2) whenever scores run
    // long ties, which is the common case since a prime score only ever ranges over a
    // handful of small integers. Nothing from this repo's own structures - it is the
    // arm the monotonic-stack strategy has to justify itself against, and stating it
    // here is what finally gets it asserted.
    public static long MaximumScoreByLinearBoundaryScan(int[] nums, int operationCount)
    {
        var scores = PrimeScores(nums);
        var (left, right) = BoundariesByOutwardScan(scores);

        return SpendOperations(ValueDescendingByArraySort(nums), left, right, operationCount);
    }

    private static (int[] Left, int[] Right) BoundariesByOutwardScan(int[] scores)
    {
        var left = new int[scores.Length];
        var right = new int[scores.Length];

        for (var i = 0; i < scores.Length; i++)
        {
            left[i] = LeftBoundaryByOutwardScan(scores, i);
            right[i] = RightBoundaryByOutwardScan(scores, i);
        }

        return (left, right);
    }

    // Nearest index to the left whose score this one does not beat, found by walking
    // outward until the strict "lower than mine" condition breaks.
    private static int LeftBoundaryByOutwardScan(int[] scores, int index)
    {
        var earlier = index - 1;

        while (earlier >= 0 && scores[earlier] < scores[index])
        {
            earlier--;
        }

        return earlier;
    }

    // Nearest index to the right whose score is strictly greater, found by walking
    // outward until that condition breaks.
    private static int RightBoundaryByOutwardScan(int[] scores, int index)
    {
        var later = index + 1;

        while (later < scores.Length && scores[later] <= scores[index])
        {
            later++;
        }

        return later;
    }

    private static IndexedValue[] ValueDescendingByArraySort(int[] nums)
    {
        var order = IndexedValues(nums);
        Array.Sort(order, (first, second) => second.Value.CompareTo(first.Value));

        return order;
    }

    // Both boundaries come from NearestBoundary's sweep - the same relation pairing this
    // class's count depends on: the left relation takes a score equal to this one, the right
    // relation refuses it, which is exactly what makes a tie belong to its leftmost
    // occurrence once. The value-descending order comes from MergeSort over an
    // ArrayIndexedSequence, the same composition ClosestRoom already uses.
    public static long MaximumScoreByStackBoundaryScan(int[] nums, int operationCount)
    {
        var scores = PrimeScores(nums);
        var left = NearestBoundary.GreaterOrEqualToTheLeft(scores, NoBlockingScoreToTheLeft);
        var right = NearestBoundary.GreaterToTheRight(scores, scores.Length);

        return SpendOperations(ValueDescendingByMergeSort(nums), left, right, operationCount);
    }

    private static IndexedValue[] ValueDescendingByMergeSort(int[] nums)
    {
        var order = IndexedValues(nums);

        MergeSort.Sort<IndexedValue, ArrayIndexedSequence<IndexedValue>>(
            new ArrayIndexedSequence<IndexedValue>(order),
            Comparer<IndexedValue>.Create((first, second) => second.Value.CompareTo(first.Value)));

        return order;
    }

    // Trial division, shared by both arms because it is plain BCL arithmetic either
    // way - the same shape CountTheNumberOfSquareFreeSubsets' own prime mask uses, not
    // a production primitive this repo offers.
    private static int[] PrimeScores(int[] nums)
    {
        var scores = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            scores[i] = DistinctPrimeFactorCount(nums[i]);
        }

        return scores;
    }

    private static int DistinctPrimeFactorCount(int value)
    {
        var score = 0;

        for (var factor = SmallestPrime; (long)factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            score++;

            value = DivideOutFactor(value, factor);
        }

        // Whatever survives the loop is a prime larger than the square root.
        if (value > 1)
        {
            score++;
        }

        return score;
    }

    // Divides every copy of `factor` out of `value`, leaving only the part `factor`
    // no longer divides.
    private static int DivideOutFactor(int value, int factor)
    {
        while (value % factor == 0)
        {
            value /= factor;
        }

        return value;
    }

    private static IndexedValue[] IndexedValues(int[] nums)
    {
        var order = new IndexedValue[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            order[i] = new IndexedValue(nums[i], i);
        }

        return order;
    }

    // Spends the operations on the largest values first, each index able to absorb as
    // many operations as there are subarrays that would pick it. The two arms reach
    // this with differently sorted arrays - MergeSort is stable, Array.Sort is not -
    // and that cannot change the answer: equal values contribute v^(total spent on the
    // tied group), and the total a tied group absorbs is min(its summed capacity, the
    // operations left when the group is reached) whatever order its members are taken
    // in.
    //
    // ModularArithmetic is LeetCode's own "report it modulo 1e9+7" convention rather
    // than a primitive either arm competes on (ARCHITECTURE.md 17.6), so both share it.
    private static long SpendOperations(IndexedValue[] valueDescending, int[] left, int[] right, int operationCount)
    {
        var score = 1L;
        var remaining = (long)operationCount;

        foreach (var entry in valueDescending)
        {
            if (remaining <= 0)
            {
                break;
            }

            var index = entry.Index;
            var subarraysPickingIndex = (long)(index - left[index]) * (right[index] - index);
            var operations = Math.Min(subarraysPickingIndex, remaining);

            score = score * ModularArithmetic.Power(entry.Value, operations) % ModularArithmetic.Modulo;
            remaining -= operations;
        }

        return score;
    }

    private readonly record struct IndexedValue(int Value, int Index);
}
