using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Daily Temperatures (LC 739): the canonical O(n^2) per-day forward scan vs. a single
// O(n) monotonic-decreasing sweep through this repo's own Stack<int> of pending day
// indices (NextGreaterElementIBenchmarks precedent) - each day is pushed once and
// popped at most once. Temperatures are a random permutation so no day's answer
// short-circuits the brute-force scan early.
[MemoryDiagnoser]
public class DailyTemperaturesBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _temperatures = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(739);
        _temperatures = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan()
    {
        var result = new int[_temperatures.Length];

        for (var day = 0; day < _temperatures.Length; day++)
        {
            for (var later = day + 1; later < _temperatures.Length; later++)
            {
                if (_temperatures[later] > _temperatures[day])
                {
                    result[day] = later - day;
                    break;
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int[] MonotonicStackSweep()
    {
        var result = new int[_temperatures.Length];
        var pendingIndices = new RepoIntStack();

        for (var day = 0; day < _temperatures.Length; day++)
        {
            while (pendingIndices.TryPeek(out var previousDay) && _temperatures[previousDay] < _temperatures[day])
            {
                pendingIndices.TryPop(out _);
                result[previousDay] = day - previousDay;
            }

            pendingIndices.Push(day);
        }

        return result;
    }
}
