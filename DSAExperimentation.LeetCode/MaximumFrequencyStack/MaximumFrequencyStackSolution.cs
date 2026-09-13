using DSAExperimentation.DataStructures.HashMap;

using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaximumFrequencyStack;

// LeetCode 895. Maximum Frequency Stack: a stack whose Pop removes the most
// frequently pushed value, breaking ties toward the value pushed most recently.
//
// This is a design problem - LeetCode's own shape is a stateful object with two
// operations, not a single return value - so each strategy is a factory rather
// than a pure function, the same CreateBy<Strategy> shape
// AllOneDataStructureSolution/MinStackSolution use for their own design problems.
//
// CreateByHashMapAndStack composes this repo's own HashMap<TKey,TValue> (value ->
// current push frequency, and frequency -> a Stack<int> of the values pushed at
// that frequency) with Stack<int> itself: Pop always takes from the stack recorded
// at the current maximum frequency, so LIFO order within that stack breaks ties
// toward the most recently pushed value for free, and both operations are O(1)
// amortized. That is the same "compose two existing primitives, don't invent a new
// structure" move MinStackSolution makes for LC 155.
//
// CreateByListRescan is the textbook baseline this composition has to justify
// itself against: a plain List<int> holding the pushes in order, with every Pop
// recounting every value's frequency and then walking back from the top for the
// most recent maximum-frequency entry - O(n) per pop, O(n^2) over a full sequence.
// Its internals are deliberately BCL only; it is what you would write without this
// repo. Before this migration it lived as untested scaffolding inside
// MaximumFrequencyStackBenchmarks, so nothing asserted the arm the composed
// strategy is measured against.
internal static class MaximumFrequencyStackSolution
{
    public static IFreqStack CreateByHashMapAndStack() => new HashMapAndStackFreqStack();

    public static IFreqStack CreateByListRescan() => new ListRescanFreqStack();

    // LeetCode's FreqStack class surface: push a value, pop the most frequent one.
    internal interface IFreqStack
    {
        void Push(int value);

        int Pop();
    }

    private sealed class HashMapAndStackFreqStack : IFreqStack
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

    private sealed class ListRescanFreqStack : IFreqStack
    {
        private readonly List<int> _values = [];

        public void Push(int value) => _values.Add(value);

        public int Pop()
        {
            var counts = CountFrequencies(_values);
            var popIndex = FindMostRecentMaxFrequencyIndex(_values, counts);
            var popped = _values[popIndex];
            _values.RemoveAt(popIndex);

            return popped;
        }

        private static Dictionary<int, int> CountFrequencies(List<int> values)
        {
            var counts = new Dictionary<int, int>();

            foreach (var value in values)
            {
                counts[value] = counts.GetValueOrDefault(value) + 1;
            }

            return counts;
        }

        private static int FindMostRecentMaxFrequencyIndex(List<int> values, Dictionary<int, int> counts)
        {
            var maxFrequency = counts.Values.Max();
            var popIndex = values.Count - 1;

            while (counts[values[popIndex]] != maxFrequency)
            {
                popIndex--;
            }

            return popIndex;
        }
    }
}
