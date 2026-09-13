using RepoDynamicArray = DSAExperimentation.DataStructures.DynamicArray.DynamicArray<string?>;

namespace DSAExperimentation.LeetCode.DesignAnOrderedStream;

// LeetCode 1656. Design an Ordered Stream: n values arrive keyed 1..n in arbitrary
// order, and each Insert returns the largest block of consecutive values that can be
// handed out starting at the read cursor - so a value arriving early emits nothing
// until the gap before it closes.
//
// That is a fixed-size slot array plus a cursor that only advances while consecutive
// slots are filled. LeetCode's own shape here is a stateful object with a
// constructor and one operation, not a single return value, so "every strategy for
// the problem" (ARCHITECTURE.md §17.3) takes the form of two full classes
// implementing the shared IOrderedStream surface below - the same shape
// DesignBrowserHistorySolution uses for its own instance-API problem (LC 1472).
internal static class DesignAnOrderedStreamSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one insertion script against either strategy without
    // restating it.
    internal interface IOrderedStream
    {
        List<string> Insert(int idKey, string value);
    }

    // The textbook baseline this composition has to justify itself against: a BCL
    // List<string?> pre-sized with n null slots plus a cursor, deliberately written
    // without this repo's DynamicArray<string?>.
    internal sealed class OrderedStreamByListBacked : IOrderedStream
    {
        private readonly List<string?> _values;
        private int _ptr;

        public OrderedStreamByListBacked(int n) => _values = new List<string?>(new string?[n]);

        public List<string> Insert(int idKey, string value)
        {
            _values[idKey - 1] = value;

            var chunk = new List<string>();

            while (_ptr < _values.Count && _values[_ptr] is { } filled)
            {
                chunk.Add(filled);
                _ptr++;
            }

            return chunk;
        }
    }

    // The composed answer: this repo's own DynamicArray<string?> (Add to lay out the
    // empty slots, Set to fill one, Get in the cursor walk) doing the same
    // pre-filled-slots-plus-cursor bookkeeping - the same "sequence + access
    // constraint" composition DesignBrowserHistorySolution already makes over
    // DynamicArray<T>, advancing on contiguous fill instead of truncating on write.
    internal sealed class OrderedStreamByDynamicArrayBacked : IOrderedStream
    {
        private readonly RepoDynamicArray _values = new();
        private int _ptr;

        public OrderedStreamByDynamicArrayBacked(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _values.Add(null);
            }
        }

        public List<string> Insert(int idKey, string value)
        {
            _values.Set(idKey - 1, value);

            var chunk = new List<string>();

            while (_ptr < _values.Count && _values.Get(_ptr) is { } filled)
            {
                chunk.Add(filled);
                _ptr++;
            }

            return chunk;
        }
    }
}
