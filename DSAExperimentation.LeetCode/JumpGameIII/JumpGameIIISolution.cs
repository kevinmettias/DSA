using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.JumpGameIII;

// LeetCode 1306. Jump Game III: from index i you may jump to i+arr[i] or i-arr[i]
// (whichever stays in bounds); can you ever reach some index holding value 0?
//
// The puzzle is plain reachability over an implicit bidirectional-hop graph - no
// weights, no hop count to minimize - so the only thing the two strategies differ
// in is who owns the visited set and the pending frontier.
internal static class JumpGameIIISolution
{
    // The textbook answer: a BCL Stack<int> frontier over a bool[] visited map,
    // stopping the moment a zero-valued index is popped. Deliberately written
    // without this repo's primitives - it is the arm the composed strategy below
    // has to justify itself against.
    public static bool CanReachByStackWalk(int[] arr, int start)
    {
        var walk = new HopWalk(arr, new bool[arr.Length], new Stack<int>());
        walk.Pending.Push(start);

        while (walk.Pending.Count > 0)
        {
            if (HasLandedOnZero(walk))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasLandedOnZero(HopWalk walk)
    {
        var index = walk.Pending.Pop();

        if (walk.Visited[index])
        {
            return false;
        }

        walk.Visited[index] = true;

        if (walk.Values[index] == 0)
        {
            return true;
        }

        PushUnvisitedHops(index, walk);
        return false;
    }

    private static void PushUnvisitedHops(int index, HopWalk walk)
    {
        var forward = index + walk.Values[index];
        var backward = index - walk.Values[index];

        if (forward < walk.Values.Length && !walk.Visited[forward])
        {
            walk.Pending.Push(forward);
        }

        if (backward >= 0 && !walk.Visited[backward])
        {
            walk.Pending.Push(backward);
        }
    }

    private readonly record struct HopWalk(int[] Values, bool[] Visited, Stack<int> Pending);

    // This repo's own DepthFirstSearch.Traverse, closed over a bare
    // Func<int, IEnumerable<int>> successor relation: reachability is exactly what
    // Traverse returns, so the puzzle reduces to asking whether any visited index
    // holds a zero. Unlike JumpGameII (a minimum hop *count*, which needs the
    // weighted ShortestPath tier), this one only needs the unweighted traversal.
    public static bool CanReachByDepthFirstSearch(int[] arr, int start)
    {
        var visited = DepthFirstSearch.Traverse(start, index => Hops(arr, index));

        return visited.Exists(index => arr[index] == 0);
    }

    private static IEnumerable<int> Hops(int[] arr, int index)
    {
        var forward = index + arr[index];
        var backward = index - arr[index];

        if (forward < arr.Length)
        {
            yield return forward;
        }

        if (backward >= 0)
        {
            yield return backward;
        }
    }
}
