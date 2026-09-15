namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 856, reused by LC 1021 (Remove Outermost
// Parentheses), whose worst case wants the same "many sibling primitives, bounded
// nesting" shape. The scoring itself is
// ScoreOfParenthesesSolution's; what stays here is only how large a balanced
// string to measure and how deeply to nest it - measurement decisions, not domain
// ones. Depth is capped because unbounded nesting would make the baseline's
// 1 << depth (and the stack fold's 2 * inner) silently overflow int, which real
// LeetCode inputs (length <= 50) never approach either.
internal static class ScoreOfParenthesesWorkloads
{
    private const int CharsPerPair = 2;
    private const int CoinFlipBound = 2;

    // Generates a valid balanced-parentheses string of pairCount atomic pairs with
    // nesting depth capped at maxDepth.
    public static string BuildBalanced(int pairCount, int maxDepth, int seed)
    {
        var random = new Random(seed);
        var result = new char[pairCount * CharsPerPair];
        var counts = new ParenCounts();

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = NextParenChar(pairCount, maxDepth, random, ref counts);
        }

        return new string(result);
    }

    // Decides the next character of the balanced string and advances the running
    // open/close counts accordingly.
    private static char NextParenChar(int pairCount, int maxDepth, Random random, ref ParenCounts counts)
    {
        if (ShouldOpen(counts, pairCount, maxDepth, random))
        {
            counts.Open++;
            return '(';
        }

        counts.Close++;
        return ')';
    }

    // Whether the next character opens a new pair: only while pairs remain and the
    // depth cap is not reached, and then only on a coin flip - except when closing
    // is impossible, which forces another open.
    private static bool ShouldOpen(ParenCounts counts, int pairCount, int maxDepth, Random random)
        => counts.Open < pairCount
            && counts.Open - counts.Close < maxDepth
            && (counts.Close >= counts.Open || random.Next(CoinFlipBound) == 0);

    private sealed class ParenCounts
    {
        public int Open { get; set; }
        public int Close { get; set; }
    }
}
