using ScoreStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ScoreOfParentheses;

// LeetCode 856. Score of Parentheses: "()" scores 1, "AB" scores A + B, and
// "(A)" scores 2 * A. Both strategies answer the same question - the score of the
// whole balanced string - and differ only in how they recover each atomic pair's
// nesting depth.
//
// ScoreByNestedDepthScan is the textbook brute force: every atomic "()" pair
// contributes 2^depth, and it recomputes that depth by rescanning the prefix in
// front of the pair, O(n) per pair and O(n^2) overall. ScoreByMonotonicStackFold
// instead carries the partial scores on this repo's own Stack<int>
// (DailyTemperatures/AsteroidCollision/BasicCalculator precedent for it over the
// CLR's own System.Collections.Generic.Stack), seeded with one sentinel 0 for the
// implicit outermost scope: each '(' opens a fresh nested scope, each ')' folds the
// scope it closes - 1 if empty, doubled otherwise - into the scope enclosing it.
// The sentinel is what makes that fold-back safe when ')' closes the outermost
// pair, with no separate "am I at the top level" branch.
internal static class ScoreOfParenthesesSolution
{
    // "(A)" is worth twice "A"; an empty pair is worth 1 instead.
    private const int NestedScoreMultiplier = 2;

    private const int EmptyPairScore = 1;

    // Brute force: score every atomic "()" as 2^depth, re-deriving depth by
    // rescanning everything before it. BCL-only internals by design (§17.5).
    public static int ScoreByNestedDepthScan(string s)
    {
        var total = 0;

        for (var i = 0; i < s.Length - 1; i++)
        {
            if (s[i] != '(' || s[i + 1] != ')')
            {
                continue;
            }

            var depth = 0;

            for (var j = 0; j < i; j++)
            {
                var isOpeningParen = s[j] == '(';
                depth += isOpeningParen ? 1 : -1;
            }

            total += EmptyPairScore << depth;
        }

        return total;
    }

    // One left-to-right pass over a sentinel-seeded Stack<int> of partial scores:
    // one push per '(', one fold-back per ')', O(n) total.
    public static int ScoreByMonotonicStackFold(string s)
    {
        var scores = new ScoreStack();
        scores.Push(0);

        foreach (var c in s)
        {
            if (c == '(')
            {
                scores.Push(0);
                continue;
            }

            scores.TryPop(out var inner);
            scores.TryPop(out var outer);
            scores.Push(outer + Math.Max(NestedScoreMultiplier * inner, EmptyPairScore));
        }

        scores.TryPop(out var result);
        return result;
    }
}
