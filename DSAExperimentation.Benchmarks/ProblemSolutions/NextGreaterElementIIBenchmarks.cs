using BenchmarkDotNet.Attributes;
using NextGreaterStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Next Greater Element II (LC 503): the O(n^2) brute-force scan (checking up to
// n-1 positions ahead, wrapping around, for each index) vs. the O(n) monotonic-
// stack sweep over this repo's own Stack<int> - the same OneThreeTwoPattern/
// LargestRectangleInHistogram precedent, applied here to a circular array via the
// classic "walk the indices twice" (i % n) trick.
[MemoryDiagnoser]
public class NextGreaterElementIIBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(3);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce()
    {
        var n = _values.Length;
        var result = new int[n];

        for (var i = 0; i < n; i++)
        {
            result[i] = -1;

            for (var offset = 1; offset < n; offset++)
            {
                var candidate = _values[(i + offset) % n];

                if (candidate > _values[i])
                {
                    result[i] = candidate;
                    break;
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int[] MonotonicStack()
    {
        var n = _values.Length;
        var result = new int[n];
        Array.Fill(result, -1);
        var pendingIndices = new NextGreaterStack();

        for (var i = 0; i < 2 * n; i++)
        {
            var value = _values[i % n];

            while (pendingIndices.TryPeek(out var top) && _values[top] < value)
            {
                pendingIndices.TryPop(out _);
                result[top] = value;
            }

            if (i < n)
            {
                pendingIndices.Push(i);
            }
        }

        return result;
    }
}
