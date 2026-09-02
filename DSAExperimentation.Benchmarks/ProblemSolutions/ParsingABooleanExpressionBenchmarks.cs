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
    // random.Next(LeafChance) == 0: roughly a 1-in-4 chance to end the generated
    // expression early, independent of remaining depth.
    private const int LeafChance = 4;

    // random.Next(TokenChoiceCount): coin flip between the two leaf tokens below.
    private const int TokenChoiceCount = 2;
    private const string TrueToken = "t";
    private const string FalseToken = "f";

    // random.Next(OperatorChoiceCount): choose among !, &, |.
    private const int OperatorChoiceCount = 3;

    // Length of the operator character plus its following '(' - consumed together.
    private const int OperatorAndParenLength = 2;

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
        if (depth == 0 || random.Next(LeafChance) == 0)
        {
            return random.Next(TokenChoiceCount) == 0 ? TrueToken : FalseToken;
        }

        return random.Next(OperatorChoiceCount) switch
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

        index += OperatorAndParenLength; // consume the operator character and its '('

        return c == '!'
            ? ParseNot(expression, ref index)
            : ParseOperands(expression, c, ref index);
    }

    // Consumes the operand of a unary '!' and its closing ')'.
    private static bool ParseNot(string expression, ref int index)
    {
        var value = ParseExpression(expression, ref index);
        index++; // consume ')'
        return !value;
    }

    // Consumes the comma-separated operands of an n-ary '&'/'|' up to its closing
    // ')', folding them with the operator as they're parsed.
    private static bool ParseOperands(string expression, char op, ref int index)
    {
        var result = op == '&';
        while (expression[index] != ')')
        {
            var operand = ParseExpression(expression, ref index);
            result = op == '&' ? result && operand : result || operand;

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
        CountOperands(stack, out var trueCount, out var falseCount);
        var op = ConsumeGroupOperator(stack);
        var value = EvaluateOperator(op, trueCount, falseCount);
        return ToBooleanToken(value);
    }

    // Pops every operand of the innermost group (down to, but not including, its
    // opening '(') and tallies how many were true vs. false.
    private static void CountOperands(RepoCharStack stack, out int trueCount, out int falseCount)
    {
        trueCount = 0;
        falseCount = 0;

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
    }

    // Pops the group's opening '(' and its operator character, returning the operator.
    private static char ConsumeGroupOperator(RepoCharStack stack)
    {
        stack.TryPop(out _); // the matching '('
        stack.TryPop(out var op);
        return op;
    }

    private static bool EvaluateOperator(char op, int trueCount, int falseCount) => op switch
    {
        '!' => trueCount == 0,
        '&' => falseCount == 0,
        _ => trueCount > 0, // '|'
    };

    private static char ToBooleanToken(bool value) => value ? 't' : 'f';
}
