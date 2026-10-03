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

    // LC 150's four operator tokens.
    public const string Plus = "+";
    public const string Minus = "-";
    public const string Times = "*";
    public const string Divide = "/";

    private static readonly string[] Operators = [Plus, Minus, Times, Divide];

    public static (string[] Tokens, int Value) Build(int operandCount, Random random)
    {
        var tokens = new List<string>((OperandsPerOperator * operandCount) - 1);
        var values = new Stack<long>();
        var operandsLeft = operandCount;

        while (operandsLeft > 0 || values.Count > 1)
        {
            if (ShouldPushOperand(operandsLeft, values.Count, random))
            {
                var operand = PushOperand(values, random);
                tokens.Add(operand);
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

    // An operand is due while operands remain and the stack cannot feed an operator yet, and
    // otherwise on a seeded coin flip; the coin is only drawn when both are possible.
    private static bool ShouldPushOperand(int operandsLeft, int stackDepth, Random random)
    {
        if (operandsLeft == 0)
        {
            return false;
        }

        return stackDepth < OperandsPerOperator || random.Next(OperandsPerOperator) == 0;
    }

    private static string PushOperand(Stack<long> values, Random random)
    {
        var operand = random.Next(-MaxOperand, MaxOperand + 1);
        values.Push(operand);

        return operand.ToString(CultureInfo.InvariantCulture);
    }

    // Tries the operators from a seeded starting point and takes the first whose result stays in
    // bounds. One of + and - always does: for operands of the same sign their difference, and for
    // opposite signs their sum, is no larger in magnitude than the larger operand - so the loop
    // returns before its end, and the fallback below is the + or - it would have found.
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

        if (Math.Abs(left + right) <= Math.Abs(left - right))
        {
            return (Plus, left + right);
        }

        return (Minus, left - right);
    }

    private static long? Apply(string token, long left, long right) => token switch
    {
        Plus => left + right,
        Minus => left - right,
        Times => left * right,
        _ => TruncatedQuotient(left, right),
    };

    // LC 150's division truncates toward zero, as long division does; null marks a division by zero.
    private static long? TruncatedQuotient(long left, long right)
    {
        if (right == 0)
        {
            return null;
        }

        return left / right;
    }
}
