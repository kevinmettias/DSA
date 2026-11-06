using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.Solutions.RangeFrequencyQueries;

// LeetCode 2080. Range Frequency Queries: RangeFreqQuery(arr) is built once, then
// query(left, right, value) reports how many times value occurs in
// arr[left..right] and may be called any number of times against it.
//
// QueryByBruteForceRescan is the textbook answer - rescan the slice on every call,
// O(right - left) each time. QueryByBinarySearchIndex builds this repo's own
// HashMap<TKey,TValue> mapping each distinct value to a DynamicArray<int> of the
// indices where it occurs, appended in ascending index order so no sort step is
// needed; every query afterward wraps that DynamicArray in a DynamicArraySequence
// and answers with BinarySearch.UpperBound - LowerBound, an O(log n) count.
//
// LeetCode's shape is that object and its query calls, so each strategy's overload
// taking only the array makes one, the object the catalog harness replays
// LeetCode's call scripts on (ARCHITECTURE 17.13). The object asks the strategy's
// query overload, which the hand-written benchmark calls directly, so each
// strategy is written once.
[LeetCodeQuestion("range-frequency-queries", LeetCodeAnswerOrder.AsWritten)]
internal static class RangeFrequencyQueriesSolution
{
    // LeetCode's shape: a RangeFreqQuery that keeps the array as given and rescans
    // the window on every query.
    public static IRangeFreqQuery QueryByBruteForceRescan(int[] arr) => new BruteForceRescanQuery(arr);

    // The textbook answer: count occurrences in the slice directly on every call.
    // Deliberately written without this repo's primitives - it is the arm the
    // indexed strategy below has to justify itself against.
    public static int QueryByBruteForceRescan(int[] arr, int left, int right, int value)
    {
        var count = 0;

        for (var i = left; i <= right; i++)
        {
            if (arr[i] == value)
            {
                count++;
            }
        }

        return count;
    }

    // LeetCode's shape: a RangeFreqQuery whose constructor builds the value index
    // once, so every query afterward bisects it.
    public static IRangeFreqQuery QueryByBinarySearchIndex(int[] arr) => new BinarySearchIndexQuery(arr);

    // The prepared-input overload: one query against the index RangeFreqQuery's
    // constructor built, so a caller answering many queries against the same array
    // pays construction once rather than per query.
    public static int QueryByBinarySearchIndex(
        HashMap<int, DynamicArray<int>> indicesByValue, int left, int right, int value)
    {
        if (!indicesByValue.TryGetValue(value, out var indices))
        {
            return 0;
        }

        var sequence = new DynamicArraySequence<int>(indices);

        return BinarySearch.UpperBound(sequence, right) - BinarySearch.LowerBound(sequence, left);
    }

    // Each value's index list is appended in ascending index order, which is exactly
    // the sorted order the two bounds above bisect over.
    public static HashMap<int, DynamicArray<int>> BuildValueIndex(int[] arr)
    {
        var indicesByValue = new HashMap<int, DynamicArray<int>>();

        for (var i = 0; i < arr.Length; i++)
        {
            if (!indicesByValue.TryGetValue(arr[i], out var indices))
            {
                indices = new DynamicArray<int>();
                indicesByValue.Set(arr[i], indices);
            }

            indices.Add(i);
        }

        return indicesByValue;
    }

    // The array as given; every query rescans its window.
    private sealed class BruteForceRescanQuery(int[] arr) : IRangeFreqQuery
    {
        public int Query(int left, int right, int value) => QueryByBruteForceRescan(arr, left, right, value);
    }

    // Each value's positions, indexed once; every query bisects one value's list twice.
    private sealed class BinarySearchIndexQuery(int[] arr) : IRangeFreqQuery
    {
        private readonly HashMap<int, DynamicArray<int>> _indicesByValue = BuildValueIndex(arr);

        public int Query(int left, int right, int value) => QueryByBinarySearchIndex(_indicesByValue, left, right, value);
    }
}
