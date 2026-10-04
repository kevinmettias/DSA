using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

// nums2 cut into blocks of about sqrt(n) consecutive indices. Each block keeps a count of its
// values by value and one pending addition that every value in it owes; a stored value is the
// element's current value minus its block's pending addition. A range-add walks, index by index,
// only the at most two blocks it cuts through, and bumps the pending addition of every block it
// covers whole - O(sqrt n). "How many elements equal v" asks each block for v minus its pending
// addition - O(sqrt n) lookups.
//
// That is the structure LC 3943 needs and the core library does not have: its Fenwick and segment
// trees aggregate by index (a sum or minimum over a range), and none counts elements by value
// under a range shift. It lives in this folder because this problem is its only user
// (ARCHITECTURE 17.3).
internal sealed class ShiftedValueBlocks
{
    private readonly long[] _stored;
    private readonly long[] _pending;
    private readonly HashMap<long, int>[] _countsByValue;
    private readonly int _blockSize;

    public ShiftedValueBlocks(int[] values)
    {
        _blockSize = Math.Max(1, (int)Math.Sqrt(values.Length));
        var blockCount = (values.Length + _blockSize - 1) / _blockSize;
        _stored = Array.ConvertAll(values, value => (long)value);
        _pending = new long[blockCount];
        _countsByValue = new HashMap<long, int>[blockCount];

        for (var block = 0; block < blockCount; block++)
        {
            _countsByValue[block] = new HashMap<long, int>();
        }

        for (var index = 0; index < _stored.Length; index++)
        {
            Increment(_countsByValue[index / _blockSize], _stored[index]);
        }
    }

    // Adds delta to every element in [left, right].
    public void AddRange(int left, int right, long delta)
    {
        var firstBlock = left / _blockSize;
        var lastBlock = right / _blockSize;

        if (firstBlock == lastBlock)
        {
            AddEach(left, right, delta);
            return;
        }

        AddEach(left, ((firstBlock + 1) * _blockSize) - 1, delta);

        for (var block = firstBlock + 1; block < lastBlock; block++)
        {
            _pending[block] += delta;
        }

        AddEach(lastBlock * _blockSize, right, delta);
    }

    // [left, right] lies inside one block: move each element's count to its new stored value.
    private void AddEach(int left, int right, long delta)
    {
        var counts = _countsByValue[left / _blockSize];

        for (var index = left; index <= right; index++)
        {
            Decrement(counts, _stored[index]);
            _stored[index] += delta;
            Increment(counts, _stored[index]);
        }
    }

    // A value whose last element leaves the block leaves the map too, so a block's map never holds
    // more entries than the block has elements.
    private static void Decrement(HashMap<long, int> counts, long value)
    {
        counts.TryGetValue(value, out var count);

        if (count == 1)
        {
            counts.TryRemove(value);
        }
        else
        {
            counts.Set(value, count - 1);
        }
    }

    private static void Increment(HashMap<long, int> counts, long value)
    {
        counts.TryGetValue(value, out var count);
        counts.Set(value, count + 1);
    }

    // How many elements currently equal value.
    public int CountEqual(long value)
    {
        var count = 0;

        for (var block = 0; block < _pending.Length; block++)
        {
            if (_countsByValue[block].TryGetValue(value - _pending[block], out var inBlock))
            {
                count += inBlock;
            }
        }

        return count;
    }
}
