using System.Text;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.KthSmallestInstructions;

// LeetCode 1643. Kth Smallest Instructions: destination is [row, column], every
// route uses exactly that many 'V' and 'H' steps in some order, and the answer is
// the k-th such route in lexicographic order - where 'H' sorts before 'V', so the
// k-th route is the k-th arrangement read left to right.
//
// The baseline produces the whole ordering and indexes into it. The greedy arm
// decides one character at a time, asking how many routes remain reachable if the
// smaller choice 'H' is taken: that count is the Pascal's-triangle recurrence
// ways(v, h) = ways(v-1, h) + ways(v, h-1) that UniquePaths already memoizes for
// LeetCode 62, so the rank either falls inside the 'H' block or walks past it onto
// 'V'.
//
// The rank is taken as a long rather than LeetCode's int so the count of routes can
// be compared against it without a widening cast at every step; every int caller
// converts implicitly.
internal static class KthSmallestInstructionsSolution
{
    private const char Horizontal = 'H';

    private const char Vertical = 'V';

    private const int RowIndex = 0;

    private const int ColumnIndex = 1;

    // The textbook brute force: generate every valid instruction string, sort them
    // lexicographically, and take the k-th. Deliberately BCL only - string
    // concatenation, a List<string> and its ordinal sort - because it is the arm
    // the greedy walk below has to justify itself against, and building
    // C(v+h, v) strings IS its cost.
    public static string KthSmallestPathByEnumerateAndSort(int[] destination, long rank)
    {
        var paths = new List<string>();

        Generate(string.Empty, destination[RowIndex], destination[ColumnIndex], paths);
        paths.Sort(StringComparer.Ordinal);

        return paths[(int)rank - 1];
    }

    // This repo's own Memoizer caches the binomial counts, so each of the v + h
    // characters is decided in O(v * h) cached states instead of enumerating
    // C(v+h, v) whole strings.
    public static string KthSmallestPathByMemoizedGreedy(int[] destination, long rank)
    {
        var state = new GreedyState(destination[RowIndex], destination[ColumnIndex], rank);
        var path = new StringBuilder();

        while (state.RemainingV > 0 || state.RemainingH > 0)
        {
            state = AppendNextInstruction(state, path);
        }

        return path.ToString();
    }

    // Once one axis is exhausted the rest of the route is forced, so only the
    // genuinely two-way step consults the recurrence.
    private static GreedyState AppendNextInstruction(GreedyState state, StringBuilder path)
    {
        if (state.RemainingH == 0)
        {
            path.Append(Vertical);
            return state with { RemainingV = state.RemainingV - 1 };
        }

        if (state.RemainingV == 0)
        {
            path.Append(Horizontal);
            return state with { RemainingH = state.RemainingH - 1 };
        }

        var waysIfH = Memoizer.Memoize<(int V, int H), long>(
            (state.RemainingV, state.RemainingH - 1), new RoutesFromStepsLeft());

        if (state.K <= waysIfH)
        {
            path.Append(Horizontal);
            return state with { RemainingH = state.RemainingH - 1 };
        }

        path.Append(Vertical);
        return state with { RemainingV = state.RemainingV - 1, K = state.K - waysIfH };
    }

    private static void Generate(string prefix, int remainingV, int remainingH, List<string> paths)
    {
        if (remainingV == 0 && remainingH == 0)
        {
            paths.Add(prefix);
            return;
        }

        if (remainingH > 0)
        {
            Generate(prefix + Horizontal, remainingV, remainingH - 1, paths);
        }

        if (remainingV > 0)
        {
            Generate(prefix + Vertical, remainingV - 1, remainingH, paths);
        }
    }

    private readonly record struct GreedyState(int RemainingV, int RemainingH, long K);

    // The route-count rule, named: ways(v, h) = ways(v - 1, h) + ways(v, h - 1) - the
    // routes split on whether the next step is the horizontal one or the vertical one,
    // and one step left on either axis leaves exactly one route.
    private sealed class RoutesFromStepsLeft : IRecurrence<(int V, int H), long>
    {
        /// <inheritdoc/>
        public long Replay((int V, int H) state, IRecurrence<(int V, int H), long> rest)
        {
            var (v, h) = state;

            if (v == 0 || h == 0)
            {
                return 1;
            }

            return rest.Replay((v - 1, h), rest) + rest.Replay((v, h - 1), rest);
        }
    }
}
