using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ExpressionAddOperators;

// LeetCode 282. Expression Add Operators: DepthFirstSearch.Traverse over an implicit
// graph of partial expressions - ARCHITECTURE.md's own "chess position, generated on
// demand" illustration for this exact primitive. Each ExprState carries the
// expression built so far, so no two distinct operand/operator choices can ever
// produce an equal node - Traverse's visited-set cycle guard never actually fires,
// making this a plain generate-every-path backtrack for free.
public sealed partial class ExpressionAddOperatorsTests
{
    [Theory]
    [InlineData("123", 6, new[] { "1+2+3", "1*2*3" })]
    [InlineData("232", 8, new[] { "2*3+2", "2+3*2" })]
    [InlineData("105", 5, new[] { "1*0+5", "10-5" })]
    [InlineData("00", 0, new[] { "0*0", "0+0", "0-0" })]
    [InlineData("1", 5, new string[] { })]
    public void AddOperators_LeetCodeExamples_ReturnsEveryExpressionEvaluatingToTarget(string num, int target, string[] expected)
    {
        var actual = AddOperators(num, target);

        Assert.Equal(expected.OrderBy(x => x, StringComparer.Ordinal), actual.OrderBy(x => x, StringComparer.Ordinal));
    }

    private readonly record struct ExprState(int Position, long Value, long LastOperand, string Expression);

    private static List<string> AddOperators(string num, int target)
    {
        var start = new ExprState(0, 0, 0, string.Empty);
        var visited = DepthFirstSearch.Traverse(start, state => Successors(num, state));

        return visited
            .Where(state => state.Position == num.Length && state.Value == target)
            .Select(state => state.Expression)
            .ToList();
    }

    private static IEnumerable<ExprState> Successors(string num, ExprState state)
    {
        if (state.Position == num.Length)
        {
            yield break;
        }

        for (var length = 1; state.Position + length <= num.Length; length++)
        {
            var operand = num.Substring(state.Position, length);
            if (length > 1 && operand[0] == '0')
            {
                // A longer operand starting here would carry the same leading zero.
                yield break;
            }

            var operandValue = long.Parse(operand);
            var nextPosition = state.Position + length;

            if (state.Position == 0)
            {
                // The first operand has no operator in front of it.
                yield return new ExprState(nextPosition, operandValue, operandValue, operand);
                continue;
            }

            yield return new ExprState(nextPosition, state.Value + operandValue, operandValue, state.Expression + "+" + operand);
            yield return new ExprState(nextPosition, state.Value - operandValue, -operandValue, state.Expression + "-" + operand);
            yield return new ExprState(
                nextPosition,
                state.Value - state.LastOperand + (state.LastOperand * operandValue),
                state.LastOperand * operandValue,
                state.Expression + "*" + operand);
        }
    }
}
