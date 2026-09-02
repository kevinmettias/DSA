using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using HullStack = DSAExperimentation.DataStructures.Stack.Stack<(int X, int Y)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Largest Triangle Area (LC 812): the textbook O(n^3) triple enumeration over
// every point vs. reducing the candidate set to the convex hull first (this
// repo's own MergeSort over ArrayIndexedSequence to sort by (X, Y), then
// Stack<(int,int)> for Andrew's monotone chain sweep - same ErectTheFenceTests/
// LC587 composition) before enumerating O(h^3) hull-only triples. Points are
// drawn uniformly from a bounded square, so almost all of them land strictly
// inside the hull and get discarded before the cubic step ever sees them.
[MemoryDiagnoser]
public class LargestTriangleAreaBenchmarks
{
    // LC 812.
    private const int RandomSeed = 812;
    private const int CoordinateUpperBound = 1_000;
    private const int MinHullVerticesForTriangle = 3;
    private const int MinStackSizeForCrossCheck = 2;
    private const double TriangleAreaDivisor = 2.0;

    [Params(60, 300)]
    public int Length;

    private (int X, int Y)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var seen = new HashSet<(int X, int Y)>();

        while (seen.Count < Length)
        {
            seen.Add((random.Next(0, CoordinateUpperBound), random.Next(0, CoordinateUpperBound)));
        }

        _points = [.. seen];
    }

    [Benchmark(Baseline = true)]
    public double BruteForceAllTriples() => LargestOver(_points);

    [Benchmark]
    public double ConvexHullReduction()
    {
        var hull = ConvexHull(_points);
        return LargestOver(hull.Count >= MinHullVerticesForTriangle ? [.. hull] : _points);
    }

    private static double LargestOver((int X, int Y)[] points)
    {
        var best = 0.0;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                for (var k = j + 1; k < points.Length; k++)
                {
                    var area = Area(points[i], points[j], points[k]);
                    best = Math.Max(best, area);
                }
            }
        }

        return best;
    }

    private static List<(int X, int Y)> ConvexHull((int X, int Y)[] points)
    {
        var sorted = points.ToArray();
        MergeSort.Sort<(int X, int Y), ArrayIndexedSequence<(int X, int Y)>>(
            new ArrayIndexedSequence<(int X, int Y)>(sorted),
            Comparer<(int X, int Y)>.Create((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y)));

        var lower = HalfHull(sorted);
        var upper = HalfHull(sorted.Reverse().ToArray());

        return [.. lower.Take(lower.Count - 1), .. upper.Take(upper.Count - 1)];
    }

    private static List<(int X, int Y)> HalfHull((int X, int Y)[] points)
    {
        var stack = BuildHullStack(points);
        var chain = DrainToChain(stack);

        chain.Reverse();
        return chain;
    }

    private static HullStack BuildHullStack((int X, int Y)[] points)
    {
        var stack = new HullStack();

        foreach (var p in points)
        {
            while (stack.Count >= MinStackSizeForCrossCheck)
            {
                stack.TryPop(out var top);
                stack.TryPeek(out var second);

                if (Cross(second, top, p) > 0)
                {
                    stack.Push(top);
                    break;
                }
            }

            stack.Push(p);
        }

        return stack;
    }

    private static List<(int X, int Y)> DrainToChain(HullStack stack)
    {
        var chain = new List<(int X, int Y)>();
        while (stack.TryPop(out var item))
        {
            chain.Add(item);
        }

        return chain;
    }

    private static double Area((int X, int Y) a, (int X, int Y) b, (int X, int Y) c)
    {
        var cross = Cross(a, b, c);
        return Math.Abs(cross) / TriangleAreaDivisor;
    }

    private static long Cross((int X, int Y) o, (int X, int Y) a, (int X, int Y) b)
        => (long)(a.X - o.X) * (b.Y - o.Y) - (long)(a.Y - o.Y) * (b.X - o.X);
}
