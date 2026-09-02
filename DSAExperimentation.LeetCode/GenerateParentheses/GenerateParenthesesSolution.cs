using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.GenerateParentheses;

// LeetCode 22. Generate Parentheses: every well-formed combination of n pairs,
// built by choosing '(' while opens remain and ')' while it would not unbalance the
// prefix already chosen.
//
// The two strategies differ only in what drives that choose/explore/unchoose
// recursion - Backtrack.Search's generic engine mutating a dedicated ParenthesesState
// (this folder), or a specialized recursive function tracking the same two counters
// directly. Both are asked to build LeetCode's actual answer, a List<string>; the
// benchmark this migrated out of had both arms merely counting completions to avoid
// paying for the List<string> allocation, which is a valid measurement choice but
// not the question either strategy is proven correct against (ARCHITECTURE.md 17.8's
// WordLadderII precedent for promoting a counting arm back to the real answer).
internal static class GenerateParenthesesSolution
{
    // The composed solution: Backtrack.Search owns the recursion; ParenthesesState
    // enforces the open/close invariant and answers Candidates itself.
    public static List<string> GenerateByBacktracking(int pairs)
    {
        var results = new List<string>();
        var state = new ParenthesesState(pairs);

        Backtrack.Search<ParenthesesState, char>(
            state,
            isSolution: s => s.Buffer.Count == s.TargetLength,
            candidates: s => s.Buffer.Count == s.TargetLength ? [] : s.Candidates(),
            choose: (s, c) => s.Choose(c),
            unchoose: (s, c) => s.Unchoose(c),
            onSolution: s => results.Add(new string(s.Buffer.ToArray())));

        return results;
    }

    // The textbook answer many first reach for: a specialized recursive function
    // tracking the open/close counts directly, without this repo's backtracking
    // engine. Deliberately written without this repo's primitives beyond the plain
    // char[] buffer and List<string> result.
    public static List<string> GenerateByRecursiveSpecialized(int pairs)
    {
        var results = new List<string>();
        var buffer = new char[pairs * 2];

        Search(0, 0);
        return results;

        void Search(int open, int close)
        {
            if (open == pairs && close == pairs)
            {
                results.Add(new string(buffer));
                return;
            }

            if (open < pairs)
            {
                buffer[open + close] = '(';
                Search(open + 1, close);
            }

            if (close < open)
            {
                buffer[open + close] = ')';
                Search(open, close + 1);
            }
        }
    }
}
