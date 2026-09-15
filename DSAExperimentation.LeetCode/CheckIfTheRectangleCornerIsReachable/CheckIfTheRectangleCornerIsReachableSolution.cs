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

        var (touchesLeftOrTop, touchesBottomOrRight, adjacency) = BuildOverlapGraph(circles, xCorner, yCorner);

        return !ReachesBottomOrRight(adjacency, touchesLeftOrTop, touchesBottomOrRight);
    }

    // The whole overlap/boundary picture the flood fill below walks: which circles touch
    // each of the two boundary groups, and which circles overlap each other. Each pair is
    // recorded on both of its circles, so the flood can travel the edge either way.
    private static (bool[] TouchesLeftOrTop, bool[] TouchesBottomOrRight, List<int>[] Adjacency) BuildOverlapGraph(
        int[][] circles, int xCorner, int yCorner)
    {
        var count = circles.Length;
        var touchesLeftOrTop = new bool[count];
        var touchesBottomOrRight = new bool[count];
        var adjacency = new List<int>[count];

        for (var index = 0; index < count; index++)
        {
            adjacency[index] = [];
            touchesLeftOrTop[index] = TouchesLeftOrTop(circles[index], xCorner, yCorner);
            touchesBottomOrRight[index] = TouchesBottomOrRight(circles[index], xCorner, yCorner);

            for (var other = 0; other < index; other++)
            {
                if (Overlaps(circles[index], circles[other]))
                {
                    adjacency[index].Add(other);
                    adjacency[other].Add(index);
                }
            }
        }

        return (touchesLeftOrTop, touchesBottomOrRight, adjacency);
    }

    // The flood fill itself, standing in for DisjointSet: every circle touching the
    // left-or-top boundary seeds the wave, and the rectangle is cut in two as soon as the
    // wave reaches a circle touching the bottom-or-right boundary.
    private static bool ReachesBottomOrRight(
        List<int>[] adjacency, bool[] touchesLeftOrTop, bool[] touchesBottomOrRight)
    {
        var visited = new bool[adjacency.Length];
        var queue = new Queue<int>();
        EnqueueLeftOrTopCircles(adjacency.Length, touchesLeftOrTop, visited, queue);

        while (queue.Count > 0)
        {
            var circle = queue.Dequeue();

            if (touchesBottomOrRight[circle])
            {
                return true;
            }

            EnqueueUnvisitedNeighbors(adjacency[circle], visited, queue);
        }

        return false;
    }

    private static void EnqueueLeftOrTopCircles(int count, bool[] touchesLeftOrTop, bool[] visited, Queue<int> queue)
    {
        for (var index = 0; index < count; index++)
        {
            if (touchesLeftOrTop[index])
            {
                visited[index] = true;
                queue.Enqueue(index);
            }
        }
    }

    private static void EnqueueUnvisitedNeighbors(List<int> neighbors, bool[] visited, Queue<int> queue)
    {
        foreach (var neighbor in neighbors)
        {
            if (visited[neighbor])
            {
                continue;
            }

            visited[neighbor] = true;
            queue.Enqueue(neighbor);
        }
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

        var (groups, leftOrTopId, bottomOrRightId) = BuildDisjointSet(circles, xCorner, yCorner);

        return !groups.IsConnected(leftOrTopId, bottomOrRightId);
    }

    // Every overlap and every boundary touch, unioned in as it is found. The two extra ids
    // past the circles are the boundary groups themselves, so the whole "is the rectangle
    // cut in two" question is settled by one IsConnected between them.
    private static (DisjointSet Groups, int LeftOrTopId, int BottomOrRightId) BuildDisjointSet(
        int[][] circles, int xCorner, int yCorner)
    {
        var count = circles.Length;
        var leftOrTopId = count;
        var bottomOrRightId = count + 1;
        var groups = new DisjointSet(count + 2);

        for (var index = 0; index < count; index++)
        {
            if (TouchesLeftOrTop(circles[index], xCorner, yCorner))
            {
                groups.Union(index, leftOrTopId);
            }

            if (TouchesBottomOrRight(circles[index], xCorner, yCorner))
            {
                groups.Union(index, bottomOrRightId);
            }

            JoinOverlappingCircles(groups, circles, index);
        }

        return (groups, leftOrTopId, bottomOrRightId);
    }

    // Each overlap is unioned once, from the lower-indexed circle to the higher one, which
    // is the same edge set BuildOverlapGraph records both ways for the flood fill.
    private static void JoinOverlappingCircles(DisjointSet groups, int[][] circles, int index)
    {
        for (var other = index + 1; other < circles.Length; other++)
        {
            if (Overlaps(circles[index], circles[other]))
            {
                groups.Union(index, other);
            }
        }
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
        => TouchesSegment(circle, (0, 0), (0, yCorner)) ||
            TouchesSegment(circle, (0, yCorner), (xCorner, yCorner));

    // The bottom edge (y=0, 0<=x<=xCorner) or the right edge (x=xCorner, 0<=y<=yCorner).
    private static bool TouchesBottomOrRight(int[] circle, int xCorner, int yCorner)
        => TouchesSegment(circle, (0, 0), (xCorner, 0)) ||
            TouchesSegment(circle, (xCorner, 0), (xCorner, yCorner));

    // Distance from the circle's center to the nearest point of an axis-aligned segment,
    // compared against the radius - the segment is always either purely vertical or
    // purely horizontal here, so clamping one coordinate onto the segment's fixed span
    // gives the closest point directly, with no general point-to-segment projection
    // needed.
    private static bool TouchesSegment(int[] circle, (int X, int Y) from, (int X, int Y) to)
    {
        var closestX = ClampOntoSpan(circle[0], from.X, to.X);
        var closestY = ClampOntoSpan(circle[1], from.Y, to.Y);

        return DistanceSquared(circle[0], circle[1], closestX, closestY) <= SquaredRadius(circle);
    }

    // A segment with a fixed span leaves the centre's coordinate alone when the two ends
    // agree on it, and clamps it into the span otherwise - the Min/Max pair is the span's
    // own low and high ends, so it stays inside the branch that needs it.
    private static int ClampOntoSpan(int coordinate, int firstEnd, int secondEnd)
    {
        if (firstEnd == secondEnd)
        {
            return firstEnd;
        }

        var low = Math.Min(firstEnd, secondEnd);
        var high = Math.Max(firstEnd, secondEnd);
        return Math.Clamp(coordinate, low, high);
    }

    private static long SquaredRadius(int[] circle) => (long)circle[2] * circle[2];

    private static long DistanceSquared(long x1, long y1, long x2, long y2)
    {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return (dx * dx) + (dy * dy);
    }
}
