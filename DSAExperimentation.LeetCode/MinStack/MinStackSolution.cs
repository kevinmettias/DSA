using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MinStack;

// LeetCode 155. Min Stack: a stack that supports push/pop/top plus O(1)
// retrieval of the current minimum.
//
// This is a design problem - LeetCode's own shape is a stateful object with
// four operations, not a single return value - so the factories below are
// factories rather than pure functions, the same shape LRUCacheSolution uses for
// its own design problem (LC 146). The first composes two of this repo's own
// Stack<int> instances - one holding the real values, one shadowing the running
// minimum at each depth - the same "compose, don't invent a new representation"
// move Stack.cs itself makes over DynamicArray.
//
// The second arm is the textbook one this problem's write-ups start from: a
// single list of the values actually pushed, and a GetMin that re-scans all of
// them. It trades the shadow stack's O(1) minimum for O(n) per query and a
// smaller footprint per pushed element - which is the trade the benchmark
// measures. Both arms are driven through IMinStackOperations so LeetCode's
// four-call shape reaches them identically, the same move
// ApplyDiscountEveryNOrdersSolution makes for its own two cashiers.
internal static class MinStackSolution
{
    public static IMinStackOperations CreateByStackPrimitive() => new MinStackOperations();

    public static IMinStackOperations CreateBySingleListScan() => new MinStackScanOperations();

    // LeetCode's four operations, stated once so both arms can be replayed by the
    // same script.
    internal interface IMinStackOperations
    {
        void Push(int value);

        void Pop();

        int Top();

        int GetMin();
    }

    internal sealed class MinStackOperations : IMinStackOperations
    {
        private readonly RepoIntStack _values = new();
        private readonly RepoIntStack _minimums = new();

        public void Push(int value)
        {
            _values.Push(value);
            var currentMin = _minimums.TryPeek(out var min) ? Math.Min(min, value) : value;
            _minimums.Push(currentMin);
        }

        public void Pop()
        {
            _values.TryPop(out _);
            _minimums.TryPop(out _);
        }

        public int Top()
        {
            _values.TryPeek(out var value);
            return value;
        }

        public int GetMin()
        {
            _minimums.TryPeek(out var min);
            return min;
        }
    }

    internal sealed class MinStackScanOperations : IMinStackOperations
    {
        private readonly List<int> _values = [];

        public void Push(int value) => _values.Add(value);

        public void Pop() => _values.RemoveAt(_values.Count - 1);

        public int Top() => _values[^1];

        public int GetMin()
        {
            var minimum = _values[0];

            foreach (var value in _values)
            {
                minimum = Math.Min(minimum, value);
            }

            return minimum;
        }
    }
}
