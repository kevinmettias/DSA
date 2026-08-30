using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAStackWithIncrementOperation;

// LeetCode 1381. Design a Stack With Increment Operation: composes two of this
// repo's own Stack<int> instances - one holding the live values (capped at
// maxSize via Count), one a scratch buffer - the same "compose two Stack<int>
// instances" move MinStackTests already makes for a running minimum, applied here
// to reach the bottom k elements Increment needs to touch. Stack<T>'s public
// surface is deliberately LIFO-only (Push/TryPop/TryPeek, no indexer), so
// Increment drains everything into scratch (whose own top-to-bottom order is then
// exactly the original bottom-to-top order), adds val to the first k popped back
// off scratch, and pushes the rest back unchanged.
public sealed partial class DesignAStackWithIncrementOperationTests
{
    [Fact]
    public void PushPopIncrement_SequenceOfOperations_RespectsMaxSizeAndIncrementsBottomK()
    {
        var stack = new CustomStackOperations(maxSize: 3);

        stack.Push(1);
        stack.Push(2);
        Assert.Equal(2, stack.Pop());

        stack.Push(2);
        stack.Push(3);
        stack.Push(4); // stack is already at maxSize (3) - no-op

        stack.Increment(k: 5, val: 100); // k exceeds size, so all 3 elements shift

        Assert.Equal(103, stack.Pop());
        Assert.Equal(102, stack.Pop());
        Assert.Equal(101, stack.Pop());
        Assert.Equal(-1, stack.Pop()); // empty stack pops -1, not an exception
    }

    [Fact]
    public void Increment_FewerElementsThanK_IncrementsEveryElement()
    {
        var stack = new CustomStackOperations(maxSize: 5);
        stack.Push(10);
        stack.Push(20);

        stack.Increment(k: 10, val: 5);

        Assert.Equal(25, stack.Pop());
        Assert.Equal(15, stack.Pop());
    }

    private sealed class CustomStackOperations(int maxSize)
    {
        private readonly RepoIntStack _values = new();

        public void Push(int value)
        {
            if (_values.Count < maxSize)
            {
                _values.Push(value);
            }
        }

        public int Pop() => _values.TryPop(out var value) ? value : -1;

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
}
