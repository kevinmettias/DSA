using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.DistributeElementsIntoTwoArraysII;

// LeetCode 3072. Distribute Elements Into Two Arrays II: nums[1] seeds arr1,
// nums[2] seeds arr2, then every later nums[i] joins whichever array currently
// holds more elements strictly greater than it - a tie in that count goes to
// whichever array has fewer elements so far, and a further tie goes to arr1.
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class DistributeElementsIntoTwoArraysIISolution
{
    // Textbook: keep arr1/arr2 as growing Lists and count "greater than" by a
    // linear scan of each list per operation. O(n) per operation, O(n^2)
    // overall - the arm the Fenwick-tree strategy has to beat.
    public static int[] DistributeByBruteForce(int[] nums)
    {
        var arr1 = new List<int> { nums[0] };
        var arr2 = new List<int> { nums[1] };

        for (var i = 2; i < nums.Length; i++)
        {
            var value = nums[i];
            var greater1 = CountGreater(arr1, value);
            var greater2 = CountGreater(arr2, value);

            if (ShouldGoToFirstArray(greater1, greater2, arr1.Count, arr2.Count))
            {
                arr1.Add(value);
            }
            else
            {
                arr2.Add(value);
            }
        }

        return Concatenate(arr1, arr2);
    }

    private static int CountGreater(List<int> values, int target)
    {
        var count = 0;

        foreach (var candidate in values)
        {
            if (candidate > target)
            {
                count++;
            }
        }

        return count;
    }

    // Composed: coordinate-compress nums via BinarySearch.LowerBound over the
    // sorted distinct values (the same idiom CountOfSmallerNumbersAfterSelfTests
    // uses for a Fenwick-tree sweep), then keep one FenwickTree<int,
    // SumOperation<int>> per array recording which ranks it has taken so far.
    // greaterCount(x) is the array's own element count minus
    // PrefixQuery(rank(x)) - every element at or below x's rank - so what is
    // left over is exactly the count strictly greater than x. O(n log n)
    // overall.
    public static int[] DistributeByFenwickTree(int[] nums)
    {
        var sortedDistinct = nums.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);

        var tree1 = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var tree2 = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var arr1 = new List<int> { nums[0] };
        var arr2 = new List<int> { nums[1] };

        var firstRank = BinarySearch.LowerBound(sequence, nums[0]);
        tree1.Add(firstRank, 1);
        var secondRank = BinarySearch.LowerBound(sequence, nums[1]);
        tree2.Add(secondRank, 1);

        var first = (Values: arr1, Tree: tree1);
        var second = (Values: arr2, Tree: tree2);

        for (var i = 2; i < nums.Length; i++)
        {
            PlaceByGreaterCount(sequence, nums[i], first, second);
        }

        return Concatenate(arr1, arr2);
    }

    // One element lands in whichever array currently holds more elements strictly
    // greater than it, and that array's Fenwick tree records the rank it took.
    // ShouldGoToFirstArray breaks the remaining ties; the rank is the same
    // coordinate-compressed lookup the two greater-counts are read at.
    private static void PlaceByGreaterCount(
        ArraySequence<int> sequence,
        int value,
        (List<int> Values, FenwickTree<int, SumOperation<int>> Tree) first,
        (List<int> Values, FenwickTree<int, SumOperation<int>> Tree) second)
    {
        var rank = BinarySearch.LowerBound(sequence, value);
        var greater1 = first.Values.Count - first.Tree.PrefixQuery(rank);
        var greater2 = second.Values.Count - second.Tree.PrefixQuery(rank);

        if (ShouldGoToFirstArray(greater1, greater2, first.Values.Count, second.Values.Count))
        {
            first.Values.Add(value);
            first.Tree.Add(rank, 1);
        }
        else
        {
            second.Values.Add(value);
            second.Tree.Add(rank, 1);
        }
    }

    // A strictly higher greater-count wins; a tie goes to whichever array has
    // fewer elements so far, and a further tie (equal counts, equal lengths)
    // goes to arr1.
    private static bool ShouldGoToFirstArray(int greater1, int greater2, int count1, int count2) =>
        greater1 > greater2 || (greater1 == greater2 && count1 <= count2);

    private static int[] Concatenate(List<int> arr1, List<int> arr2)
    {
        var result = new int[arr1.Count + arr2.Count];
        arr1.CopyTo(result, 0);
        arr2.CopyTo(result, arr1.Count);
        return result;
    }
}
