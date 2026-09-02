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
    public static List<int> DiffWaysToComputeByPlainRecursion(string expression)
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

            foreach (var left in DiffWaysToComputeByPlainRecursion(expression[..i]))
            {
                foreach (var right in DiffWaysToComputeByPlainRecursion(expression[(i + 1)..]))
                {
                    results.Add(Combine(op, left, right));
                }
            }
        }

        return results;
    }

    // Identical recurrence, driven top-down through Memoizer keyed by substring,
    // so a substring reached from multiple parents is evaluated once and its
    // result list shared across every caller.
    public static List<int> DiffWaysToComputeByMemoizedSubstring(string expression) =>
        Memoizer.Memoize<string, List<int>>(expression, Evaluate);

    private static List<int> Evaluate(string expression, Func<string, List<int>> compute)
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

            foreach (var left in compute(expression[..i]))
            {
                foreach (var right in compute(expression[(i + 1)..]))
                {
                    results.Add(Combine(op, left, right));
                }
            }
        }

        return results;
    }

    private static int Combine(char op, int left, int right) => op switch
    {
        '+' => left + right,
        '-' => left - right,
        _ => left * right,
    };
}
