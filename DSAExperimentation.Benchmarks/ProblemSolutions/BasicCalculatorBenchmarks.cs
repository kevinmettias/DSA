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
        var result = 0;
        var sign = 1;
        var i = 0;

        while (i < _expression.Length)
        {
            var c = _expression[i];

            if (char.IsDigit(c))
            {
                var number = 0;

                while (i < _expression.Length && char.IsDigit(_expression[i]))
                {
                    number = number * 10 + (_expression[i] - '0');
                    i++;
                }

                result += sign * number;
                continue;
            }

            switch (c)
            {
                case '+':
                    sign = 1;
                    break;
                case '-':
                    sign = -1;
                    break;
                case '(':
                    stack.Push((result, sign));
                    result = 0;
                    sign = 1;
                    break;
                case ')':
                    stack.TryPop(out var outer);
                    result = outer.Result + outer.Sign * result;
                    break;
            }

            i++;
        }

        return result;
    }

    private static int EvaluateRecursive(string expression, ref int i)
    {
        var result = 0;
        var sign = 1;

        while (i < expression.Length)
        {
            var c = expression[i];

            if (char.IsDigit(c))
            {
                var number = 0;

                while (i < expression.Length && char.IsDigit(expression[i]))
                {
                    number = number * 10 + (expression[i] - '0');
                    i++;
                }

                result += sign * number;
                continue;
            }

            switch (c)
            {
                case '+':
                    sign = 1;
                    break;
                case '-':
                    sign = -1;
                    break;
                case '(':
                    i++;
                    result += sign * EvaluateRecursive(expression, ref i);
                    break;
                case ')':
                    return result;
            }

            i++;
        }

        return result;
    }

    private static string BuildExpression(int length)
    {
        var builder = new StringBuilder("0");
        var n = 1;

        while (builder.Length < length)
        {
            builder.Append(n % 4 == 1 ? "+(" : "-(").Append(n).Append('+').Append(n + 1).Append(')');
            n += 2;
        }

        return builder.ToString();
    }
}
