using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeFrequencyQueries;

// LeetCode 2080. Range Frequency Queries: this repo's own HashMap<TKey,TValue> maps
// each distinct value to a DynamicArray<int> of the indices where it occurs, built in
// ascending index order (so it's already sorted, with no extra sort step needed).
// Query wraps that DynamicArray in a DynamicArraySequence and answers with
// BinarySearch.UpperBound - LowerBound, an O(log n) count instead of a fresh O(n)
// rescan of arr[left..right] per call.
public sealed class RangeFrequencyQueriesTests
{
    [Fact]
    public void Query_LeetCodeExample_ReturnsFrequencyOfValueInSubarray()
    {
        var rangeFreqQuery = new RangeFreqQueryOperations([12, 33, 4, 56, 22, 2, 34, 33, 22, 12, 34, 56]);

        var firstQuery = rangeFreqQuery.Query(1, 2, 4);
        var secondQuery = rangeFreqQuery.Query(0, 11, 33);

        Assert.Equal(1, firstQuery);
        Assert.Equal(2, secondQuery);
    }

    [Fact]
    public void Query_ValueNeverOccursInArray_ReturnsZero()
    {
        var rangeFreqQuery = new RangeFreqQueryOperations([1, 2, 3]);

        var count = rangeFreqQuery.Query(0, 2, 99);

        Assert.Equal(0, count);
    }

    private sealed class RangeFreqQueryOperations
    {
        private readonly HashMap<int, DynamicArray<int>> _indicesByValue = new();

        public RangeFreqQueryOperations(int[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                if (!_indicesByValue.TryGetValue(arr[i], out var indices))
                {
                    indices = new DynamicArray<int>();
                    _indicesByValue.Set(arr[i], indices);
                }

                indices.Add(i);
            }
        }

        public int Query(int left, int right, int value)
        {
            if (!_indicesByValue.TryGetValue(value, out var indices))
            {
                return 0;
            }

            var sequence = new DynamicArraySequence<int>(indices);
            return BinarySearch.UpperBound(sequence, right) - BinarySearch.LowerBound(sequence, left);
        }
    }
}
