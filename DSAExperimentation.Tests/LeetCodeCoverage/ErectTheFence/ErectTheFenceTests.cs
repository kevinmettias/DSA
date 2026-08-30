using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using HullStack = DSAExperimentation.DataStructures.Stack.Stack<(int X, int Y)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ErectTheFence;

// LeetCode 587. Erect the Fence: sort trees with this repo's own MergeSort over
// ArrayIndexedSequence, then sweep them with Andrew's monotone chain to build the
// convex hull's CORNER vertices via this repo's own Stack<(int,int)> - pop while
// the last two points on the stack and the next point don't turn strictly left
// (discarding collinear points too), pushing the popped point back once a left
// turn is found - the same pop-while/push-back-once shape
// LargestRectangleInHistogramTests already uses for a height comparison, applied
// here to a cross-product turn test instead. Unlike a textbook convex hull, this
// problem also needs every tree that sits exactly ON an edge (not just at a
// corner) in the fence, so a second pass checks every original point for
// collinearity-and-betweenness against each strict-corner edge and folds any hit
// back in - recovering exactly the collinear boundary points the strict chain
// above deliberately dropped.
public sealed partial class ErectTheFenceTests
{
    [Fact]
    public void OuterTrees_ClassicExample_ExcludesInteriorPoint()
    {
        (int X, int Y)[] points = [(1, 1), (2, 2), (2, 0), (2, 4), (3, 3), (4, 2)];

        var fence = OuterTrees(points);

        Assert.Equal(
            new HashSet<(int X, int Y)> { (1, 1), (2, 0), (4, 2), (3, 3), (2, 4) },
            new HashSet<(int X, int Y)>(fence));
    }

    [Fact]
    public void OuterTrees_AllPointsCollinear_IncludesEveryPoint()
    {
        (int X, int Y)[] points = [(1, 2), (2, 2), (4, 2)];

        var fence = OuterTrees(points);

        Assert.Equal(
            new HashSet<(int X, int Y)> { (1, 2), (2, 2), (4, 2) },
            new HashSet<(int X, int Y)>(fence));
    }

    private static List<(int X, int Y)> OuterTrees((int X, int Y)[] points)
    {
        if (points.Length < 3)
        {
            return points.ToList();
        }

        var sorted = points.ToArray();
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

            foreach (var p in points)
            {
                if (IsOnSegment(a, b, p))
                {
                    fence.Add(p);
                }
            }
        }

        return fence.ToList();
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
