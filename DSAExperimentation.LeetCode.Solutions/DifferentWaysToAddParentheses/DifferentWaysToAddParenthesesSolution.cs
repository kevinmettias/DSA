using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.Solutions.DifferentWaysToAddParentheses;

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
    // repo's primitives - the recursion is written out directly rather than through
    // Memoizer's IRecurrence contract, because a baseline reaches no library type
    // (ARCHITECTURE 17.5). It is the arm the memoized strategy below has to justify
    // itself against.
    public static List<int> DiffWaysToComputeByPlainRecursion(string expression) =>
        PlainComputeResults(expression);

    // Identical recurrence, driven top-down through Memoizer keyed by substring,
    // so a substring reached from multiple parents is evaluated once and its
    // result list shared across every caller.
    public static List<int> DiffWaysToComputeByMemoizedSubstring(string expression) =>
        Memoizer.Memoize<string, List<int>>(expression, new SharedSubstrings());

    // The recurrence for the memoized arm: the same splits as PlainComputeResults, each
    // half asked of the `compute` recurrence so a repeated substring is answered from
    // the cache.
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

    // The recurrence as plain recursion. A substring that parses as a number is its
    // own single answer; otherwise every operator split point contributes its
    // left/right combinations, each half re-derived by calling straight back in.
    private static List<int> PlainComputeResults(string expression)
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

            PlainAddCombinations(results, expression, i);
        }

        return results;
    }

    // Every left/right pair the split point at `index` contributes, combined
    // through the operator sitting at it, with both halves recomputed.
    private static void PlainAddCombinations(List<int> results, string expression, int index)
    {
        var op = expression[index];

        foreach (var left in PlainComputeResults(expression[..index]))
        {
            foreach (var right in PlainComputeResults(expression[(index + 1)..]))
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

    // The recurrence answered once per substring: the memo run in front of it is what
    // makes a substring reached from several split points share a single result list.
    private sealed class SharedSubstrings : IRecurrence<string, List<int>>
    {
        public List<int> Replay(string expression, IRecurrence<string, List<int>> rest) =>
            ComputeResults(expression, rest);
    }

}
