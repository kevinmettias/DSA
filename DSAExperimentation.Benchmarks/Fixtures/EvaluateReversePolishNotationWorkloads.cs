using System.Diagnostics;
using System.Globalization;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 150: a valid postfix expression of operandCount
// operands drawn from LC's [-200, 200] and operandCount - 1 operators, with the value
// it evaluates to. It is built by evaluating as it goes: after each operand a coin
// decides whether to reduce the top two values, so the expression tree takes a
// seeded shape, and an operator is only emitted when its result stays within
// IntermediateBound - a division only onto a nonzero divisor. That keeps LC's
// promises: no division by zero, and every intermediate value fits a 32-bit int.
internal static class EvaluateReversePolishNotationWorkloads
{
    private const int MaxOperand = 200;
    private const int IntermediateBound = 1_000_000;
    private const int OperandsPerOperator = 2;

    private static readonly string[] Operators = ["+", "-", "*", "/"];

    public static (string[] Tokens, int Value) Build(int operandCount, Random random)
    {
        var tokens = new List<string>((OperandsPerOperator * operandCount) - 1);
        var values = new Stack<long>();
        var operandsLeft = operandCount;

        while (operandsLeft > 0 || values.Count > 1)
        {
            if (operandsLeft > 0 && (values.Count < OperandsPerOperator || random.Next(OperandsPerOperator) == 0))
            {
                var operand = random.Next(-MaxOperand, MaxOperand + 1);
                tokens.Add(operand.ToString(CultureInfo.InvariantCulture));
                values.Push(operand);
                operandsLeft--;
                continue;
            }

            var right = values.Pop();
            var left = values.Pop();
            var (token, value) = PickOperator(left, right, random);
            tokens.Add(token);
            values.Push(value);
        }

        return ([.. tokens], (int)values.Pop());
    }

    // Tries the operators from a seeded starting point and takes the first whose result stays in
    // bounds. One of + and - always does: for operands of the same sign their difference, and for
    // opposite signs their sum, is no larger in magnitude than the larger operand.
    private static (string Token, long Value) PickOperator(long left, long right, Random random)
    {
        var start = random.Next(Operators.Length);

        for (var offset = 0; offset < Operators.Length; offset++)
        {
            var token = Operators[(start + offset) % Operators.Length];

            if (Apply(token, left, right) is { } value && Math.Abs(value) <= IntermediateBound)
            {
                return (token, value);
            }
        }

        throw new UnreachableException("One of + and - always stays within the bound.");
    }

    // LC 150's division truncates toward zero, as long division does; null marks a division by zero.
    private static long? Apply(string token, long left, long right) => token switch
    {
        "+" => left + right,
        "-" => left - right,
        "*" => left * right,
        _ => right == 0 ? null : left / right,
    };
}
