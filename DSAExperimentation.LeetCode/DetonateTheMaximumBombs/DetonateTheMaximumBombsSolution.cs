using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.DetonateTheMaximumBombs;

// LeetCode 2101. Detonate the Maximum Bombs: bomb i's blast reaches bomb j exactly
// when j's center falls inside i's radius - a directed edge i -> j, the same
// implicit-graph shape JumpGameIII uses. "How many bombs go off if I trigger bomb i"
// is then just the size of the reachable set from i, and the answer is the largest
// such set over every candidate trigger. The relation is asymmetric (a big bomb can
// reach a small one that cannot reach back), so the walk has to be redone per
// candidate rather than being one undirected component count.
//
// Reachability is a squared-distance-against-squared-radius comparison, taken in
// long to stay clear of overflow at the problem's 1e5 coordinate bound. That is
// plain scalar arithmetic: no geometry primitive in this repo applies to a single
// circle-contains-point check, the same "lighter repo-primitive fit"
// CircleAndRectangleOverlapping already documents.
//
// Both strategies run the identical O(n) scan per candidate over the identical
// edges - no smarter algorithm applies, since LeetCode's own constraints assume the
// try-every-source O(n^3) shape. They differ only in how the visited set is carried:
// a bool[] indexed by bomb with hand-rolled recursion, against the same successor
// closure handed to this repo's DepthFirstSearch.Traverse, which keeps a HashSet and
// an explicit Stack. The comparison is about that container overhead, not asymptotics.
internal static class DetonateTheMaximumBombsSolution
{
    // A bomb is [x, y, radius].
    private const int XIndex = 0;
    private const int YIndex = 1;
    private const int RadiusIndex = 2;

    // The textbook answer: a fresh bool[] per candidate trigger and a recursive
    // walk written out by hand, with no repo primitive anywhere inside it.
    public static int MaxDetonationsByManualRecursion(int[][] bombs)
    {
        var maxDetonated = 0;

        for (var i = 0; i < bombs.Length; i++)
        {
            var visited = new bool[bombs.Length];
            VisitManual(bombs, i, visited);
            maxDetonated = Math.Max(maxDetonated, CountVisited(visited));
        }

        return maxDetonated;
    }

    private static int CountVisited(bool[] visited)
    {
        var count = 0;

        foreach (var wasVisited in visited)
        {
            if (wasVisited)
            {
                count++;
            }
        }

        return count;
    }

    // The same per-candidate walk expressed as a successor closure handed to this
    // repo's DepthFirstSearch.Traverse, whose returned visit order doubles as the
    // reachable set - so the chain size is just its Count.
    public static int MaxDetonationsByRepoDepthFirstSearch(int[][] bombs)
    {
        var maxDetonated = 0;

        for (var i = 0; i < bombs.Length; i++)
        {
            var reached = DepthFirstSearch.Traverse(i, index => Reachable(bombs, index));
            maxDetonated = Math.Max(maxDetonated, reached.Count);
        }

        return maxDetonated;
    }

    private static IEnumerable<int> Reachable(int[][] bombs, int index)
    {
        for (var j = 0; j < bombs.Length; j++)
        {
            if (j != index && IsWithinBlastRadius(bombs, index, j))
            {
                yield return j;
            }
        }
    }

    private static void VisitManual(int[][] bombs, int index, bool[] visited)
    {
        if (visited[index])
        {
            return;
        }

        visited[index] = true;

        for (var j = 0; j < bombs.Length; j++)
        {
            if (!visited[j] && IsWithinBlastRadius(bombs, index, j))
            {
                VisitManual(bombs, j, visited);
            }
        }
    }

    // Squared comparison in long: at the problem's 1e5 coordinate bound a squared
    // separation reaches 2e10, well past what int holds.
    private static bool IsWithinBlastRadius(int[][] bombs, int index, int targetIndex)
    {
        var dx = (long)(bombs[targetIndex][XIndex] - bombs[index][XIndex]);
        var dy = (long)(bombs[targetIndex][YIndex] - bombs[index][YIndex]);
        var radius = (long)bombs[index][RadiusIndex];

        return (dx * dx) + (dy * dy) <= radius * radius;
    }
}
