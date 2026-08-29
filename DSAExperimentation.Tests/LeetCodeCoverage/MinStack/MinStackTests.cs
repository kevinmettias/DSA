using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinStack;

// LeetCode 155. Min Stack: composes two of this repo's Stack<int> instances - one
// holding the real values, one shadowing the running minimum at each depth - the
// same "compose, don't invent a new representation" move Stack.cs itself makes over
// DynamicArray.
public sealed partial class MinStackTests
{
    [Fact]
    public void PushPopTopGetMin_SequenceOfOperations_TracksRunningMinimum()
    {
        var stack = new MinStackOperations();

        stack.Push(-2);
        stack.Push(0);
        stack.Push(-3);
        Assert.Equal(-3, stack.GetMin());

        stack.Pop();
        Assert.Equal(0, stack.Top());
        Assert.Equal(-2, stack.GetMin());
    }

    private sealed class MinStackOperations
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
