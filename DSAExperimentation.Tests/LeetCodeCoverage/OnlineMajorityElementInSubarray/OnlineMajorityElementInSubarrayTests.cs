using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OnlineMajorityElementInSubarray;

// LeetCode 1157. Online Majority Element In Subarray: groups each value's occurrence
// indices into this repo's own DynamicArray<int> (naturally ascending, since built by
// one left-to-right construction pass), keyed by value in a
// HashMap<int, DynamicArray<int>>. A query samples a bounded number of candidate
// indices inside [left, right] and, for each one, counts how many of that value's
// stored positions fall in range via BinarySearch.LowerBound/UpperBound over a
// DynamicArraySequence<int> view onto the same DynamicArray - the same "wrap an
// existing structure in an IRandomAccessSequence witness" idiom
// RangeModuleTests (LC 715) already uses. LeetCode guarantees 2*threshold >
// right-left+1 whenever a query has an answer, so the answer element (when one
// exists) is a true majority of the subarray - which is what makes a bounded number
// of samples enough to find it with overwhelming probability, matching the problem's
// own official randomized approach.
public sealed partial class OnlineMajorityElementInSubarrayTests
{
    [Fact]
    public void Query_LeetCodeExample_ReturnsExpectedMajorityOrNegativeOne()
    {
        var checker = new MajorityChecker([1, 1, 2, 2, 1, 1]);

        Assert.Equal(1, checker.Query(0, 5, 4));
        Assert.Equal(-1, checker.Query(0, 3, 3));
        Assert.Equal(2, checker.Query(2, 3, 2));
    }

    [Fact]
    public void Query_SingleIndexRange_ReturnsThatElement()
    {
        var checker = new MajorityChecker([5, 5, 5, 5, 5]);

        Assert.Equal(5, checker.Query(2, 2, 1));
    }

    private sealed class MajorityChecker
    {
        private const int SampleAttempts = 20;

        private readonly int[] _values;
        private readonly HashMap<int, DynamicArray<int>> _positionsByValue = new();
        private readonly Random _random = new(1);

        public MajorityChecker(int[] arr)
        {
            _values = arr;

            for (var i = 0; i < arr.Length; i++)
            {
                if (!_positionsByValue.TryGetValue(arr[i], out var positions))
                {
                    positions = new DynamicArray<int>();
                    _positionsByValue.Set(arr[i], positions);
                }

                positions.Add(i);
            }
        }

        public int Query(int left, int right, int threshold)
        {
            for (var attempt = 0; attempt < SampleAttempts; attempt++)
            {
                var sampleIndex = left + _random.Next(right - left + 1);
                var value = _values[sampleIndex];

                if (CountInRange(value, left, right) >= threshold)
                {
                    return value;
                }
            }

            return -1;
        }

        private int CountInRange(int value, int left, int right)
        {
            if (!_positionsByValue.TryGetValue(value, out var positions))
            {
                return 0;
            }

            var sequence = new DynamicArraySequence<int>(positions);
            var lower = BinarySearch.LowerBound<int, DynamicArraySequence<int>>(sequence, left);
            var upper = BinarySearch.UpperBound<int, DynamicArraySequence<int>>(sequence, right);

            return upper - lower;
        }
    }
}
