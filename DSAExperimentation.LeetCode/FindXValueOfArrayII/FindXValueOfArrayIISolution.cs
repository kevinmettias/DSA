using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.FindXValueOfArrayII;

// LeetCode 3525. Find X Value of Array II: nums and modulus are fixed up front;
// each query updates one element, then narrows the array to a suffix starting at
// starti, then asks how many ways there are to further drop a (possibly empty)
// suffix of THAT so the remaining product is xi mod modulus. "Ways to drop a
// suffix" is just "prefixes of nums[starti..] and their product mod modulus", so each query is a
// range aggregate over [starti, n-1] plus one point update - exactly what
// SegmentTree<Element,TOperation> answers in O(log n), with XValueNode/
// XValueCombineOperation as the element/operation pair.
internal static class FindXValueOfArrayIISolution
{
    // The textbook baseline: BCL array, no segment tree - recompute the running
    // product from start on every query. O(n) per query, deliberately not reusing
    // anything from a previous query the way the segment tree does.
    public static int[] XValueCountsByBruteForce(int[] nums, int modulus, int[][] queries)
    {
        var values = (int[])nums.Clone();
        var results = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = ApplyQuery(values, modulus, queries[i]);
        }

        return results;
    }

    // One query: write the updated value into the array, then count the prefixes of
    // nums[start..] whose running product mod modulus lands on x.
    private static int ApplyQuery(int[] values, int modulus, int[] query)
    {
        var (index, value, start, x) = (query[0], query[1], query[2], query[3]);
        values[index] = value;

        var product = 1 % modulus;
        var count = 0;

        for (var j = start; j < values.Length; j++)
        {
            product = product * (values[j] % modulus) % modulus;

            if (product == x)
            {
                count++;
            }
        }

        return count;
    }

    // The composed strategy: modulus <= 5 is a closed set, so dispatch once onto the
    // matching IModulus witness and let XValueCombineOperation<TModulus> and
    // SegmentTree carry every query in O(log n) - one point update plus one range
    // query per query, versus the baseline's full O(n) rescan.
    public static int[] XValueCountsBySegmentTreeAutomaton(int[] nums, int modulus, int[][] queries) => modulus switch
    {
        1 => Solve<Modulus1>(nums, queries),
        2 => Solve<Modulus2>(nums, queries),
        3 => Solve<Modulus3>(nums, queries),
        4 => Solve<Modulus4>(nums, queries),
        5 => Solve<Modulus5>(nums, queries),
        _ => throw new ArgumentOutOfRangeException(
            nameof(modulus), modulus, "LeetCode 3525 guarantees 1 <= modulus <= 5."),
    };

    private static int[] Solve<TModulus>(int[] nums, int[][] queries)
        where TModulus : struct, IModulus
    {
        var leaves = new XValueNode[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            leaves[i] = XValueCombineOperation<TModulus>.Leaf(nums[i]);
        }

        var tree = new SegmentTree<XValueNode, XValueCombineOperation<TModulus>>(leaves);

        return RunQueries<TModulus>(tree, queries);
    }

    private static int[] RunQueries<TModulus>(
        SegmentTree<XValueNode, XValueCombineOperation<TModulus>> tree, int[][] queries)
        where TModulus : struct, IModulus
    {
        var startResidue = 1 % TModulus.Value;
        var results = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var (index, value, start, x) = (queries[i][0], queries[i][1], queries[i][2], queries[i][3]);
            tree.Update(index, XValueCombineOperation<TModulus>.Leaf(value));

            var aggregate = tree.Query(start, tree.Count - 1);
            results[i] = aggregate.Counts[startResidue, x];
        }

        return results;
    }
}
