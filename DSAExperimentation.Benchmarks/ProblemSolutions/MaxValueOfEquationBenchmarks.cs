using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Max Value of Equation (LC 1499): the O(n^2) all-pairs-within-k scan vs. the O(n)
// reduction to a SlidingWindowMaximum-shaped sweep - this repo's own Deque<int> holding
// indices in decreasing (y - x) order, evicting from the front once xj - xi exceeds k
// and from the back once a later point's (y - x) dominates. k is set wide enough that
// almost every pair stays in-window, forcing BOTH strategies through close to their full
// O(n^2)/O(n) shapes instead of an early window-shrink making brute force look
// artificially competitive.
[MemoryDiagnoser]
public class MaxValueOfEquationBenchmarks
{
    private const int K = 1_000_000;

    private const int MaxXStep = 5;

    private const int YCoordinateRange = 1_000;

    [Params(500, 4_000)]
    public int Length;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var x = 0;
        _points = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                x += random.Next(1, MaxXStep);
                return new[] { x, random.Next(-YCoordinateRange, YCoordinateRange) };
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int AllPairsScan()
    {
        var best = int.MinValue;
        for (var i = 0; i < _points.Length; i++)
        {
            for (var j = i + 1; j < _points.Length; j++)
            {
                if (_points[j][0] - _points[i][0] > K)
                {
                    break;
                }

                var value = _points[i][1] + _points[j][1] + _points[j][0] - _points[i][0];
                best = Math.Max(best, value);
            }
        }

        return best;
    }

    [Benchmark]
    public int MonotonicDeque()
    {
        var window = new RepoDeque();
        var best = int.MinValue;

        for (var j = 0; j < _points.Length; j++)
        {
            best = AdvanceWindow(window, j, best);
        }

        return best;
    }

    private int AdvanceWindow(RepoDeque window, int j, int best)
    {
        var (x, y) = (_points[j][0], _points[j][1]);

        while (window.TryPeekFront(out var frontIndex) && x - _points[frontIndex][0] > K)
        {
            window.TryPopFront(out _);
        }

        if (window.TryPeekFront(out var bestIndex))
        {
            best = Math.Max(best, x + y + _points[bestIndex][1] - _points[bestIndex][0]);
        }

        while (window.TryPeekBack(out var backIndex) && _points[backIndex][1] - _points[backIndex][0] <= y - x)
        {
            window.TryPopBack(out _);
        }

        window.PushBack(j);

        return best;
    }
}
