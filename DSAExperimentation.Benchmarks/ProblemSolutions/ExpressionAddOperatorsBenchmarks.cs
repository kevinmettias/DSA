using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Expression Add Operators (LC 282): a hand-rolled recursive backtrack vs. this
// repo's own DepthFirstSearch.Traverse walking the identical implicit graph of
// partial expressions (see ExpressionAddOperatorsTests). Both explore the same
// O(4^n) state space - Target is unreachable so neither strategy ever short-circuits -
// so this is a constant-factor/allocation comparison, not an asymptotic-class split,
// the same framing IntegerToEnglishWordsBenchmarks uses for a bounded domain.
[MemoryDiagnoser]
public class ExpressionAddOperatorsBenchmarks
{
    private const long UnreachableTarget = long.MinValue;

    private const string AdditionOperator = "+";

    private const string SubtractionOperator = "-";

    private const string MultiplicationOperator = "*";

    [Params("1234567", "123456789")]
    public string Num = "";

    [Benchmark(Baseline = true)]
    public int RecursiveBacktrack() => CountMatches(0, 0, 0);

    private int CountMatches(int position, long value, long lastOperand)
    {
        if (position == Num.Length)
        {
            return value == UnreachableTarget ? 1 : 0;
        }

        var matches = 0;

        for (var length = 1; position + length <= Num.Length; length++)
        {
            var operand = Num.Substring(position, length);
            if (length > 1 && operand[0] == '0')
            {
                break;
            }

            matches += CountMatchesForOperand(position, value, lastOperand, operand);
        }

        return matches;
    }

    private int CountMatchesForOperand(int position, long value, long lastOperand, string operand)
    {
        var operandValue = long.Parse(operand);
        var nextPosition = position + operand.Length;

        if (position == 0)
        {
            return CountMatches(nextPosition, operandValue, operandValue);
        }

        return CountMatches(nextPosition, value + operandValue, operandValue)
             + CountMatches(nextPosition, value - operandValue, -operandValue)
             + CountMatches(nextPosition, value - lastOperand + (lastOperand * operandValue), lastOperand * operandValue);
    }

    [Benchmark]
    public int TraverseComposed()
    {
        var start = new ExprState(0, 0, 0, string.Empty);
        var visited = DepthFirstSearch.Traverse(start, state => Successors(state));
        return visited.Count(state => state.Position == Num.Length && state.Value == UnreachableTarget);
    }

    private IEnumerable<ExprState> Successors(ExprState state)
    {
        if (state.Position == Num.Length)
        {
            yield break;
        }

        for (var length = 1; state.Position + length <= Num.Length; length++)
        {
            var operand = Num.Substring(state.Position, length);
            if (length > 1 && operand[0] == '0')
            {
                yield break;
            }

            foreach (var next in SuccessorsForOperand(state, operand))
            {
                yield return next;
            }
        }
    }

    private static IEnumerable<ExprState> SuccessorsForOperand(ExprState state, string operand)
    {
        var operandValue = long.Parse(operand);
        var nextPosition = state.Position + operand.Length;

        if (state.Position == 0)
        {
            yield return new ExprState(nextPosition, operandValue, operandValue, operand);
            yield break;
        }

        yield return new ExprState(nextPosition, state.Value + operandValue, operandValue, state.Expression + AdditionOperator + operand);
        yield return new ExprState(nextPosition, state.Value - operandValue, -operandValue, state.Expression + SubtractionOperator + operand);
        yield return new ExprState(
            nextPosition,
            state.Value - state.LastOperand + (state.LastOperand * operandValue),
            state.LastOperand * operandValue,
            state.Expression + MultiplicationOperator + operand);
    }

    // Expression carries the full path so far, the same reason ExpressionAddOperatorsTests
    // gives: it guarantees every node is distinct, so Traverse's visited-set guard never
    // merges two different operand/operator choices and this stays a true full enumeration.
    private readonly record struct ExprState(int Position, long Value, long LastOperand, string Expression);
}
