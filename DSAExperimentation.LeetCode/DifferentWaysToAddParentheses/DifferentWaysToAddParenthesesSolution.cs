using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.DifferentWaysToAddParentheses;

// LeetCode 241. Different Ways to Add Parentheses: split expression on every
// operator, recursively compute every result for the left and right substrings,
// then combine each (left, right) pair through that operator.
//
// The two strategies solve the identical recurrence and differ only in whether a
// substring reached from more than one split point - e.g. for "1+1+1+1", "1+1"
// is reached both as the left half of one split and the right half of another -
// is re-derived from scratch each time, or solved once and shared through this
// repo's own Memoizer<TState,TResult>, keyed by substring.
internal static class DifferentWaysToAddParenthesesSolution
{
    // Textbook baseline: plain split/recurse/combine, re-deriving any substring
    // reached from more than one parent call. Deliberately written without this
    // repo's primitives - it is the arm the memoized strategy below has to
    // justify itself against.
    public static List<int> DiffWaysToComputeByPlainRecursion(string expression) =>
        ComputeResults(expression, new RecomputedSubstrings());

    // The recurrence both arms share. A substring that parses as a number is its
    // own single answer; otherwise every operator split point contributes its
    // left/right combinations. Which recurrence is handed in is the only thing
    // that tells the arms apart - the plain recursion re-derives each half, the
    // memoized arm shares it.
    private static List<int> ComputeResults(string expression, IRecurrence<string, List<int>> compute)
    {
        if (int.TryParse(expression, out var value))
        {
            return [value];
        }

        var results = new List<int>();

        for (var i = 0; i < expression.Length; i++)
        {
            var op = expression[i];

            if (op is not ('+' or '-' or '*'))
            {
                continue;
            }

            AddCombinations(results, expression, i, compute);
        }

        return results;
    }

    // Every left/right pair the split point at `index` contributes, combined
    // through the operator sitting at it.
    private static void AddCombinations(
        List<int> results, string expression, int index, IRecurrence<string, List<int>> compute)
    {
        var op = expression[index];

        foreach (var left in compute.Replay(expression[..index], compute))
        {
            foreach (var right in compute.Replay(expression[(index + 1)..], compute))
            {
                var combined = Combine(op, left, right);
                results.Add(combined);
            }
        }
    }

    private static int Combine(char op, int left, int right) => op switch
    {
        '+' => left + right,
        '-' => left - right,
        _ => left * right,
    };

    // Identical recurrence, driven top-down through Memoizer keyed by substring,
    // so a substring reached from multiple parents is evaluated once and its
    // result list shared across every caller.
    public static List<int> DiffWaysToComputeByMemoizedSubstring(string expression) =>
        Memoizer.Memoize<string, List<int>>(expression, new SharedSubstrings());

    // The recurrence re-derived from scratch at every call: no cache in front of it,
    // so a substring reached from more than one parent is worked out again each time.
    // This is the arm the memoized SharedSubstrings is measured against.
    private sealed class RecomputedSubstrings : IRecurrence<string, List<int>>
    {
        public List<int> Replay(string expression, IRecurrence<string, List<int>> rest) =>
            ComputeResults(expression, this);
    }

    // The recurrence answered once per substring: the memo run in front of it is what
    // makes a substring reached from several split points share a single result list.
    private sealed class SharedSubstrings : IRecurrence<string, List<int>>
    {
        public List<int> Replay(string expression, IRecurrence<string, List<int>> rest) =>
            ComputeResults(expression, rest);
    }
}
