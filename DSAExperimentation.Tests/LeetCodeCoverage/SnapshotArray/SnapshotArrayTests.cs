using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SnapshotArray;

// LeetCode 1146. Snapshot Array: per-index parallel DynamicArray<int> histories
// (SnapIds/Values), the same "append on write, binary-search on read" shape
// TimeBasedKeyValueStoreTests already uses for its (key, timestamp) -> value floor
// query - here it's (index, snapId) -> value instead. Set appends the current,
// not-yet-snapshotted snap id; Snap just hands out and increments a counter, no
// per-call copy of the whole array. Get floors to the largest snap id <= the query
// via this repo's own BinarySearch.UpperBound over a DynamicArraySequence<int>.
public sealed partial class SnapshotArrayTests
{
    [Fact]
    public void SetSnapGet_LeetCodeExampleSequence_ReturnsValueAtSnapshotTime()
    {
        var snapshotArray = new SnapshotArray(3);

        snapshotArray.Set(0, 5);
        var snapId = snapshotArray.Snap();
        snapshotArray.Set(0, 6);

        Assert.Equal(0, snapId);

        var valueAtSnapshot = snapshotArray.Get(0, snapId);
        Assert.Equal(5, valueAtSnapshot);
    }

    [Fact]
    public void Get_IndexNeverSetBeforeQueriedSnapshot_ReturnsZero()
    {
        var snapshotArray = new SnapshotArray(3);
        var snapId = snapshotArray.Snap();
        snapshotArray.Set(1, 9);

        var valueBeforeAnySet = snapshotArray.Get(1, snapId);
        Assert.Equal(0, valueBeforeAnySet);
    }

    [Fact]
    public void Get_MultipleSnapshotsSameIndex_ReturnsValueFromRequestedSnapshot()
    {
        var snapshotArray = new SnapshotArray(1);

        snapshotArray.Set(0, 1);
        var first = snapshotArray.Snap();
        snapshotArray.Set(0, 2);
        var second = snapshotArray.Snap();
        snapshotArray.Set(0, 3);
        var third = snapshotArray.Snap();

        var valueAtFirst = snapshotArray.Get(0, first);
        Assert.Equal(1, valueAtFirst);

        var valueAtSecond = snapshotArray.Get(0, second);
        Assert.Equal(2, valueAtSecond);

        var valueAtThird = snapshotArray.Get(0, third);
        Assert.Equal(3, valueAtThird);
    }

    private sealed class SnapshotArray
    {
        private readonly History[] _histories;
        private int _snapId;

        public SnapshotArray(int length)
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
                return 0;
            }

            var sequence = new DynamicArraySequence<int>(history.SnapIds);
            var floorIndex = BinarySearch.UpperBound(sequence, snapId) - 1;

            return floorIndex < 0 ? 0 : history.Values.Get(floorIndex);
        }

        private sealed class History
        {
            public DynamicArray<int> SnapIds { get; } = new();
            public DynamicArray<int> Values { get; } = new();
        }
    }
}
