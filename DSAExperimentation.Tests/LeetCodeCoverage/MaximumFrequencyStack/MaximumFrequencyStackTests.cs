using DSAExperimentation.DataStructures.HashMap;

using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumFrequencyStack;

// LeetCode 895. Maximum Frequency Stack: composes this repo's own HashMap<TKey,TValue>
// (value -> current push frequency, and frequency -> a Stack<int> of values pushed at
// that frequency) with Stack<int> itself - Pop always takes from the stack recorded
// at the current max frequency, so LIFO order within that stack naturally breaks
// ties toward the most recently pushed value - the same "compose two existing
// primitives, don't invent a new structure" move MinStackTests already makes for
// LeetCode 155.
public sealed partial class MaximumFrequencyStackTests
{
    [Fact]
    public void PushPop_ClassicExample_PopsMostFrequentThenMostRecent()
    {
        var freqStack = new FreqStackOperations();

        freqStack.Push(5);
        freqStack.Push(7);
        freqStack.Push(5);
        freqStack.Push(7);
        freqStack.Push(4);
        freqStack.Push(5);

        Assert.Equal(5, freqStack.Pop());
        Assert.Equal(7, freqStack.Pop());
        Assert.Equal(5, freqStack.Pop());
        Assert.Equal(4, freqStack.Pop());
    }

    [Fact]
    public void Pop_SingleValuePushedOnce_ReturnsThatValue()
    {
        var freqStack = new FreqStackOperations();
        freqStack.Push(42);

        Assert.Equal(42, freqStack.Pop());
    }

    private sealed class FreqStackOperations
    {
        private readonly HashMap<int, int> _frequencies = new();
        private readonly HashMap<int, RepoStack> _byFrequency = new();
        private int _maxFrequency;

        public void Push(int value)
        {
            _frequencies.TryGetValue(value, out var frequency);
            frequency++;
            _frequencies.Set(value, frequency);
            _maxFrequency = Math.Max(_maxFrequency, frequency);

            if (!_byFrequency.TryGetValue(frequency, out var group))
            {
                group = new RepoStack();
                _byFrequency.Set(frequency, group);
            }

            group.Push(value);
        }

        public int Pop()
        {
            _byFrequency.TryGetValue(_maxFrequency, out var group);
            group!.TryPop(out var value);

            _frequencies.TryGetValue(value, out var frequency);
            _frequencies.Set(value, frequency - 1);

            if (group.Count == 0)
            {
                _maxFrequency--;
            }

            return value;
        }
    }
}
