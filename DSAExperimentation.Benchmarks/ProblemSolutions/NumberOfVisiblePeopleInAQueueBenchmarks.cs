using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Visible People in a Queue (LC 1944): the definition-literal O(n^2)
// forward scan (for each person, walk right tracking the tallest person seen so far
// between them and the candidate, per LC's own "min(heights[i], heights[j]) >=
// everyone between" rule) vs. a single O(n) right-to-left sweep through this repo's
// own monotonic Stack<int> (DailyTemperaturesBenchmarks precedent). Heights are a
// random permutation so no person's answer short-circuits the brute-force scan
// early.
[MemoryDiagnoser]
public class NumberOfVisiblePeopleInAQueueBenchmarks
{
    private const int RandomSeed = 1944; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan()
    {
        var n = _heights.Length;
        var result = new int[n];

        for (var i = 0; i < n; i++)
        {
            result[i] = CountVisibleFrom(i, n);
        }

        return result;
    }

    private int CountVisibleFrom(int i, int n)
    {
        var maxBetween = 0;
        var visible = 0;

        for (var j = i + 1; j < n; j++)
        {
            if (maxBetween <= _heights[i] && maxBetween <= _heights[j])
            {
                visible++;
            }

            if (_heights[j] >= _heights[i])
            {
                break;
            }

            maxBetween = Math.Max(maxBetween, _heights[j]);
        }

        return visible;
    }

    [Benchmark]
    public int[] MonotonicStackSweep()
    {
        var n = _heights.Length;
        var result = new int[n];
        var stack = new RepoIntStack();

        for (var i = n - 1; i >= 0; i--)
        {
            result[i] = CountVisibleAndAdvance(stack, i);
        }

        return result;
    }

    private int CountVisibleAndAdvance(RepoIntStack stack, int i)
    {
        var count = 0;

        while (stack.TryPeek(out var top) && top < _heights[i])
        {
            stack.TryPop(out _);
            count++;
        }

        if (stack.Count > 0)
        {
            count++;
        }

        while (stack.TryPeek(out var top) && top == _heights[i])
        {
            stack.TryPop(out _);
        }

        stack.Push(_heights[i]);
        return count;
    }
}
