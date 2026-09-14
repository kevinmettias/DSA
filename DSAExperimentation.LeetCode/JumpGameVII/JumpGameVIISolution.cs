using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.JumpGameVII;

// LeetCode 1871. Jump Game VII: starting at index 0 of a binary string, a move goes
// from i to any j in [i + minJump, min(i + maxJump, n - 1)] whose character is '0'.
// Can the last index be reached?
//
// That is reachability over an implicit graph - the edge set is a rule, never
// materialized - so the two strategies differ only in whether they remember where
// they have already been. CanReachByUnmemoizedRecursion re-explores an index once
// per jump chain that reconverges on it, which for a narrow (maxJump - minJump)
// window is a Fibonacci-shaped call-count blowup. CanReachByVisitedTrackingTraversal
// hands the same successor rule to this repo's own DepthFirstSearch.Traverse, whose
// HashSet-backed visited tracking makes it one O(n * (maxJump - minJump)) walk - the
// same "board too large or awkward to materialize as actual edges" composition
// EscapeALargeMaze uses, just over a 1-D index range instead of a 2-D grid.
internal static class JumpGameVIISolution
{
    // The textbook answer: plain recursion with no visited set at all. Deliberately
    // BCL-only - it is the arm the traversal below has to justify itself against.
    public static bool CanReachByUnmemoizedRecursion(string positions, int minJump, int maxJump)
        => CanReachFrom(positions, 0, minJump, maxJump);

    // This repo's own DepthFirstSearch.Traverse: the successor rule is a plain Func,
    // the visited tracking is Traverse's, and the answer is whether the last index
    // turns up in the reached set.
    public static bool CanReachByVisitedTrackingTraversal(string positions, int minJump, int maxJump)
    {
        var openIndicesAfter = OpenIndicesAfter(positions, minJump, maxJump);
        var reached = DepthFirstSearch.Traverse(0, openIndicesAfter);

        return reached.Contains(positions.Length - 1);
    }

    // Every landable index in the jump window after one index: '1' characters are
    // walls, and the window is clipped to the last index.
    private static Func<int, IEnumerable<int>> OpenIndicesAfter(string positions, int minJump, int maxJump)
        => index =>
        {
            var lastIndex = Math.Min(index + maxJump, positions.Length - 1);
            var next = new List<int>();

            for (var j = index + minJump; j <= lastIndex; j++)
            {
                if (positions[j] == '0')
                {
                    next.Add(j);
                }
            }

            return next;
        };

    private static bool CanReachFrom(string positions, int index, int minJump, int maxJump)
    {
        if (index == positions.Length - 1)
        {
            return true;
        }

        var lastIndex = Math.Min(index + maxJump, positions.Length - 1);

        for (var j = index + minJump; j <= lastIndex; j++)
        {
            if (positions[j] == '0' && CanReachFrom(positions, j, minJump, maxJump))
            {
                return true;
            }
        }

        return false;
    }
}
