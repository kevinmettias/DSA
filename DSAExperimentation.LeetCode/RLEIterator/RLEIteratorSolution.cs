using RepoRunQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Count, int Value)>;

namespace DSAExperimentation.LeetCode.RLEIterator;

// LeetCode 900. RLE Iterator: an encoding of (count, value) run pairs is exhausted
// by repeated next(n) calls, each consuming the next n elements of the decoded
// sequence and returning the last one consumed - or -1 if fewer than n remain.
//
// A design problem - LeetCode's own shape is a stateful object with a single
// next(n) operation, not one return value - so the strategy choice is which
// implementation backs it, the same shape PeekingIteratorSolution uses for LC 284.
//
// IRleIterator is bespoke to this problem alone, so it stays here rather than in
// DataStructures/.
internal static class RLEIteratorSolution
{
    // Two ints per run pair in LeetCode's flat encoding array.
    public const int ValuesPerRun = 2;

    // The textbook baseline: eagerly decompress the whole encoding into a flat
    // int[] and walk it with an index cursor - O(total element count) time and
    // memory up front, which on LeetCode's own constraints (individual run counts
    // up to 1e9) can be far larger than the encoding itself. Deliberately written
    // with nothing but the BCL.
    public static IRleIterator CreateByDecompressedArray(int[] encoding) => new DecompressedArrayRleIterator(encoding);

    // The composed answer: consume runs lazily from this repo's own FIFO
    // Queue<(int, int)>, tracking only the currently-in-progress run's remaining
    // count as instance state - O(number of runs) memory and O(total next-calls +
    // number of runs) time, since each run is dequeued at most once across the
    // whole query stream. A partially-consumed run is instance state rather than a
    // push-to-front, because Queue<T> deliberately offers no such operation.
    public static IRleIterator CreateByRunLengthQueue(int[] encoding) => new RunLengthQueueRleIterator(encoding);

    private sealed class DecompressedArrayRleIterator : IRleIterator
    {
        private readonly int[] _values;
        private int _index;

        public DecompressedArrayRleIterator(int[] encoding) => _values = Decompress(encoding);

        public int Next(int n)
        {
            var last = LeetCodeAnswer.None;

            while (n > 0 && _index < _values.Length)
            {
                last = _values[_index++];
                n--;
            }

            return n > 0 ? LeetCodeAnswer.None : last;
        }

        private static int[] Decompress(int[] encoding)
        {
            var totalCount = 0;

            for (var i = 0; i < encoding.Length; i += ValuesPerRun)
            {
                totalCount += encoding[i];
            }

            var values = new int[totalCount];
            var index = 0;

            for (var i = 0; i < encoding.Length; i += ValuesPerRun)
            {
                for (var repeat = 0; repeat < encoding[i]; repeat++)
                {
                    values[index++] = encoding[i + 1];
                }
            }

            return values;
        }
    }

    private sealed class RunLengthQueueRleIterator : IRleIterator
    {
        private readonly RepoRunQueue _runs = new();
        private int _remaining;
        private int _value;

        public RunLengthQueueRleIterator(int[] encoding)
        {
            for (var i = 0; i < encoding.Length; i += ValuesPerRun)
            {
                _runs.Enqueue((encoding[i], encoding[i + 1]));
            }
        }

        public int Next(int n)
        {
            while (n > 0)
            {
                if (_remaining == 0)
                {
                    if (!_runs.TryDequeue(out var run))
                    {
                        return LeetCodeAnswer.None;
                    }

                    _remaining = run.Count;
                    _value = run.Value;
                }

                var consumed = Math.Min(n, _remaining);
                _remaining -= consumed;
                n -= consumed;
            }

            return _value;
        }
    }
}

// The next(n) contract every strategy above implements. Bespoke to this problem:
// no other LeetCode entry shares this shape, so it stays here rather than in
// DataStructures/.
internal interface IRleIterator
{
    int Next(int n);
}
