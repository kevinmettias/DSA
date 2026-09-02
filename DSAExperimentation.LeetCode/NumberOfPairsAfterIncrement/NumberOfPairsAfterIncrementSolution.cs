using DSAExperimentation.DataStructures.RangeFenwickTree;

namespace DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

// LeetCode 3943. Number of Pairs After Increment: nums1 stays tiny (<= 5) while
// nums2 and the query stream can each reach 5*10^4. A type-1 query range-adds
// into nums2; a type-2 query counts pairs (j, k) with nums1[j] + nums2[k] == tot,
// which - since nums1 is so small - is really "for each of <= 5 targets
// tot - nums1[j], how many nums2[k] currently equal it".
//
// Both arms below rebuild nums2's current value at every index for each type-2
// query and tally it into a frequency map; they differ only in how a "current
// value" is obtained after a range of prior increments. Neither is the fully
// optimal answer to this problem - the textbook approach needs a block/"sqrt"
// decomposition (per-block value histograms under a lazy per-block add) to
// answer "how many elements now equal v" without visiting every index, and this
// repo has no such structure: FenwickTree/RangeFenwickTree/SegmentTree/
// LazySegmentTree all aggregate by INDEX (sum/min/max over a range), none tracks
// frequency-by-VALUE under a range shift. That is a genuine primitive gap, not
// an effort shortcut - see the composed arm's own comment for what it proves
// instead.
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
                results.Add(CountMatchesByScan(nums1, working, query.Tot));
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
    // ScaledSumOperation<long>> - the classic two-Fenwick-tree range-add/point-
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
        var tree = new RangeFenwickTree<long, ScaledSumOperation<long>>(initial);
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query.Kind == PairQueryKind.Increment)
            {
                tree.RangeAdd(query.Left, query.Right, query.Delta);
            }
            else
            {
                results.Add(CountMatchesByFrequencyMap(nums1, tree, query.Tot));
            }
        }

        return [.. results];
    }

    private static int CountMatchesByFrequencyMap(
        int[] nums1, RangeFenwickTree<long, ScaledSumOperation<long>> tree, long tot)
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

    private static List<PairQuery> ParseQueries(int[][] queries)
    {
        var parsed = new List<PairQuery>(queries.Length);

        foreach (var query in queries)
        {
            parsed.Add(query[0] == 1
                ? PairQuery.Increment(query[1], query[2], query[3])
                : PairQuery.Count(query[1]));
        }

        return parsed;
    }
}
