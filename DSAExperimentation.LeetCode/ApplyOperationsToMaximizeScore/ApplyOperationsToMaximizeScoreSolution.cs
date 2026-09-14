using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ApplyOperationsToMaximizeScore;

// LeetCode 2818. Apply Operations to Maximize Score: each operation picks a subarray
// not picked before and multiplies the running score by that subarray's element of
// highest prime score - the count of its distinct prime factors - taking the leftmost
// such element on a tie. With k operations to spend, report the largest score
// obtainable, modulo 1e9+7.
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

    // No index to the left carries a score this one does not beat, so the range
    // reaches the start of the array - the same "one before the first index" sentinel
    // a monotonic boundary scan always needs (CountBowlSubarrays' NoGreaterElement).
    private const int NoBlockingScoreToTheLeft = -1;

    // The textbook answer: walk outward from every index until the score condition
    // breaks, then order by value with a BCL Array.Sort. O(n^2) whenever scores run
    // long ties, which is the common case since a prime score only ever ranges over a
    // handful of small integers. Nothing from this repo's own structures - it is the
    // arm the monotonic-stack strategy has to justify itself against, and stating it
    // here is what finally gets it asserted.
    public static long MaximumScoreByLinearBoundaryScan(int[] nums, int k)
    {
        var scores = PrimeScores(nums);
        var (left, right) = BoundariesByOutwardScan(scores);

        return SpendOperations(ValueDescendingByArraySort(nums), left, right, k);
    }

    // Two passes over this repo's own Stack<int> - the same LIFO primitive
    // AddTwoNumbersII uses for a different purpose - each maintaining a monotonic
    // stack of indices, so every index is pushed and popped at most once per pass and
    // the boundaries cost O(n) in total rather than O(n^2). The value-descending order
    // comes from MergeSort over an ArrayIndexedSequence, the same composition
    // ClosestRoom already uses.
    public static long MaximumScoreByStackBoundaryScan(int[] nums, int k)
    {
        var scores = PrimeScores(nums);
        var left = LeftBoundariesByMonotonicStack(scores);
        var right = RightBoundariesByMonotonicStack(scores);

        return SpendOperations(ValueDescendingByMergeSort(nums), left, right, k);
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

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        // Whatever survives the loop is a prime larger than the square root.
        if (value > 1)
        {
            score++;
        }

        return score;
    }

    private static (int[] Left, int[] Right) BoundariesByOutwardScan(int[] scores)
    {
        var left = new int[scores.Length];
        var right = new int[scores.Length];

        for (var i = 0; i < scores.Length; i++)
        {
            var earlier = i - 1;

            while (earlier >= 0 && scores[earlier] < scores[i])
            {
                earlier--;
            }

            left[i] = earlier;

            var later = i + 1;

            while (later < scores.Length && scores[later] <= scores[i])
            {
                later++;
            }

            right[i] = later;
        }

        return (left, right);
    }

    // Nearest index to the left with a score >= this one - keeps popping
    // strictly-lower scores off the stack, so a tie's leftmost occurrence is always
    // the one a later equal-score index sees as its boundary.
    private static int[] LeftBoundariesByMonotonicStack(int[] scores)
    {
        var left = new int[scores.Length];
        var stack = new NumberStack();

        for (var i = 0; i < scores.Length; i++)
        {
            while (stack.TryPeek(out var top) && scores[top] < scores[i])
            {
                stack.TryPop(out _);
            }

            left[i] = stack.TryPeek(out var boundary) ? boundary : NoBlockingScoreToTheLeft;
            stack.Push(i);
        }

        return left;
    }

    // Nearest index to the right with a strictly greater score - pops on "<=" so an
    // equal-score neighbour never closes the range early; only a real improvement does.
    private static int[] RightBoundariesByMonotonicStack(int[] scores)
    {
        var right = new int[scores.Length];
        var stack = new NumberStack();

        for (var i = scores.Length - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && scores[top] <= scores[i])
            {
                stack.TryPop(out _);
            }

            right[i] = stack.TryPeek(out var boundary) ? boundary : scores.Length;
            stack.Push(i);
        }

        return right;
    }

    private static IndexedValue[] ValueDescendingByArraySort(int[] nums)
    {
        var order = IndexedValues(nums);
        Array.Sort(order, (first, second) => second.Value.CompareTo(first.Value));

        return order;
    }

    private static IndexedValue[] ValueDescendingByMergeSort(int[] nums)
    {
        var order = IndexedValues(nums);

        MergeSort.Sort<IndexedValue, ArrayIndexedSequence<IndexedValue>>(
            new ArrayIndexedSequence<IndexedValue>(order),
            Comparer<IndexedValue>.Create((first, second) => second.Value.CompareTo(first.Value)));

        return order;
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
    private static long SpendOperations(IndexedValue[] valueDescending, int[] left, int[] right, int k)
    {
        var score = 1L;
        var remaining = (long)k;

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
