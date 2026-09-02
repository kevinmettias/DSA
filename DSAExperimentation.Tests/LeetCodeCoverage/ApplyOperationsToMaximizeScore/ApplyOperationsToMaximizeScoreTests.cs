using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ApplyOperationsToMaximizeScore;

// LeetCode 2818. Apply Operations to Maximize Score: each element's "prime score"
// (its count of distinct prime factors, found by trial division) decides, via a
// monotonic-stack nearest-boundary scan over this repo's own Stack<T> - the same
// LIFO primitive AddTwoNumbersIITests uses for a different purpose - how many
// subarrays pick that index as their highest-prime-score, leftmost-on-tie element:
// (i - left[i]) * (right[i] - i), where left[i] is the nearest index to the left
// with score >= scores[i] and right[i] is the nearest index to the right with
// score > scores[i] (asymmetric on purpose, so a tie is only ever "owned" by its
// leftmost occurrence). Indices are then sorted by nums value descending via this
// repo's own MergeSort + ArrayIndexedSequence (the same composition ClosestRoomTests
// uses) and consumed greedily until k operations are spent, each raised via
// ModularArithmetic.Power (the same modulo-1e9+7 primitive
// CountTheNumberOfSquareFreeSubsetsSolution already uses) so the largest values are
// always exhausted first.
public sealed partial class ApplyOperationsToMaximizeScoreTests
{
    [Theory]
    [InlineData(new[] { 8, 3, 9, 3, 8 }, 2, 81L)]
    [InlineData(new[] { 19, 12, 14, 6, 10, 18 }, 3, 4788L)]
    public void MaximumScore_LeetCodeExamples_ReturnsProductModuloOfSelectedElements(
        int[] nums, int k, long expected)
    {
        var actual = MaximumScore(nums, k);

        Assert.Equal(expected, actual);
    }

    private static long MaximumScore(int[] nums, int k)
    {
        var scores = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            scores[i] = PrimeScore(nums[i]);
        }

        var left = ComputeLeftBoundaries(scores);
        var right = ComputeRightBoundaries(scores);
        var order = BuildValueDescendingOrder(nums);

        var result = 1L;
        var remaining = (long)k;

        foreach (var entry in order)
        {
            if (remaining <= 0)
            {
                break;
            }

            var i = entry.Index;
            var available = (long)(i - left[i]) * (right[i] - i);
            var uses = Math.Min(available, remaining);

            result = result * ModularArithmetic.Power(entry.Value, uses) % ModularArithmetic.Modulo;
            remaining -= uses;
        }

        return result;
    }

    private static int PrimeScore(int value)
    {
        var score = 0;

        for (var factor = 2; (long)factor * factor <= value; factor++)
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

        if (value > 1)
        {
            score++;
        }

        return score;
    }

    // Nearest index to the left with a score >= this one - keeps popping
    // strictly-lower scores off the stack, so a tie's leftmost occurrence is always
    // the one a later equal-score index sees as its boundary.
    private static int[] ComputeLeftBoundaries(int[] scores)
    {
        var left = new int[scores.Length];
        var stack = new NumberStack();

        for (var i = 0; i < scores.Length; i++)
        {
            while (stack.TryPeek(out var top) && scores[top] < scores[i])
            {
                stack.TryPop(out _);
            }

            left[i] = stack.TryPeek(out var boundary) ? boundary : -1;
            stack.Push(i);
        }

        return left;
    }

    // Nearest index to the right with a strictly greater score - pops on "<=" so an
    // equal-score neighbor never closes the range early; only a real improvement does.
    private static int[] ComputeRightBoundaries(int[] scores)
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

    private static IndexedValue[] BuildValueDescendingOrder(int[] nums)
    {
        var order = new IndexedValue[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            order[i] = new IndexedValue(nums[i], i);
        }

        MergeSort.Sort<IndexedValue, ArrayIndexedSequence<IndexedValue>>(
            new ArrayIndexedSequence<IndexedValue>(order),
            Comparer<IndexedValue>.Create((first, second) => second.Value.CompareTo(first.Value)));

        return order;
    }

    private readonly record struct IndexedValue(int Value, int Index);
}
