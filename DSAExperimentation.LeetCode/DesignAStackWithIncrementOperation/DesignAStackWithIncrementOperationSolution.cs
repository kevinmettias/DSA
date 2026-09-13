using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.DesignAStackWithIncrementOperation;

// LeetCode 1381. Design a Stack With Increment Operation: a stack capped at
// maxSize, whose Push is a no-op once full, whose Pop reports -1 on empty, and
// whose Increment adds val to the bottom min(k, size) elements.
//
// This is a design problem - LeetCode's own shape is a stateful object with three
// operations, not a single return value - so each strategy is a factory rather
// than a pure function, the same CreateBy<Strategy> shape MinStackSolution and
// MaximumFrequencyStackSolution use for their own design problems.
//
// CreateByStackDrain composes two of this repo's own Stack<int> instances - one
// holding the live values, one a scratch buffer - the same "compose two Stack<int>
// instances" move MinStackSolution makes for LC 155. Stack<T>'s public surface is
// deliberately LIFO-only (Push/TryPop/TryPeek, no indexer - see Stack.cs's own doc
// comment), so reaching the bottom k elements means draining everything into
// scratch, whose top-to-bottom order is then exactly the original bottom-to-top
// order, adding val to the first k popped back off it, and pushing the rest back
// unchanged: O(size) per Increment regardless of k.
//
// CreateByIndexedList is the textbook baseline that composition has to justify
// itself against: a BCL List<int> whose indexer reaches the bottom slots directly,
// so Increment touches exactly min(k, size) of them, O(k) per call. Its internals
// are deliberately BCL only; it is what you would write without this repo. Before
// this migration it lived as untested scaffolding inside
// DesignAStackWithIncrementOperationBenchmarks - and not even as a stack: the
// [Benchmark(Baseline = true)] arm was a bare increment loop over a List<int> with
// no maxSize cap, no Push and no Pop at all, so nothing asserted the arm the
// composed strategy is measured against. It is promoted here to LeetCode's real
// CustomStack surface, which is what finally gets it under test.
internal static class DesignAStackWithIncrementOperationSolution
{
    // LeetCode's own "pop from an empty stack" convention for this problem.
    private const int EmptyPop = -1;

    public static ICustomStack CreateByStackDrain(int maxSize) => new StackDrainCustomStack(maxSize);

    public static ICustomStack CreateByIndexedList(int maxSize) => new IndexedListCustomStack(maxSize);

    // LeetCode's CustomStack class surface.
    internal interface ICustomStack
    {
        void Push(int value);

        int Pop();

        void Increment(int k, int val);
    }

    private sealed class StackDrainCustomStack(int maxSize) : ICustomStack
    {
        private readonly RepoIntStack _values = new();

        public void Push(int value)
        {
            if (_values.Count < maxSize)
            {
                _values.Push(value);
            }
        }

        public int Pop() => _values.TryPop(out var value) ? value : EmptyPop;

        public void Increment(int k, int val)
        {
            var scratch = new RepoIntStack();

            while (_values.TryPop(out var item))
            {
                scratch.Push(item);
            }

            var affected = Math.Min(k, scratch.Count);

            for (var i = 0; i < affected; i++)
            {
                scratch.TryPop(out var item);
                _values.Push(item + val);
            }

            while (scratch.TryPop(out var item))
            {
                _values.Push(item);
            }
        }
    }

    private sealed class IndexedListCustomStack(int maxSize) : ICustomStack
    {
        private readonly List<int> _values = [];

        public void Push(int value)
        {
            if (_values.Count < maxSize)
            {
                _values.Add(value);
            }
        }

        public int Pop()
        {
            if (_values.Count == 0)
            {
                return EmptyPop;
            }

            var value = _values[^1];
            _values.RemoveAt(_values.Count - 1);
            return value;
        }

        public void Increment(int k, int val)
        {
            var affected = Math.Min(k, _values.Count);

            for (var i = 0; i < affected; i++)
            {
                _values[i] += val;
            }
        }
    }
}
