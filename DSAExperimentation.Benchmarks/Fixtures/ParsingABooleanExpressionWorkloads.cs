namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1106: a valid boolean expression whose every
// operator node sits above a full binary tree of the given depth - each internal
// node an "&" or "|" over two subexpressions, a seeded quarter of them wrapped in
// "!", and every leaf "t" or "f". A tree of depth d is at most 8 * 2^d - 7
// characters (every node negated), so depth 11 stays inside LC's 2 * 10^4 however
// the draws fall, and the expression's size grows with depth instead of hanging on
// an early draw.
internal static class ParsingABooleanExpressionWorkloads
{
    public const int MaxLength = 20_000;

    private const string TrueToken = "t";
    private const string FalseToken = "f";

    // One draw over this many choices per node: zero picks the first option.
    private const int CoinSides = 2;

    // One internal node in this many is wrapped in "!".
    private const int NegationOneIn = 4;

    public static string Build(int depth, Random random)
    {
        if (depth == 0)
        {
            var isTrue = random.Next(CoinSides) == 0;

            return isTrue ? TrueToken : FalseToken;
        }

        var isAnd = random.Next(CoinSides) == 0;
        var left = Build(depth - 1, random);
        var right = Build(depth - 1, random);
        var combined = isAnd ? $"&({left},{right})" : $"|({left},{right})";
        var isNegated = random.Next(NegationOneIn) == 0;

        return isNegated ? $"!({combined})" : combined;
    }
}
