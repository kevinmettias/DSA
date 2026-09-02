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
    private const int RandomSeed = 7;
    private const double PushProbability = 0.6;
    private const int ValueRange = 50;

    [Params(200, 3_000)]
    public int OperationCount;

    private (bool IsPush, int Value)[] _operations = null!;

    private readonly record struct FrequencyTracking(HashMap<int, int> Frequencies, HashMap<int, RepoStack> ByFrequency);

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var operations = new (bool IsPush, int Value)[OperationCount];
        var depth = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            // Push-biased so a pop always has something to remove - depth==0 forces
            // a push, otherwise pushes still outweigh pops, the same push-heavy
            // shape TopKFrequentElementsBenchmarks' bounded-range setup documents
            // for producing real frequency skew.
            var isPush = depth == 0 || random.NextDouble() < PushProbability;
            operations[i] = (isPush, random.Next(0, ValueRange));
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
            sum += ApplyRescanOperation(values, isPush, value);
        }

        return sum;
    }

    private static int ApplyRescanOperation(List<int> values, bool isPush, int value)
    {
        if (isPush)
        {
            values.Add(value);
            return 0;
        }

        var counts = CountFrequencies(values);
        var popIndex = FindMostRecentMaxFrequencyIndex(values, counts);
        var popped = values[popIndex];
        values.RemoveAt(popIndex);
        return popped;
    }

    private static Dictionary<int, int> CountFrequencies(List<int> values)
    {
        var counts = new Dictionary<int, int>();

        foreach (var v in values)
        {
            counts[v] = counts.GetValueOrDefault(v) + 1;
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

    [Benchmark]
    public long HashMapAndStackByFrequency()
    {
        var frequencies = new HashMap<int, int>();
        var byFrequency = new HashMap<int, RepoStack>();
        var tracking = new FrequencyTracking(frequencies, byFrequency);
        var maxFrequency = 0;
        long sum = 0;

        foreach (var (isPush, value) in _operations)
        {
            sum += ApplyFrequencyStackOperation(tracking, isPush, value, ref maxFrequency);
        }

        return sum;
    }

    private static long ApplyFrequencyStackOperation(FrequencyTracking tracking, bool isPush, int value, ref int maxFrequency)
    {
        var (frequencies, byFrequency) = tracking;

        if (isPush)
        {
            ApplyPush(frequencies, byFrequency, value, ref maxFrequency);
            return 0;
        }

        return ApplyPop(frequencies, byFrequency, ref maxFrequency);
    }

    private static void ApplyPush(HashMap<int, int> frequencies, HashMap<int, RepoStack> byFrequency, int value, ref int maxFrequency)
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
    }

    private static int ApplyPop(HashMap<int, int> frequencies, HashMap<int, RepoStack> byFrequency, ref int maxFrequency)
    {
        byFrequency.TryGetValue(maxFrequency, out var top);
        top!.TryPop(out var popped);

        frequencies.TryGetValue(popped, out var poppedFrequency);
        frequencies.Set(popped, poppedFrequency - 1);

        if (top.Count == 0)
        {
            maxFrequency--;
        }

        return popped;
    }
}
