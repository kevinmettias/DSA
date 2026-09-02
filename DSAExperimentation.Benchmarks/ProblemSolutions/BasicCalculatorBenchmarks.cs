using System.Text;
using BenchmarkDotNet.Attributes;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Result, int Sign)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Basic Calculator (LC 224): a recursive-descent evaluator that relies on the CLR's
// own call stack (no repo primitive - the same "lighter repo-primitive fit" the
// Pow(x, n)/Rectangle Area benchmarks already accept) vs. a single left-to-right
// pass over this repo's own Stack<(int,int)> holding (result-so-far, sign) pairs
// across '(' / ')'. The generated expression chains many *sequential*, only
// single-level-nested "+(a+b)"/"-(a+b)" groups, so recursion depth stays constant
// (~2) as Length grows instead of risking a StackOverflowException.
[MemoryDiagnoser]
public class BasicCalculatorBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    // Base of the digit run accumulated by ParseNumber - decimal.
    private const int DecimalBase = 10;

    // Operand BuildExpression seeds the expression with before appending groups.
    private const string InitialOperand = "0";

    // n cycles between the two operator tokens every other generated group.
    private const int OperatorAlternationModulus = 4;

    // Each generated group consumes two operands (n, n + 1), so n advances by 2.
    private const int OperandStep = 2;

    private const string PlusOpenParen = "+(";
    private const string MinusOpenParen = "-(";

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() => _expression = BuildExpression(Length);

    [Benchmark(Baseline = true)]
    public int RecursiveDescent()
    {
        var i = 0;
        return EvaluateRecursive(_expression, ref i);
    }

    [Benchmark]
    public int StackScan()
    {
        var stack = new RepoStack();
        var state = (Result: 0, Sign: 1);
        var i = 0;

        while (i < _expression.Length)
        {
            ProcessToken(_expression, stack, ref i, ref state);
        }

        return state.Result;
    }

    private static void ProcessToken(string expression, RepoStack stack, ref int i, ref (int Result, int Sign) state)
    {
        var c = expression[i];

        if (char.IsDigit(c))
        {
            AccumulateOperand(expression, ref i, ref state);
            return;
        }

        ApplyOperatorToken(c, stack, ref state);
        i++;
    }

    private static void AccumulateOperand(string expression, ref int i, ref (int Result, int Sign) state)
        => state.Result += state.Sign * ParseNumber(expression, ref i);

    private static void ApplyOperatorToken(char token, RepoStack stack, ref (int Result, int Sign) state)
    {
        switch (token)
        {
            case '+':
                state.Sign = 1;
                break;
            case '-':
                state.Sign = -1;
                break;
            case '(':
                stack.Push(state);
                state = (0, 1);
                break;
            case ')':
                stack.TryPop(out var outer);
                state.Result = outer.Result + outer.Sign * state.Result;
                break;
        }
    }

    private static int EvaluateRecursive(string expression, ref int i)
    {
        var result = 0;
        var sign = 1;

        while (i < expression.Length)
        {
            if (!TryConsumeToken(expression, ref i, ref result, ref sign))
            {
                return result;
            }
        }

        return result;
    }

    private static bool TryConsumeToken(string expression, ref int i, ref int result, ref int sign)
    {
        var c = expression[i];

        if (char.IsDigit(c))
        {
            result += sign * ParseNumber(expression, ref i);
            return true;
        }

        if (c == ')')
        {
            return false;
        }

        if (c == '(')
        {
            result += sign * ConsumeGroup(expression, ref i);
            return true;
        }

        sign = ApplyOperatorSign(c);
        i++;
        return true;
    }

    private static int ConsumeGroup(string expression, ref int i)
    {
        i++;
        var value = EvaluateRecursive(expression, ref i);
        i++;
        return value;
    }

    private static int ApplyOperatorSign(char token) => token == '+' ? 1 : -1;

    private static int ParseNumber(string expression, ref int i)
    {
        var number = 0;

        while (i < expression.Length && char.IsDigit(expression[i]))
        {
            number = number * DecimalBase + (expression[i] - '0');
            i++;
        }

        return number;
    }

    private static string BuildExpression(int length)
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
