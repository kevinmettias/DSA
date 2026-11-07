using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.Solutions.RangeFrequencyQueries;

// LeetCode 2080. Range Frequency Queries: RangeFreqQuery(arr) is built once, then
// query(left, right, value) reports how many times value occurs in
// arr[left..right] and may be called any number of times against it.
//
// BruteForceRescan is the textbook answer - rescan the slice on every call,
// O(right - left) each time. BinarySearchIndex builds this repo's own
// HashMap<TKey,TValue> mapping each distinct value to a DynamicArray<int> of the
// indices where it occurs, appended in ascending index order so no sort step is
// needed; every query afterward wraps that DynamicArray in a DynamicArraySequence
// and answers with BinarySearch.UpperBound - LowerBound, an O(log n) count.
//
// LeetCode's shape is that object and its query calls, so each strategy is a
// factory, CreateBy<Strategy>, taking only the array and making one: the object
// the catalog harness replays LeetCode's call scripts on (ARCHITECTURE 17.13). The
// strategy's algorithm lives in a nested class named for it - BruteForceRescan.Query,
// and BinarySearchIndex's BuildValueIndex and Query - which the object asks and the
// hand-written benchmark calls directly, so each strategy is written once.
[LeetCodeQuestion("range-frequency-queries", LeetCodeAnswerOrder.AsWritten)]
internal static class RangeFrequencyQueriesSolution
{
    // LeetCode's shape: a RangeFreqQuery that keeps the array as given and rescans
    // the window on every query.
    public static IRangeFreqQuery CreateByBruteForceRescan(int[] arr) => new BruteForceRescanQuery(arr);

    // LeetCode's shape: a RangeFreqQuery whose constructor builds the value index
    // once, so every query afterward bisects it.
    public static IRangeFreqQuery CreateByBinarySearchIndex(int[] arr) => new BinarySearchIndexQuery(arr);

    // The brute-force strategy's one query.
    internal static class BruteForceRescan
    {
        // The textbook answer: count occurrences in the slice directly on every call.
        // Deliberately written without this repo's primitives - it is the arm the
        // indexed strategy below has to justify itself against.
        public static int Query(int[] arr, int left, int right, int value)
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
    }

    // The binary-search-index strategy: the index its constructor builds once, and
    // one query against it.
    internal static class BinarySearchIndex
    {
        // The prepared-input query: one count against the index RangeFreqQuery's
        // constructor built, so a caller answering many queries against the same
        // array pays construction once rather than per query.
        public static int Query(
            HashMap<int, DynamicArray<int>> indicesByValue, int left, int right, int value)
        {
            if (!indicesByValue.TryGetValue(value, out var indices))
            {
                return 0;
            }

            var sequence = new DynamicArraySequence<int>(indices);

            return BinarySearch.UpperBound(sequence, right) - BinarySearch.LowerBound(sequence, left);
        }

        // Each value's index list is appended in ascending index order, which is
        // exactly the sorted order the two bounds above bisect over.
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
    }

    // The array as given; every query rescans its window.
    private sealed class BruteForceRescanQuery(int[] arr) : IRangeFreqQuery
    {
        public int Query(int left, int right, int value) => BruteForceRescan.Query(arr, left, right, value);
    }

    // Each value's positions, indexed once; every query bisects one value's list twice.
    private sealed class BinarySearchIndexQuery(int[] arr) : IRangeFreqQuery
    {
        private readonly HashMap<int, DynamicArray<int>> _indicesByValue = BinarySearchIndex.BuildValueIndex(arr);

        public int Query(int left, int right, int value) => BinarySearchIndex.Query(_indicesByValue, left, right, value);
    }
}
