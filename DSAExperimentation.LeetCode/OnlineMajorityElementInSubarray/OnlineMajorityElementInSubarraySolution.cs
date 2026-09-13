using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.OnlineMajorityElementInSubarray;

// LeetCode 1157. Online Majority Element In Subarray: a MajorityChecker is built once
// from an array and then asked, repeatedly, for a value occurring at least `threshold`
// times inside [left, right] - or -1 if there is none.
//
// A Design problem's whole point is a sequence of calls against one instance, so
// "every strategy for the problem" (ARCHITECTURE.md 17.3) takes the form of two
// classes implementing the shared IMajorityChecker surface below, the same shape
// StreamOfCharactersSolution uses for LC 1032.
//
// LeetCode guarantees 2*threshold > right-left+1 on every query, so at most one value
// can ever satisfy a query - which is what makes both "return the first value that
// clears the threshold" (the tally baseline) and "sample a bounded number of candidate
// positions" (the indexed strategy) correct rather than merely plausible.
internal static class OnlineMajorityElementInSubarraySolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one query sequence against either strategy without
    // restating it.
    internal interface IMajorityChecker
    {
        int Query(int left, int right, int threshold);
    }

    // The textbook answer: no precomputation at all, just tally every value in
    // [left, right] into a BCL Dictionary on each query and report the first one that
    // clears the threshold - O(range) per query. Deliberately written without this
    // repo's primitives; it is the arm the indexed strategy has to justify itself
    // against.
    internal sealed class MajorityCheckerByRangeTally : IMajorityChecker
    {
        private readonly int[] _values;

        public MajorityCheckerByRangeTally(int[] arr) => _values = arr;

        public int Query(int left, int right, int threshold)
        {
            var counts = new Dictionary<int, int>();

            for (var i = left; i <= right; i++)
            {
                counts[_values[i]] = counts.GetValueOrDefault(_values[i]) + 1;
            }

            foreach (var (value, count) in counts)
            {
                if (count >= threshold)
                {
                    return value;
                }
            }

            return LeetCodeAnswer.None;
        }
    }

    // Groups each value's occurrence indices into this repo's own DynamicArray<int>
    // (naturally ascending, since built by one left-to-right construction pass), keyed
    // by value in a HashMap<int, DynamicArray<int>>. A query samples a bounded number
    // of candidate positions inside [left, right] and, for each one, counts how many of
    // that value's stored positions fall in range via BinarySearch.LowerBound/UpperBound
    // over a DynamicArraySequence<int> view onto the same DynamicArray - the same "wrap
    // an existing structure in an IRandomAccessSequence witness" idiom RangeModuleTests
    // (LC 715) already uses. Because the query guarantee makes any answer a true
    // majority of the subarray, a bounded number of samples finds it with overwhelming
    // probability, matching the problem's own official randomized approach. The seed is
    // fixed so the strategy is deterministic under test and under measurement.
    internal sealed class MajorityCheckerByPositionIndex : IMajorityChecker
    {
        private const int SampleAttempts = 20;
        private const int RandomSeed = 1157; // LC problem number

        private readonly int[] _values;
        private readonly HashMap<int, DynamicArray<int>> _positionsByValue = new();
        private readonly Random _random = new(RandomSeed);

        public MajorityCheckerByPositionIndex(int[] arr)
        {
            _values = arr;

            for (var i = 0; i < arr.Length; i++)
            {
                var positions = PositionsOf(arr[i]);
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

            return LeetCodeAnswer.None;
        }

        private DynamicArray<int> PositionsOf(int value)
        {
            if (_positionsByValue.TryGetValue(value, out var existing))
            {
                return existing;
            }

            var positions = new DynamicArray<int>();
            _positionsByValue.Set(value, positions);

            return positions;
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
