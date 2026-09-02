using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Boxes (LC 546): plain un-memoized interval recursion over (left, right,
// extra) triples - exponential, since the same triple recurs across many different
// choices of which inner sub-range gets cleared first - vs. this repo's own
// Memoizer<TState,TResult> caching that exact triple. BoxCount stays well under
// LeetCode's own limit (<=24, vs. LC's 100) specifically because the un-memoized
// baseline's blowup is real - a three-dimensional state space grows faster than
// BurstBalloonsBenchmarks' two-dimensional one, so its BalloonCount cap of 14
// would already be too slow here; 16 boxes is where the gap first clears
// measurement noise, and 24 is where it's dramatic.
[MemoryDiagnoser]
public class RemoveBoxesBenchmarks
{
    private const int MaxBoxColorValueExclusive = 4;

    [Params(16, 24)]
    public int BoxCount;

    private int[] _boxes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _boxes = Enumerable.Range(0, BoxCount).Select(_ => random.Next(1, MaxBoxColorValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Points(0, _boxes.Length - 1, 0);

    private int Points(int left, int right, int extra)
    {
        if (left > right)
        {
            return 0;
        }

        var best = Points(left, right - 1, 0) + (extra + 1) * (extra + 1);

        for (var i = left; i < right; i++)
        {
            if (_boxes[i] == _boxes[right])
            {
                best = Math.Max(best, Points(left, i, extra + 1) + Points(i + 1, right - 1, 0));
            }
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion()
    {
        return Memoizer.Memoize<(int Left, int Right, int Extra), int>((0, _boxes.Length - 1, 0), PointsMemoized);

        int PointsMemoized((int Left, int Right, int Extra) state, Func<(int Left, int Right, int Extra), int> points)
        {
            var (left, right, extra) = state;
            if (left > right)
            {
                return 0;
            }

            var best = points((left, right - 1, 0)) + (extra + 1) * (extra + 1);

            for (var i = left; i < right; i++)
            {
                if (_boxes[i] == _boxes[right])
                {
                    best = Math.Max(best, points((left, i, extra + 1)) + points((i + 1, right - 1, 0)));
                }
            }

            return best;
        }
    }
}
