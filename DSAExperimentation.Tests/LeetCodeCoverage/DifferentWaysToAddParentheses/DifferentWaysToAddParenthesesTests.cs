using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DifferentWaysToAddParentheses;

// LeetCode 241. Different Ways to Add Parentheses: split on every operator, recurse
// on the left/right substrings, then combine every (left, right) result pair - this
// repo's own Memoizer<TState,TResult> supplies the cache, keyed by substring, the
// same "recurrence takes a memoized recursive callback" shape DecodeWaysTests.cs
// already uses, just with a List<int> result per state instead of a single int.
public sealed partial class DifferentWaysToAddParenthesesTests
{
    [Fact]
    public void ComputeResults_TwoMinusOneMinusOne_ReturnsBothGroupings()
    {
        var results = ComputeResults("2-1-1");

        Assert.Equal([0, 2], results.Order());
    }

    [Fact]
    public void ComputeResults_TwoTimesThreeMinusFourTimesFive_ReturnsAllFiveGroupings()
    {
        var results = ComputeResults("2*3-4*5");

        Assert.Equal([-34, -14, -10, -10, 10], results.Order());
    }

    [Fact]
    public void ComputeResults_SingleOperand_ReturnsThatValue()
    {
        var results = ComputeResults("7");

        Assert.Equal([7], results);
    }

    private static List<int> ComputeResults(string expression) => Memoizer.Memoize<string, List<int>>(expression, Evaluate);

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
                    results.Add(op switch
                    {
                        '+' => left + right,
                        '-' => left - right,
                        _ => left * right,
                    });
                }
            }
        }

        return results;
    }
}
