using DSAExperimentation.LeetCode.ErectTheFence;
using HullStack = DSAExperimentation.DataStructures.Stack.Stack<(int X, int Y)>;

namespace DSAExperimentation.LeetCode.LargestTriangleArea;

// LeetCode 812. Largest Triangle Area: the maximum area of any triangle formed by
// three of the given points.
//
// Both strategies answer with that area. They differ only in how large a candidate
// set the cubic triple enumeration runs over: the baseline enumerates every triple
// of every point, the composed strategy first reduces the points to their convex
// hull and enumerates only hull triples. The reduction is sound because the
// maximum-area triangle always has all three vertices on the hull - sliding an
// interior vertex outward along the direction perpendicular to the opposite side
// can only grow the triangle - so no candidate that could win is discarded.
//
// The hull is Andrew's monotone chain over this repo's own primitives, the same
// composition ErectTheFenceSolution (LC 587) uses: the (X, Y) order LC 587's own
// CoordinateOrder declares, then Stack<(int,int)> for the
// pop-while-not-a-left-turn sweep. The order is shared because it is the sweep's
// precondition; what each problem does with the swept chain is not, since LC 587
// needs the *boundary* points (corners plus every point collinear-and-between),
// while this one needs only the strict corners.
//
// A degenerate all-collinear input has no hull triangle at all - the sweep leaves
// fewer than three corners - so the composed strategy falls back to the original
// points, which all have zero area anyway, rather than enumerating an empty set.
internal static class LargestTriangleAreaSolution
{
    // A triangle needs three vertices; fewer hull corners than this means the
    // input is degenerate (collinear) and there is nothing to reduce to.
    private const int MinHullVerticesForTriangle = 3;

    // A cross product needs two points already on the chain plus the incoming one.
    private const int MinStackSizeForCrossCheck = 2;

    // The shoelace formula gives twice the triangle's area.
    private const double TriangleAreaDivisor = 2.0;

    // The textbook O(n^3) answer: every triple of points, keep the largest area.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static double LargestAreaByAllTriples((int X, int Y)[] points) => LargestOver(points);

    // Reduce the candidate set to the convex hull first, then run the same cubic
    // enumeration over just the hull vertices - O(n log n + h^3) instead of O(n^3),
    // and for points drawn from a filled region h is a small fraction of n.
    public static double LargestAreaByConvexHullReduction((int X, int Y)[] points)
    {
        var hull = ConvexHull(points);

        return LargestOver(hull.Count >= MinHullVerticesForTriangle ? HullVertices(hull) : points);
    }

    private static List<(int X, int Y)> ConvexHull((int X, int Y)[] points)
    {
        var sorted = CoordinateOrder.SortedByCoordinates(points);
        var lower = HalfHull(sorted);
        var upper = HalfHull(sorted.Reverse().ToArray());

        return [.. lower.Take(lower.Count - 1), .. upper.Take(upper.Count - 1)];
    }

    private static List<(int X, int Y)> HalfHull((int X, int Y)[] points)
    {
        var stack = BuildMonotoneStack(points);

        return DrainToChain(stack);
    }

    // Sweeps the (already X,Y-sorted) points left to right, popping any point that
    // would make the chain turn right or run straight, so only strict corners survive.
    private static HullStack BuildMonotoneStack((int X, int Y)[] points)
    {
        var stack = new HullStack();

        foreach (var p in points)
        {
            while (stack.Count >= MinStackSizeForCrossCheck && TryDiscardTrailingPoint(stack, p))
            {
            }

            stack.Push(p);
        }

        return stack;
    }

    private static bool TryDiscardTrailingPoint(HullStack stack, (int X, int Y) p)
    {
        stack.TryPop(out var top);
        stack.TryPeek(out var second);

        if (Cross(second, top, p) <= 0)
        {
            return true;
        }

        stack.Push(top);
        return false;
    }

    // Pops the stack (LIFO, so back-to-front relative to the sweep) into a list,
    // then reverses it back to the original left-to-right chain order.
    private static List<(int X, int Y)> DrainToChain(HullStack stack)
    {
        var chain = new List<(int X, int Y)>();

        while (stack.TryPop(out var item))
        {
            chain.Add(item);
        }

        chain.Reverse();
        return chain;
    }

    // The convex hull's own vertices, as the candidate array to enumerate.
    private static (int X, int Y)[] HullVertices(List<(int X, int Y)> hull) => [.. hull];

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

    private static double Area((int X, int Y) a, (int X, int Y) b, (int X, int Y) c)
    {
        var cross = Cross(a, b, c);

        return Math.Abs(cross) / TriangleAreaDivisor;
    }

    private static long Cross((int X, int Y) o, (int X, int Y) a, (int X, int Y) b)
        => (long)(a.X - o.X) * (b.Y - o.Y) - (long)(a.Y - o.Y) * (b.X - o.X);
}
