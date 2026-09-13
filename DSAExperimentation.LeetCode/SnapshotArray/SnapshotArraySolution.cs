using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SnapshotArray;

// LeetCode 1146. Snapshot Array: SnapshotArray(length) starts as length zeroes,
// Set(index, val) writes a value, Snap() takes a snapshot and returns its id
// (0-based, incrementing), and Get(index, snapId) reports what index held at the
// moment that snapshot was taken.
//
// This is a "design" problem - the published interface is a stateful object
// replayed across a call script, not a single pure function - so the strategies
// here are factory methods returning that stateful object, the same
// CreateBy<Strategy> shape TimeBasedKeyValueStoreSolution uses for its TimeMap.
//
// Neither strategy copies the array on Snap: Snap just hands out and increments a
// counter, and each index keeps only the (snapId, value) pairs actually written to
// it. Get is then a floor query - the value recorded at the largest snap id <= the
// one asked for - which is the same (key, timestamp) -> value lookup
// TimeBasedKeyValueStoreSolution answers, with the index standing in for the key.
//
// CreateByLinearFloorScan walks an index's whole history on every Get: O(n) per
// query. CreateByBinarySearchFloor keeps that history in parallel
// DynamicArray<int> columns and floors to the query with this repo's own
// BinarySearch.UpperBound over a DynamicArraySequence<int> witness, minus one: the
// index just past the last value written at or before snapId. O(log n) per query.
internal static class SnapshotArraySolution
{
    // The value every index holds before anything is written to it, and therefore
    // what a Get floored past the start of an index's history reports.
    private const int UnwrittenValue = 0;

    // Real answer: parallel history columns plus BinarySearch.UpperBound.
    public static ISnapshotArray CreateByBinarySearchFloor(int length) =>
        new BinarySearchFloorSnapshotArray(length);

    // The textbook answer: append to a flat list per index and scan it forward
    // until the snap ids overshoot the query. Deliberately written without this
    // repo's primitives - it is the arm the composed strategy above has to justify
    // itself against.
    public static ISnapshotArray CreateByLinearFloorScan(int length) =>
        new LinearFloorScanSnapshotArray(length);

    public interface ISnapshotArray
    {
        void Set(int index, int val);

        int Snap();

        int Get(int index, int snapId);
    }

    private sealed class BinarySearchFloorSnapshotArray : ISnapshotArray
    {
        private readonly History[] _histories;
        private int _snapId;

        public BinarySearchFloorSnapshotArray(int length)
        {
            _histories = new History[length];

            for (var i = 0; i < length; i++)
            {
                _histories[i] = new History();
            }
        }

        public void Set(int index, int val)
        {
            _histories[index].SnapIds.Add(_snapId);
            _histories[index].Values.Add(val);
        }

        public int Snap() => _snapId++;

        public int Get(int index, int snapId)
        {
            var history = _histories[index];

            if (history.SnapIds.Count == 0)
            {
                return UnwrittenValue;
            }

            var sequence = new DynamicArraySequence<int>(history.SnapIds);
            var floorIndex = BinarySearch.UpperBound(sequence, snapId) - 1;

            return floorIndex < 0 ? UnwrittenValue : history.Values.Get(floorIndex);
        }

        private sealed class History
        {
            public DynamicArray<int> SnapIds { get; } = new();

            public DynamicArray<int> Values { get; } = new();
        }
    }

    private sealed class LinearFloorScanSnapshotArray : ISnapshotArray
    {
        private readonly List<(int SnapId, int Value)>[] _histories;
        private int _snapId;

        public LinearFloorScanSnapshotArray(int length)
        {
            _histories = new List<(int SnapId, int Value)>[length];

            for (var i = 0; i < length; i++)
            {
                _histories[i] = [];
            }
        }

        public void Set(int index, int val) => _histories[index].Add((_snapId, val));

        public int Snap() => _snapId++;

        public int Get(int index, int snapId)
        {
            var floorValue = UnwrittenValue;

            foreach (var (recorded, value) in _histories[index])
            {
                if (recorded > snapId)
                {
                    break;
                }

                floorValue = value;
            }

            return floorValue;
        }
    }
}
