using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.CheckIfTheRectangleCornerIsReachable;

// LeetCode 3235. Check if the Rectangle Corner Is Reachable: the rectangle runs from
// (0,0) to (xCorner,yCorner); a path from one corner to the other must stay inside it
// and never touch or enter any circle. A path is blocked exactly when the circles glue
// together into one obstacle that spans from the left-or-top boundary through to the
// bottom-or-right boundary, since any such chain cuts the rectangle in two. That
// reduces to: group circles into components by pairwise overlap, and check whether any
// component touches both boundary groups - "grouped by an equivalence relation, then
// asked whether two particular elements ended up in the same group" is this repo's own
// DisjointSet's whole reason for existing (FindTheStringWithLCPSolution.cs composes it
// for the identical shape).
//
// Both strategies share the geometry helpers below; they differ only in how they decide
// "did the left/top group ever reach the bottom/right group" - a flood fill over an
// explicit adjacency list (baseline) versus DisjointSet's own Union/IsConnected
// (composed). Distances are compared as squares in long arithmetic throughout - up to
// 1e9 coordinates make (2e9)^2 overflow int but not long - so no circle-touches-edge
// check ever calls Math.Sqrt and risks a precision-driven wrong answer at the boundary.
internal static class CheckIfTheRectangleCornerIsReachableSolution
{
    // Baseline: build the same overlap/boundary information the composed strategy does,
    // but decide reachability with a plain BCL queue flood fill from every circle that
    // touches the left-or-top boundary, instead of DisjointSet.
    public static bool IsReachableByBoundaryFloodFill(int xCorner, int yCorner, int[][] circles)
    {
        if (AnyCircleCoversCorner(xCorner, yCorner, circles))
        {
            return false;
        }

        var count = circles.Length;
        var touchesLeftOrTop = new bool[count];
        var touchesBottomOrRight = new bool[count];
        var adjacency = new List<int>[count];

        for (var i = 0; i < count; i++)
        {
            adjacency[i] = [];
            touchesLeftOrTop[i] = TouchesLeftOrTop(circles[i], xCorner, yCorner);
            touchesBottomOrRight[i] = TouchesBottomOrRight(circles[i], xCorner, yCorner);

            for (var j = 0; j < i; j++)
            {
                if (Overlaps(circles[i], circles[j]))
                {
                    adjacency[i].Add(j);
                    adjacency[j].Add(i);
                }
            }
        }

        var visited = new bool[count];
        var queue = new Queue<int>();

        for (var i = 0; i < count; i++)
        {
            if (touchesLeftOrTop[i])
            {
                visited[i] = true;
                queue.Enqueue(i);
            }
        }

        while (queue.Count > 0)
        {
            var circle = queue.Dequeue();

            if (touchesBottomOrRight[circle])
            {
                return false;
            }

            foreach (var neighbor in adjacency[circle])
            {
                if (visited[neighbor])
                {
                    continue;
                }

                visited[neighbor] = true;
                queue.Enqueue(neighbor);
            }
        }

        return true;
    }

    // Composed: two extra ids beyond the circles - LeftOrTop and BottomOrRight - stand
    // for the two boundary groups, so "is the rectangle cut in two" is a single
    // DisjointSet.IsConnected query once every overlap and every boundary touch has
    // been unioned in.
    public static bool IsReachableByDisjointSet(int xCorner, int yCorner, int[][] circles)
    {
        if (AnyCircleCoversCorner(xCorner, yCorner, circles))
        {
            return false;
        }

        var count = circles.Length;
        var leftOrTopId = count;
        var bottomOrRightId = count + 1;
        var groups = new DisjointSet(count + 2);

        for (var i = 0; i < count; i++)
        {
            if (TouchesLeftOrTop(circles[i], xCorner, yCorner))
            {
                groups.Union(i, leftOrTopId);
            }

            if (TouchesBottomOrRight(circles[i], xCorner, yCorner))
            {
                groups.Union(i, bottomOrRightId);
            }

            for (var j = i + 1; j < count; j++)
            {
                if (Overlaps(circles[i], circles[j]))
                {
                    groups.Union(i, j);
                }
            }
        }

        return !groups.IsConnected(leftOrTopId, bottomOrRightId);
    }

    private static bool AnyCircleCoversCorner(int xCorner, int yCorner, int[][] circles)
    {
        foreach (var circle in circles)
        {
            if (DistanceSquared(circle[0], circle[1], 0, 0) <= SquaredRadius(circle) ||
                DistanceSquared(circle[0], circle[1], xCorner, yCorner) <= SquaredRadius(circle))
            {
                return true;
            }
        }

        return false;
    }

    private static bool Overlaps(int[] first, int[] second)
    {
        var radiusSum = (long)first[2] + second[2];

        return DistanceSquared(first[0], first[1], second[0], second[1]) <= radiusSum * radiusSum;
    }

    // The left edge (x=0, 0<=y<=yCorner) or the top edge (y=yCorner, 0<=x<=xCorner).
    private static bool TouchesLeftOrTop(int[] circle, int xCorner, int yCorner)
        => TouchesSegment(circle, 0, 0, 0, yCorner) || TouchesSegment(circle, 0, yCorner, xCorner, yCorner);

    // The bottom edge (y=0, 0<=x<=xCorner) or the right edge (x=xCorner, 0<=y<=yCorner).
    private static bool TouchesBottomOrRight(int[] circle, int xCorner, int yCorner)
        => TouchesSegment(circle, 0, 0, xCorner, 0) || TouchesSegment(circle, xCorner, 0, xCorner, yCorner);

    // Distance from the circle's center to the nearest point of an axis-aligned segment,
    // compared against the radius - the segment is always either purely vertical or
    // purely horizontal here, so clamping one coordinate onto the segment's fixed span
    // gives the closest point directly, with no general point-to-segment projection
    // needed.
    private static bool TouchesSegment(int[] circle, int x1, int y1, int x2, int y2)
    {
        var closestX = x1 == x2 ? x1 : Math.Clamp(circle[0], Math.Min(x1, x2), Math.Max(x1, x2));
        var closestY = y1 == y2 ? y1 : Math.Clamp(circle[1], Math.Min(y1, y2), Math.Max(y1, y2));

        return DistanceSquared(circle[0], circle[1], closestX, closestY) <= SquaredRadius(circle);
    }

    private static long SquaredRadius(int[] circle) => (long)circle[2] * circle[2];

    private static long DistanceSquared(long x1, long y1, long x2, long y2)
    {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return (dx * dx) + (dy * dy);
    }
}
