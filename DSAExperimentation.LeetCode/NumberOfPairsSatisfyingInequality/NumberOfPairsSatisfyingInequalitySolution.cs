using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.NumberOfPairsSatisfyingInequality;

// LeetCode 2426. Number of Pairs Satisfying Inequality: count the index pairs
// i < j with nums1[i]-nums1[j] <= nums2[i]-nums2[j]+diff.
//
// Rearranged, that condition is d[i] <= d[j]+diff where d[i] = nums1[i]-nums2[i],
// which turns a two-array pair count into a one-array rank query: for each j,
// how many earlier d[i] sit at or below d[j]+diff. The two strategies differ only
// in whether they answer that by re-scanning the prefix or by keeping it in a
// Binary Indexed Tree of counts.
internal static class NumberOfPairsSatisfyingInequalitySolution
{
    // The textbook answer: test every pair against the inequality exactly as the
    // statement writes it. Deliberately written without this repo's primitives -
    // no rearrangement, no compression - it is the arm the composed solution below
    // has to justify itself against. O(n^2).
    public static long CountPairsByPairwiseScan(int[] nums1, int[] nums2, int diff)
    {
        var count = 0L;

        for (var i = 0; i < nums1.Length; i++)
        {
            for (var j = i + 1; j < nums1.Length; j++)
            {
                if (nums1[i] - nums1[j] <= nums2[i] - nums2[j] + diff)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Coordinate-compress the differences via the sorted distinct values, then
    // sweep left to right: BinarySearch.UpperBound finds how many distinct values
    // are <= d[j]+diff, and FenwickTree<int, SumOperation<int>>.PrefixQuery sums
    // how many already-swept d[i] (so i < j by construction) sit at or below that
    // rank. Add(ownRank, 1) admits the current element before moving on.
    // O(n log n) - the same compress-and-sweep shape LC 315 uses, generalized from
    // a strict "smaller" count to a "<=" count with a per-query offset.
    public static long CountPairsByFenwickTreeSweep(int[] nums1, int[] nums2, int diff)
    {
        var differences = DifferencesOf(nums1, nums2);
        var sortedDistinct = differences.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);

        var count = 0L;

        foreach (var value in differences)
        {
            var upperRank = BinarySearch.UpperBound(sequence, value + diff);
            count += upperRank == 0 ? 0 : tree.PrefixQuery(upperRank - 1);

            var ownRank = BinarySearch.LowerBound(sequence, value);
            tree.Add(ownRank, 1);
        }

        return count;
    }

    // The rearrangement the sweep rests on: the pair condition
    // nums1[i]-nums1[j] <= nums2[i]-nums2[j]+diff is d[i] <= d[j]+diff over these
    // per-index differences alone, so both arrays collapse into one axis.
    private static int[] DifferencesOf(int[] nums1, int[] nums2)
    {
        var differences = new int[nums1.Length];

        for (var i = 0; i < nums1.Length; i++)
        {
            differences[i] = nums1[i] - nums2[i];
        }

        return differences;
    }
}
