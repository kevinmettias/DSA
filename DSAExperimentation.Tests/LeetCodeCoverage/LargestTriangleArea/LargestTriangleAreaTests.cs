using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using HullStack = DSAExperimentation.DataStructures.Stack.Stack<(int X, int Y)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestTriangleArea;

// LeetCode 812. Largest Triangle Area: the maximum-area triangle over any point set
// always has all three vertices on the convex hull - moving an interior point
// outward to the hull boundary can only grow (never shrink) the triangle it forms
// with any two other points - so this reduces the textbook O(n^3) triple
// enumeration down to just the hull vertices first. Same Andrew's monotone chain
// composition ErectTheFenceTests (LC 587) already uses - MergeSort over
// ArrayIndexedSequence to sort by (X, Y), then Stack<(int,int)> for the
// pop-while-not-a-left-turn sweep - applied here to shrink the candidate set
// instead of building a fence. Degenerate all-collinear inputs collapse the hull
// below 3 vertices (every triangle has zero area anyway), so that case falls back
// to the original points instead of leaving nothing to enumerate.
public sealed partial class LargestTriangleAreaTests
{
    [Fact]
    public void LargestArea_ClassicExample_ReturnsExpectedArea()
    {
        int[][] points = [[0, 0], [0, 1], [1, 0], [0, 2], [2, 0]];

        var area = LargestArea(points);

        Assert.Equal(2.0, area, 5);
    }

    [Fact]
    public void LargestArea_AllPointsCollinear_FallsBackToOriginalPoints()
    {
        int[][] points = [[0, 0], [1, 0], [2, 0]];

        var area = LargestArea(points);

        Assert.Equal(0.0, area, 5);
    }

    private static double LargestArea(int[][] points)
    {
        var pts = points.Select(p => (X: p[0], Y: p[1])).ToArray();
        var hull = ConvexHull(pts);
        var candidates = hull.Count >= 3 ? hull : pts.ToList();

        var best = 0.0;

        for (var i = 0; i < candidates.Count; i++)
        {
            for (var j = i + 1; j < candidates.Count; j++)
            {
                for (var k = j + 1; k < candidates.Count; k++)
                {
                    best = Math.Max(best, Area(candidates[i], candidates[j], candidates[k]));
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
        var stack = new HullStack();

        foreach (var p in points)
        {
            while (stack.Count >= 2)
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

        var chain = new List<(int X, int Y)>();
        while (stack.TryPop(out var item))
        {
            chain.Add(item);
        }

        chain.Reverse();
        return chain;
    }

    private static double Area((int X, int Y) a, (int X, int Y) b, (int X, int Y) c)
        => Math.Abs(Cross(a, b, c)) / 2.0;

    private static long Cross((int X, int Y) o, (int X, int Y) a, (int X, int Y) b)
        => (long)(a.X - o.X) * (b.Y - o.Y) - (long)(a.Y - o.Y) * (b.X - o.X);
}
