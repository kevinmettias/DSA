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
    private const int RandomSeed = 587; // LC problem number
    private const int CoordinateBound = 1_000;
    private const int MinPointsForTurn = 2;

    [Params(50, 300)]
    public int Length;

    private (int X, int Y)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, Length)
            .Select(_ => (random.Next(0, CoordinateBound), random.Next(0, CoordinateBound)))
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
                AddHullLinePoints(i, j, fence);
            }
        }

        return fence.Count;
    }

    private void AddHullLinePoints(int i, int j, HashSet<(int X, int Y)> fence)
    {
        if (i == j)
        {
            return;
        }

        var a = _points[i];
        var b = _points[j];

        if (!IsHullLine(a, b))
        {
            return;
        }

        AddSegmentPoints(fence, a, b);
    }

    private void AddSegmentPoints(HashSet<(int X, int Y)> fence, (int X, int Y) a, (int X, int Y) b)
    {
        foreach (var p in _points)
        {
            if (IsOnSegment(a, b, p))
            {
                fence.Add(p);
            }
        }
    }

    private bool IsHullLine((int X, int Y) a, (int X, int Y) b)
    {
        var side = 0;

        foreach (var c in _points)
        {
            if (!TryUpdateSide(a, b, c, ref side))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUpdateSide((int X, int Y) a, (int X, int Y) b, (int X, int Y) c, ref int side)
    {
        var cross = Cross(a, b, c);

        if (cross == 0)
        {
            return true;
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

        return true;
    }

    [Benchmark]
    public int MonotoneChainThenEdgeScan()
    {
        var sorted = SortByCoordinates(_points);
        var corners = ComputeHullCorners(sorted);
        var fence = ScanEdgesForFencePoints(corners);

        return fence.Count;
    }

    private static (int X, int Y)[] SortByCoordinates((int X, int Y)[] points)
    {
        var sorted = points.ToArray();
        MergeSort.Sort<(int X, int Y), ArrayIndexedSequence<(int X, int Y)>>(
            new ArrayIndexedSequence<(int X, int Y)>(sorted),
            Comparer<(int X, int Y)>.Create((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y)));

        return sorted;
    }

    private static List<(int X, int Y)> ComputeHullCorners((int X, int Y)[] sorted)
    {
        var lower = StrictHalfHull(sorted);
        var upper = StrictHalfHull(sorted.Reverse().ToArray());

        return lower.Take(lower.Count - 1).Concat(upper.Take(upper.Count - 1)).ToList();
    }

    private HashSet<(int X, int Y)> ScanEdgesForFencePoints(List<(int X, int Y)> corners)
    {
        var fence = new HashSet<(int X, int Y)>();

        for (var i = 0; i < corners.Count; i++)
        {
            var a = corners[i];
            var b = corners[(i + 1) % corners.Count];

            AddSegmentPoints(fence, a, b);
        }

        return fence;
    }

    private static List<(int X, int Y)> StrictHalfHull((int X, int Y)[] points)
    {
        var stack = new HullStack();

        foreach (var p in points)
        {
            PopNonLeftTurns(stack, p);
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

    private static void PopNonLeftTurns(HullStack stack, (int X, int Y) p)
    {
        while (stack.Count >= MinPointsForTurn)
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
    }

    private static bool IsOnSegment((int X, int Y) a, (int X, int Y) b, (int X, int Y) p)
        => Cross(a, b, p) == 0
            && p.X >= Math.Min(a.X, b.X) && p.X <= Math.Max(a.X, b.X)
            && p.Y >= Math.Min(a.Y, b.Y) && p.Y <= Math.Max(a.Y, b.Y);

    private static long Cross((int X, int Y) o, (int X, int Y) a, (int X, int Y) b)
        => (long)(a.X - o.X) * (b.Y - o.Y) - (long)(a.Y - o.Y) * (b.X - o.X);
}
