using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RelativeSortArray;

// LeetCode 1122. Relative Sort Array: order arr1 so that every value also present
// in arr2 appears in arr2's order, and every value absent from arr2 follows, sorted
// ascending among themselves.
//
// Both strategies are the same total order - "rank if ranked, int.MaxValue
// otherwise, ties broken by the value itself" - and differ only in how a value's
// rank is obtained: rescanned out of arr2 on every single comparison, or looked up
// once per comparison from a rank table built in one pass.
internal static class RelativeSortArraySolution
{
    // Sorts after every ranked value, tied among themselves by value ascending.
    private const int UnrankedKey = int.MaxValue;

    // The textbook approach: hand Array.Sort a comparer that re-derives each
    // element's rank with Array.IndexOf, so every comparison costs a full scan of
    // arr2 - O(n log n * m). Pure BCL, the baseline the rank-table approach is
    // measured against.
    public static int[] RelativeSortByLinearScanComparer(int[] arr1, int[] arr2)
    {
        var sorted = (int[])arr1.Clone();

        Array.Sort(sorted, (a, b) =>
        {
            var aRank = Array.IndexOf(arr2, a);
            var bRank = Array.IndexOf(arr2, b);
            var aKey = aRank >= 0 ? aRank : UnrankedKey;
            var bKey = bRank >= 0 ? bRank : UnrankedKey;
            return aKey != bKey ? aKey.CompareTo(bKey) : a.CompareTo(b);
        });

        return sorted;
    }

    // Precompute every value's rank once into this repo's own HashMap<int,int> -
    // O(n + m) - then sort by that O(1) lookup with MergeSort.Sort<Element,TSequence>
    // over an ArrayIndexedSequence, the same custom-comparer shape ArrayPartition
    // and TwoCityScheduling use.
    public static int[] RelativeSortByHashMapMergeSort(int[] arr1, int[] arr2)
    {
        var rank = new HashMap<int, int>();
        for (var i = 0; i < arr2.Length; i++)
        {
            rank.Set(arr2[i], i);
        }

        var sorted = (int[])arr1.Clone();
        var byRelativeOrder = Comparer<int>.Create((a, b) =>
        {
            var aKey = rank.TryGetValue(a, out var aIndex) ? aIndex : UnrankedKey;
            var bKey = rank.TryGetValue(b, out var bIndex) ? bIndex : UnrankedKey;
            return aKey != bKey ? aKey.CompareTo(bKey) : a.CompareTo(b);
        });

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted), byRelativeOrder);
        return sorted;
    }
}
