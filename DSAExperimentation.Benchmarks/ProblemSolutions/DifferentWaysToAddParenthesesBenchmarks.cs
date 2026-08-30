using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Different Ways to Add Parentheses (LC 241): plain recursive split/combine with no
// cache vs. this repo's own Memoizer<TState,TResult> keyed by substring. The
// expression repeats the same operand ("1+1+...+1"), so the same substrings ("1",
// "1+1", ...) recur across many different split points - recomputed from scratch
// every time by the un-memoized recursion, resolved once and reused by Memoizer.
[MemoryDiagnoser]
public class DifferentWaysToAddParenthesesBenchmarks
{
    [Params(6, 10)]
    public int OperandCount;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() => _expression = string.Join('+', Enumerable.Repeat("1", OperandCount));

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Evaluate(_expression).Count;

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<string, List<int>>(_expression, Evaluate).Count;

    private static List<int> Evaluate(string expression) => Evaluate(expression, Evaluate);

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
