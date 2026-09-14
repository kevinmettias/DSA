using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RangeFrequencyQueries;

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
internal static class RangeFrequencyQueriesSolution
{
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

    // LeetCode's shape: builds RangeFreqQuery's index fresh for this one call.
    public static int QueryByBinarySearchIndex(int[] arr, int left, int right, int value)
    {
        var indicesByValue = BuildValueIndex(arr);

        return QueryByBinarySearchIndex(indicesByValue, left, right, value);
    }

    // The prepared-input overload: the index RangeFreqQuery's constructor would
    // already have built, so a caller answering many queries against the same array
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
}
