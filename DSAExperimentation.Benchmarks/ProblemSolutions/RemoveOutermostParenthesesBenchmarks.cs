using BenchmarkDotNet.Attributes;
using System.Text;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Outermost Parentheses (LC 1021): a plain running-depth counter (baseline
// - no unmatched opener is ever stored, just its count) vs. this repo's own
// Stack<char> holding each unmatched opener explicitly, the same LIFO primitive
// ValidParenthesesTests/MinimumAddToMakeParenthesesValidBenchmarks already use -
// here its Count stands in for the running depth instead of validity-checking.
[MemoryDiagnoser]
public class RemoveOutermostParenthesesBenchmarks
{
    private const int MaxDepth = 10;

    [Params(1_000, 20_000)]
    public int PairCount;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1021);
        _expression = GenerateBalanced(PairCount, MaxDepth, random);
    }

    [Benchmark(Baseline = true)]
    public string RunningDepthCounter()
    {
        var s = _expression;
        var result = new StringBuilder(s.Length);
        var depth = 0;

        foreach (var c in s)
        {
            if (c == '(')
            {
                if (depth > 0)
                {
                    result.Append(c);
                }

                depth++;
            }
            else
            {
                depth--;

                if (depth > 0)
                {
                    result.Append(c);
                }
            }
        }

        return result.ToString();
    }

    [Benchmark]
    public string StackOfOpeners()
    {
        var s = _expression;
        var openers = new RepoCharStack();
        var result = new StringBuilder(s.Length);

        foreach (var c in s)
        {
            if (c == '(')
            {
                if (openers.Count > 0)
                {
                    result.Append(c);
                }

                openers.Push(c);
            }
            else
            {
                openers.TryPop(out _);

                if (openers.Count > 0)
                {
                    result.Append(c);
                }
            }
        }

        return result.ToString();
    }

    // Generates a concatenation of many independent balanced primitives (each
    // one a run back down to depth 0) with nesting depth capped at maxDepth -
    // the same shape ScoreOfParenthesesBenchmarks' generator uses, reused here
    // because it already produces the "many sibling primitives" structure this
    // problem's own worst case needs.
    private static string GenerateBalanced(int pairCount, int maxDepth, Random random)
    {
        var result = new char[pairCount * 2];
        var openCount = 0;
        var closeCount = 0;

        for (var i = 0; i < result.Length; i++)
        {
            var depth = openCount - closeCount;
            var canOpen = openCount < pairCount && depth < maxDepth;
            var canClose = closeCount < openCount;

            if (canOpen && (!canClose || random.Next(2) == 0))
            {
                result[i] = '(';
                openCount++;
            }
            else
            {
                result[i] = ')';
                closeCount++;
            }
        }

        return new string(result);
    }
}
