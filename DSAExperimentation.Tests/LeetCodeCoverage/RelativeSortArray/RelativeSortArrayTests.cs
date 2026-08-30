using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RelativeSortArray;

// LeetCode 1122. Relative Sort Array: a HashMap<int,int> maps each arr2 value to its
// index (its "rank"), then this repo's own MergeSort.Sort<Element,TSequence> sorts
// arr1 by that rank - values absent from arr2 sort after every ranked value, tied
// among themselves by their own value ascending - the same custom-comparer-over-
// ArrayIndexedSequence shape TwoCitySchedulingTests.cs already exercises.
public sealed class RelativeSortArrayTests
{
    [Fact]
    public void RelativeSortArray_LeetCodeExample_RanksByArr2ThenAppendsRemainderAscending()
    {
        int[] arr1 = [2, 3, 1, 3, 2, 4, 6, 7, 9, 2, 19];
        int[] arr2 = [2, 1, 4, 3, 9, 6];

        var sorted = RelativeSortArray(arr1, arr2);

        Assert.Equal([2, 2, 2, 1, 4, 3, 3, 9, 6, 7, 19], sorted);
    }

    [Fact]
    public void RelativeSortArray_SecondLeetCodeExample_ReturnsExpectedOrder()
    {
        int[] arr1 = [28, 6, 22, 8, 44, 17];
        int[] arr2 = [22, 28, 8, 6];

        var sorted = RelativeSortArray(arr1, arr2);

        Assert.Equal([22, 28, 8, 6, 17, 44], sorted);
    }

    private static int[] RelativeSortArray(int[] arr1, int[] arr2)
    {
        var rank = new HashMap<int, int>();
        for (var i = 0; i < arr2.Length; i++)
        {
            rank.Set(arr2[i], i);
        }

        var sorted = (int[])arr1.Clone();
        var byRelativeOrder = Comparer<int>.Create((a, b) =>
        {
            var aKey = rank.TryGetValue(a, out var aIndex) ? aIndex : int.MaxValue;
            var bKey = rank.TryGetValue(b, out var bIndex) ? bIndex : int.MaxValue;
            return aKey != bKey ? aKey.CompareTo(bKey) : a.CompareTo(b);
        });

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted), byRelativeOrder);
        return sorted;
    }
}
