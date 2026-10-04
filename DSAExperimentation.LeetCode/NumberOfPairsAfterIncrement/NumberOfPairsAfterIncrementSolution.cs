using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.RangeFenwickTree;

namespace DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

// LeetCode 3943. Number of Pairs After Increment: nums1 stays tiny (<= 5) while
// nums2 and the query stream can each reach 5*10^4. A type-1 query range-adds
// into nums2; a type-2 query counts pairs (j, k) with nums1[j] + nums2[k] == tot,
// which - since nums1 is so small - is really "for each of <= 5 targets
// tot - nums1[j], how many nums2[k] currently equal it".
//
// The first two arms rebuild nums2's current value at every index for each
// type-2 query; they differ only in how a "current value" is obtained after a
// range of prior increments, and both are O(n) per count - about 2.5 * 10^9 steps
// at LeetCode's limits. The third is the textbook answer: a square-root
// decomposition, ShiftedValueBlocks in this folder, that keeps per-block counts by
// value under a lazy per-block addition and answers either kind of query in
// O(sqrt n). The core library's Fenwick and segment trees aggregate by index, not
// by value, so that structure is this problem's own.
internal static class NumberOfPairsAfterIncrementSolution
{
    // Textbook baseline: nums2 as a plain mutable array, a range-add applied by
    // walking every index in [x, y], and a type-2 query answered by a nested
    // scan over nums1 x nums2. Deliberately BCL - the arm the Fenwick-composed
    // strategy below has to justify itself against.
    public static int[] CountPairsByDirectArray(int[] nums1, int[] nums2, int[][] queries) =>
        CountPairsByDirectArray(nums1, nums2, ParseQueries(queries));

    public static int[] CountPairsByDirectArray(int[] nums1, int[] nums2, IReadOnlyList<PairQuery> queries)
    {
        var working = Array.ConvertAll(nums2, value => (long)value);
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query.Kind == PairQueryKind.Increment)
            {
                for (var index = query.Left; index <= query.Right; index++)
                {
                    working[index] += query.Delta;
                }
            }
            else
            {
                var matches = CountMatchesByScan(nums1, working, query.Tot);
                results.Add(matches);
            }
        }

        return [.. results];
    }

    private static int CountMatchesByScan(int[] nums1, long[] working, long tot)
    {
        var count = 0;

        foreach (var a in nums1)
        {
            var target = tot - a;

            foreach (var value in working)
            {
                if (value == target)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Composed: nums2's running values live in a RangeFenwickTree<long,
    // SumOperation<long>> - the classic two-Fenwick-tree range-add/point-
    // query pair (see RangeFenwickTree.cs's own doc comment) - so a type-1 query
    // is one O(log n) RangeAdd instead of an O(range) walk. A type-2 query still
    // has to materialize every current value once to build its frequency map
    // (this repo has nothing that answers "count equal to v" without doing
    // that), so this arm trades a cheaper update for a costlier per-index point
    // query; it is offered as proof the primitive composes correctly, not as a
    // claim that it dominates the baseline on every workload.
    public static int[] CountPairsByRangeFenwickTree(int[] nums1, int[] nums2, int[][] queries) =>
        CountPairsByRangeFenwickTree(nums1, nums2, ParseQueries(queries));

    public static int[] CountPairsByRangeFenwickTree(int[] nums1, int[] nums2, IReadOnlyList<PairQuery> queries)
    {
        var initial = Array.ConvertAll(nums2, value => (long)value);
        var tree = new RangeFenwickTree<long, SumOperation<long>>(initial);
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query.Kind == PairQueryKind.Increment)
            {
                tree.RangeAdd(query.Left, query.Right, query.Delta);
            }
            else
            {
                var matches = CountMatchesByFrequencyMap(nums1, tree, query.Tot);
                results.Add(matches);
            }
        }

        return [.. results];
    }

    private static int CountMatchesByFrequencyMap(
        int[] nums1, RangeFenwickTree<long, SumOperation<long>> tree, long tot)
    {
        var frequency = new Dictionary<long, int>();

        for (var index = 0; index < tree.Count; index++)
        {
            var value = tree.Query(index, index);
            frequency[value] = frequency.GetValueOrDefault(value) + 1;
        }

        var count = 0;

        foreach (var a in nums1)
        {
            count += frequency.GetValueOrDefault(tot - a);
        }

        return count;
    }

    // Composed: nums2 lives in ShiftedValueBlocks, so a type-1 query costs the two
    // blocks it cuts plus one pending-addition bump per block it covers, and a type-2
    // query asks every block for each of nums1's at most five targets - O(sqrt n)
    // for either, against the rebuilds' O(n) per count.
    public static int[] CountPairsByValueBlocks(int[] nums1, int[] nums2, int[][] queries) =>
        CountPairsByValueBlocks(nums1, nums2, ParseQueries(queries));

    public static int[] CountPairsByValueBlocks(int[] nums1, int[] nums2, IReadOnlyList<PairQuery> queries)
    {
        var blocks = new ShiftedValueBlocks(nums2);
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query.Kind == PairQueryKind.Increment)
            {
                blocks.AddRange(query.Left, query.Right, query.Delta);
            }
            else
            {
                var matches = CountMatchesByBlocks(nums1, blocks, query.Tot);
                results.Add(matches);
            }
        }

        return [.. results];
    }

    private static int CountMatchesByBlocks(int[] nums1, ShiftedValueBlocks blocks, long tot)
    {
        var count = 0;

        foreach (var a in nums1)
        {
            count += blocks.CountEqual(tot - a);
        }

        return count;
    }

    private static List<PairQuery> ParseQueries(int[][] queries)
    {
        var parsed = new List<PairQuery>(queries.Length);

        foreach (var query in queries)
        {
            var isIncrement = query[0] == 1;
            parsed.Add(isIncrement
                ? PairQuery.Increment(query[1], query[2], query[3])
                : PairQuery.Count(query[1]));
        }

        return parsed;
    }
}
