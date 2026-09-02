using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MinStack;

// LeetCode 155. Min Stack: a stack that supports push/pop/top plus O(1)
// retrieval of the current minimum.
//
// This is a design problem - LeetCode's own shape is a stateful object with
// four operations, not a single return value - so CreateByStackPrimitive is a
// factory rather than a pure function, the same shape LRUCacheSolution uses for
// its own design problem (LC 146). It composes two of this repo's own
// Stack<int> instances - one holding the real values, one shadowing the
// running minimum at each depth - the same "compose, don't invent a new
// representation" move Stack.cs itself makes over DynamicArray. No benchmark
// existed for this problem to inventory a second, textbook-baseline arm from
// (bench: null), so only this one strategy is migrated here.
internal static class MinStackSolution
{
    public static MinStackOperations CreateByStackPrimitive() => new();

    internal sealed class MinStackOperations
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
}
