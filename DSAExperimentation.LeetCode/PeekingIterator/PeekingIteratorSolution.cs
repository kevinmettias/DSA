using RepoIntQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.PeekingIterator;

// LeetCode 284. Peeking Iterator: wrap a sequence with Peek() - look without
// consuming - alongside the usual HasNext()/Next() pair.
//
// A design problem - LeetCode's own shape is a stateful object with
// HasNext/Peek/Next operations, not a single return value - so the strategy
// choice is which implementation backs it, the same shape
// FindMedianFromDataStreamSolution uses for its own two-strategy design
// problem (LC 295).
//
// IPeekingIterator is bespoke to this problem alone - no other LeetCode entry
// shares a HasNext/Peek/Next contract - so it stays here rather than in
// DataStructures/.
internal static class PeekingIteratorSolution
{
    // The textbook baseline this composition has to justify itself against: a
    // raw index into the backing list plus a "have I already peeked" flag and
    // buffered value. Deliberately written without this repo's primitives.
    public static IPeekingIterator CreateByIndexTracked(IReadOnlyList<int> source) => new IndexTrackedPeekingIterator(source);

    // The composed answer: this repo's own Queue<int>, whose TryPeek (look
    // without consuming) and TryDequeue (look and consume) already are
    // exactly the pair the problem asks for - no hand-rolled buffering state
    // left to write.
    public static IPeekingIterator CreateByQueuePrimitive(IEnumerable<int> source) => new QueuePrimitivePeekingIterator(source);

    private sealed class IndexTrackedPeekingIterator(IReadOnlyList<int> source) : IPeekingIterator
    {
        private int _index;
        private bool _havePeeked;
        private int _peeked;

        public bool HasNext() => _havePeeked || _index < source.Count;

        public int Peek()
        {
            if (!_havePeeked)
            {
                _peeked = source[_index++];
                _havePeeked = true;
            }

            return _peeked;
        }

        public int Next()
        {
            var value = Peek();
            _havePeeked = false;
            return value;
        }
    }

    private sealed class QueuePrimitivePeekingIterator : IPeekingIterator
    {
        private readonly RepoIntQueue _items = new();

        public QueuePrimitivePeekingIterator(IEnumerable<int> source)
        {
            foreach (var value in source)
            {
                _items.Enqueue(value);
            }
        }

        public bool HasNext() => _items.Count > 0;

        public int Peek()
        {
            _items.TryPeek(out var value);
            return value;
        }

        public int Next()
        {
            _items.TryDequeue(out var value);
            return value;
        }
    }
}

// The HasNext/Peek/Next contract every strategy above implements. Bespoke to
// this problem: no other LeetCode entry shares this shape, so it stays here
// rather than in DataStructures/.
internal interface IPeekingIterator
{
    bool HasNext();

    int Peek();

    int Next();
}
