using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Frequency Stack (LC 895): a naive List<int>-backed stack that finds the
// value to pop by rescanning the whole list every time (recount every value's
// frequency, then walk back from the top for the most-recent max-frequency entry -
// O(n) per pop, O(n^2) over a full push/pop sequence) vs. this repo's own
// HashMap<TKey,TValue> tracking each value's running push frequency plus a second
// HashMap of frequency -> Stack<int> holding the values pushed at that frequency, so
// Pop is O(1) amortized - the same HashMap-plus-Stack composition
// MaximumFrequencyStackTests uses, exercised here over a longer, mixed push/pop
// workload instead of one fixed example sequence.
[MemoryDiagnoser]
public class MaximumFrequencyStackBenchmarks
{
    [Params(200, 3_000)]
    public int OperationCount;

    private (bool IsPush, int Value)[] _operations = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(7);
        var operations = new (bool IsPush, int Value)[OperationCount];
        var depth = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            // Push-biased so a pop always has something to remove - depth==0 forces
            // a push, otherwise pushes still outweigh pops, the same push-heavy
            // shape TopKFrequentElementsBenchmarks' bounded-range setup documents
            // for producing real frequency skew.
            var isPush = depth == 0 || random.NextDouble() < 0.6;
            operations[i] = (isPush, random.Next(0, 50));
            depth += isPush ? 1 : -1;
        }

        _operations = operations;
    }

    [Benchmark(Baseline = true)]
    public long RescanListOnEveryPop()
    {
        var values = new List<int>();
        long sum = 0;

        foreach (var (isPush, value) in _operations)
        {
            if (isPush)
            {
                values.Add(value);
                continue;
            }

            var counts = new Dictionary<int, int>();

            foreach (var v in values)
            {
                counts[v] = counts.GetValueOrDefault(v) + 1;
            }

            var maxFrequency = counts.Values.Max();
            var popIndex = values.Count - 1;

            while (counts[values[popIndex]] != maxFrequency)
            {
                popIndex--;
            }

            sum += values[popIndex];
            values.RemoveAt(popIndex);
        }

        return sum;
    }

    [Benchmark]
    public long HashMapAndStackByFrequency()
    {
        var frequencies = new HashMap<int, int>();
        var byFrequency = new HashMap<int, RepoStack>();
        var maxFrequency = 0;
        long sum = 0;

        foreach (var (isPush, value) in _operations)
        {
            if (isPush)
            {
                frequencies.TryGetValue(value, out var frequency);
                frequency++;
                frequencies.Set(value, frequency);
                maxFrequency = Math.Max(maxFrequency, frequency);

                if (!byFrequency.TryGetValue(frequency, out var group))
                {
                    group = new RepoStack();
                    byFrequency.Set(frequency, group);
                }

                group.Push(value);
                continue;
            }

            byFrequency.TryGetValue(maxFrequency, out var top);
            top!.TryPop(out var popped);
            sum += popped;

            frequencies.TryGetValue(popped, out var poppedFrequency);
            frequencies.Set(popped, poppedFrequency - 1);

            if (top.Count == 0)
            {
                maxFrequency--;
            }
        }

        return sum;
    }
}
