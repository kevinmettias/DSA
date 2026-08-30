using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using HullStack = DSAExperimentation.DataStructures.Stack.Stack<(int X, int Y)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Erect the Fence (LC 587): the O(n^3) brute force - for every ordered pair of
// points, check whether every other point lies on one side of the line through
// them (or on it), and if so fold in every point that's collinear-and-between on
// that segment - vs. this repo's own MergeSort (over ArrayIndexedSequence) to
// sort by (x, y) followed by Andrew's monotone chain over this repo's own
// Stack<(int,int)> to build the O(h) strict hull corners, then the same
// collinearity/betweenness scan restricted to just those h edges instead of every
// O(n^2) pair. Points are uniform-random in a bounded grid, so the hull stays a
// small fraction of Length (h << n), which is exactly what makes the
// O(n log n + h*n) primitive-based approach beat brute force's O(n^3) so
// decisively.
[MemoryDiagnoser]
public class ErectTheFenceBenchmarks
{
    [Params(50, 300)]
    public int Length;

    private (int X, int Y)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(587);
        _points = Enumerable.Range(0, Length)
            .Select(_ => (random.Next(0, 1_000), random.Next(0, 1_000)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int EveryPairHalfPlaneScan()
    {
        var fence = new HashSet<(int X, int Y)>();

        for (var i = 0; i < _points.Length; i++)
        {
            for (var j = 0; j < _points.Length; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var a = _points[i];
                var b = _points[j];

                if (!IsHullLine(a, b))
                {
                    continue;
                }

                foreach (var c in _points)
                {
                    if (IsOnSegment(a, b, c))
                    {
                        fence.Add(c);
                    }
                }
            }
        }

        return fence.Count;
    }

    private bool IsHullLine((int X, int Y) a, (int X, int Y) b)
    {
        var side = 0;

        foreach (var c in _points)
        {
            var cross = Cross(a, b, c);

            if (cross == 0)
            {
                continue;
            }

            var thisSide = cross > 0 ? 1 : -1;

            if (side == 0)
            {
                side = thisSide;
            }
            else if (side != thisSide)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public int MonotoneChainThenEdgeScan()
    {
        var sorted = _points.ToArray();
        MergeSort.Sort<(int X, int Y), ArrayIndexedSequence<(int X, int Y)>>(
            new ArrayIndexedSequence<(int X, int Y)>(sorted),
            Comparer<(int X, int Y)>.Create((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y)));

        var lower = StrictHalfHull(sorted);
        var upper = StrictHalfHull(sorted.Reverse().ToArray());
        var corners = lower.Take(lower.Count - 1).Concat(upper.Take(upper.Count - 1)).ToList();

        var fence = new HashSet<(int X, int Y)>();
        for (var i = 0; i < corners.Count; i++)
        {
            var a = corners[i];
            var b = corners[(i + 1) % corners.Count];

            foreach (var p in _points)
            {
                if (IsOnSegment(a, b, p))
                {
                    fence.Add(p);
                }
            }
        }

        return fence.Count;
    }

    private static List<(int X, int Y)> StrictHalfHull((int X, int Y)[] points)
    {
        var stack = new HullStack();

        foreach (var p in points)
        {
            while (stack.Count >= 2)
            {
                stack.TryPop(out var top);
                stack.TryPeek(out var second);

                if (Cross(second, top, p) <= 0)
                {
                    continue;
                }

                stack.Push(top);
                break;
            }

            stack.Push(p);
        }

        var chain = new List<(int X, int Y)>();
        while (stack.TryPop(out var item))
        {
            chain.Add(item);
        }

        chain.Reverse();
        return chain;
    }

    private static bool IsOnSegment((int X, int Y) a, (int X, int Y) b, (int X, int Y) p)
        => Cross(a, b, p) == 0
            && p.X >= Math.Min(a.X, b.X) && p.X <= Math.Max(a.X, b.X)
            && p.Y >= Math.Min(a.Y, b.Y) && p.Y <= Math.Max(a.Y, b.Y);

    private static long Cross((int X, int Y) o, (int X, int Y) a, (int X, int Y) b)
        => (long)(a.X - o.X) * (b.Y - o.Y) - (long)(a.Y - o.Y) * (b.X - o.X);
}
