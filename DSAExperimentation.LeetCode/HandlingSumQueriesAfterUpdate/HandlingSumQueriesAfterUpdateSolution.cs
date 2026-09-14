using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.LeetCode.HandlingSumQueriesAfterUpdate;

// LeetCode 2569. Handling Sum Queries After Update: three query kinds over a pair of
// equal-length arrays - type 1 range-flips nums1's bits, type 2 adds p times the
// CURRENT count of ones in nums1 to a running total of nums2, and type 3 reports that
// total. The answer is the list of type-3 readings, in order.
//
// nums2 itself never has to be materialized after the first pass: a type-2 query adds
// the same p to every position that holds a one, so the only thing it changes about
// the total is `p * (ones in nums1)`. That reduces the whole problem to maintaining
// one number - the count of ones in nums1 under range flips - which is exactly what
// this repo's LazySegmentTree<Element, TUpdate, TOperation> does once handed
// FlipCountOperation, the third IRangeUpdateOperation witness (beside
// RangeAddSumOperation and RangeAssignMaxOperation) and the same "compose the generic
// lazy engine over a new algebra" move FancySequence's AffineOperation makes.
//
// Both strategies answer LeetCode's real shape - the long[] of type-3 readings - so
// the test harness can assert them against the same examples and the benchmark
// harness can time them against each other.
internal static class HandlingSumQueriesAfterUpdateSolution
{
    private const int FlipQuery = 1;
    private const int AddScaledOnesQuery = 2;

    // The textbook answer: keep nums1 as a mutable bit array, walk the flipped range
    // one index at a time, and re-scan the whole array for its count of ones on every
    // type-2 query. O(n) per query either way - correct, and deliberately written with
    // nothing but BCL pieces, which is what makes it the arm the segment-tree strategy
    // below has to justify itself against.
    public static long[] HandleQueryByArrayRescan(int[] nums1, int[] nums2, int[][] queries)
    {
        var bits = (int[])nums1.Clone();
        var total = nums2.Sum(value => (long)value);
        var answers = new List<long>();

        foreach (var query in queries)
        {
            switch (query[0])
            {
                case FlipQuery:
                    FlipRange(bits, query[1], query[2]);
                    break;
                case AddScaledOnesQuery:
                    total += (long)query[1] * bits.Sum();
                    break;
                default:
                    answers.Add(total);
                    break;
            }
        }

        return answers.ToArray();
    }

    private static void FlipRange(int[] bits, int left, int right)
    {
        for (var i = left; i <= right; i++)
        {
            bits[i] ^= 1;
        }
    }

    // One LazySegmentTree over nums1 turns both halves of the cost into O(log n): a
    // type-1 query is a single lazy range update (FlipCountOperation rewrites a node's
    // count as rangeLength - count without touching its leaves), and a type-2 query is
    // one whole-array Query for the current count of ones.
    public static long[] HandleQueryByLazySegmentTree(int[] nums1, int[] nums2, int[][] queries)
    {
        var ones = new LazySegmentTree<int, bool, FlipCountOperation>(nums1);
        var lastIndex = nums1.Length - 1;
        var total = nums2.Sum(value => (long)value);
        var answers = new List<long>();

        foreach (var query in queries)
        {
            switch (query[0])
            {
                case FlipQuery:
                    ones.UpdateRange(query[1], query[2], true);
                    break;
                case AddScaledOnesQuery:
                    total += (long)query[1] * ones.Query(0, lastIndex);
                    break;
                default:
                    answers.Add(total);
                    break;
            }
        }

        return answers.ToArray();
    }
}
