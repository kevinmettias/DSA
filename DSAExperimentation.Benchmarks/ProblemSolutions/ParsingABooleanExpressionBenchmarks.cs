using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Parsing A Boolean Expression (LC 1106): a recursive-descent parser leaning on the
// CLR call stack vs. this repo's own Stack<char> as an explicit, iterative parser
// stack - the same head-to-head MultiplyStringsBenchmarks already runs (digit-by-digit
// arithmetic) for a different problem. Depth controls how deeply the generated
// expression nests (and therefore its total size), not the value it evaluates to.
[MemoryDiagnoser]
public class ParsingABooleanExpressionBenchmarks
{
    [Params(8, 12)]
    public int Depth;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _expression = Generate(random, Depth);
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveDescent()
    {
        var index = 0;
        return ParseExpression(_expression, ref index);
    }

    [Benchmark]
    public bool StackBased() => Parse(_expression);

    private static string Generate(Random random, int depth)
    {
        if (depth == 0 || random.Next(4) == 0)
        {
            return random.Next(2) == 0 ? "t" : "f";
        }

        return random.Next(3) switch
        {
            0 => $"!({Generate(random, depth - 1)})",
            1 => $"&({Generate(random, depth - 1)},{Generate(random, depth - 1)})",
            _ => $"|({Generate(random, depth - 1)},{Generate(random, depth - 1)})",
        };
    }

    // Consumes one sub-expression starting at index, advancing index past it - the
    // baseline every other solution to this problem reaches for first.
    private static bool ParseExpression(string expression, ref int index)
    {
        var c = expression[index];

        if (c is 't' or 'f')
        {
            index++;
            return c == 't';
        }

        index += 2; // consume the operator character and its '('

        if (c == '!')
        {
            var value = ParseExpression(expression, ref index);
            index++; // consume ')'
            return !value;
        }

        var result = c == '&';
        while (expression[index] != ')')
        {
            var operand = ParseExpression(expression, ref index);
            result = c == '&' ? result && operand : result || operand;

            if (expression[index] == ',')
            {
                index++;
            }
        }

        index++; // consume ')'
        return result;
    }

    private static bool Parse(string expression)
    {
        var stack = new RepoCharStack();

        foreach (var c in expression)
        {
            if (c == ',')
            {
                continue;
            }

            if (c != ')')
            {
                stack.Push(c);
                continue;
            }

            stack.Push(EvaluateGroup(stack));
        }

        stack.TryPop(out var result);
        return result == 't';
    }

    private static char EvaluateGroup(RepoCharStack stack)
    {
        var trueCount = 0;
        var falseCount = 0;

        while (stack.TryPeek(out var top) && top != '(')
        {
            stack.TryPop(out var operand);
            if (operand == 't')
            {
                trueCount++;
            }
            else
            {
                falseCount++;
            }
        }

        stack.TryPop(out _); // the matching '('
        stack.TryPop(out var op);

        var value = op switch
        {
            '!' => trueCount == 0,
            '&' => falseCount == 0,
            _ => trueCount > 0, // '|'
        };

        return value ? 't' : 'f';
    }
}
