using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountOfSmallerNumbersAfterSelf;

// LeetCode 315. Count of Smaller Numbers After Self: for every index, how many
// later values are strictly smaller than it.
internal static class CountOfSmallerNumbersAfterSelfSolution
{
    // The textbook answer: for every index, scan every later index and count.
    // Deliberately written without this repo's primitives; it is the arm the
    // composed solution below has to justify itself against.
    public static int[] CountSmallerByPairwiseScan(int[] nums)
    {
        var counts = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            var count = 0;

            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[j] < nums[i])
                {
                    count++;
                }
            }

            counts[i] = count;
        }

        return counts;
    }

    // Coordinate-compress nums via BinarySearch.LowerBound over the sorted
    // distinct values, then sweep right-to-left through a
    // FenwickTree<int, SumOperation<int>> (this repo's own Binary Indexed Tree) -
    // PrefixQuery(rank-1) counts every smaller value already added on the way in,
    // and Add(rank, 1) records the current one before moving further left.
    public static int[] CountSmallerByFenwickTreeSweep(int[] nums)
    {
        var sortedDistinct = nums.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var counts = new int[nums.Length];

        for (var i = nums.Length - 1; i >= 0; i--)
        {
            var rank = BinarySearch.LowerBound(sequence, nums[i]);
            counts[i] = rank == 0 ? 0 : tree.PrefixQuery(rank - 1);
            tree.Add(rank, 1);
        }

        return counts;
    }
}
