using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.WaterAndJugProblem;

// LeetCode 365. Water and Jug Problem: decide whether some sequence of
// fill/empty/pour moves over two jugs of capacity jugX and jugY can leave
// exactly `target` units of water measured out.
//
// StackSearch and DepthFirstSearch both explore the same implicit graph of
// (jugX fill level, jugY fill level) states - each state's six successors are
// the classic fill/empty/pour moves, and the target is reachable iff some
// visited state's two jugs sum to it. GcdFormula replaces the search
// entirely with Bezout's identity: target is reachable iff it does not
// exceed the combined capacity and is a multiple of gcd(jugX, jugY).
internal static class WaterAndJugProblemSolution
{
    // The textbook answer: a hand-rolled BCL Stack<T>/HashSet<T> DFS.
    // Iterative rather than recursive because this state graph's DFS order
    // can nest thousands of frames deep at large capacities.
    public static bool CanMeasureWaterByStackSearch(int jugX, int jugY, int target)
    {
        var visited = new HashSet<(int X, int Y)>();
        var pending = new Stack<(int X, int Y)>();
        pending.Push((0, 0));

        while (pending.TryPop(out var state))
        {
            if (!visited.Add(state))
            {
                continue;
            }

            if (state.X + state.Y == target)
            {
                return true;
            }

            foreach (var next in Successors(state, jugX, jugY))
            {
                if (!visited.Contains(next))
                {
                    pending.Push(next);
                }
            }
        }

        return false;
    }

    // This repo's own DepthFirstSearch.Traverse, generating each state's six
    // successors on demand instead of materializing the graph up front.
    public static bool CanMeasureWaterByDepthFirstSearch(int jugX, int jugY, int target)
    {
        if (target > jugX + jugY)
        {
            return false;
        }

        var start = (X: 0, Y: 0);
        var visited = DepthFirstSearch.Traverse(start, state => Successors(state, jugX, jugY));

        return visited.Any(state => state.X + state.Y == target);
    }

    // Bezout's identity: target is reachable iff it fits in the combined
    // capacity and is a multiple of gcd(jugX, jugY).
    public static bool CanMeasureWaterByGcdFormula(int jugX, int jugY, int target)
    {
        if (target > jugX + jugY)
        {
            return false;
        }

        return target % Gcd(jugX, jugY) == 0;
    }

    private static IEnumerable<(int X, int Y)> Successors((int X, int Y) state, int jugX, int jugY)
    {
        yield return (jugX, state.Y);
        yield return (state.X, jugY);
        yield return (0, state.Y);
        yield return (state.X, 0);

        var pourXtoY = Math.Min(state.X, jugY - state.Y);
        yield return (state.X - pourXtoY, state.Y + pourXtoY);

        var pourYtoX = Math.Min(state.Y, jugX - state.X);
        yield return (state.X + pourYtoX, state.Y - pourYtoX);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
