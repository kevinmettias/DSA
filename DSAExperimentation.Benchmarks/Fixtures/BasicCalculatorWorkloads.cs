using System.Text;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 224 - a synthetic expression string of at
// least the requested Length, built from many *sequential*, only
// single-level-nested "+(a+b)"/"-(a+b)" groups so recursion depth stays
// constant (~2) as Length grows instead of risking a StackOverflowException
// in the recursive-descent baseline.
internal static class BasicCalculatorWorkloads
{
    // Operand the expression is seeded with before appending groups.
    private const string InitialOperand = "0";

    // n cycles between the two operator tokens every other generated group.
    private const int OperatorAlternationModulus = 4;

    // Each generated group consumes two operands (n, n + 1), so n advances by 2.
    private const int OperandStep = 2;

    private const string PlusOpenParen = "+(";
    private const string MinusOpenParen = "-(";

    public static string BuildExpression(int length)
    {
        var builder = new StringBuilder(InitialOperand);
        var n = 1;

        while (builder.Length < length)
        {
            builder.Append(n % OperatorAlternationModulus == 1 ? PlusOpenParen : MinusOpenParen).Append(n).Append('+').Append(n + 1).Append(')');
            n += OperandStep;
        }

        return builder.ToString();
    }
}
