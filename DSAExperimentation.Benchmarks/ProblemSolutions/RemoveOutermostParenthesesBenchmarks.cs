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

    private const int RandomSeed = 1021; // LeetCode problem number

    private const int CharsPerPair = 2;

    private const int CoinFlipUpperBoundExclusive = 2;

    [Params(1_000, 20_000)]
    public int PairCount;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _expression = GenerateBalanced(PairCount, MaxDepth, random);
    }

    [Benchmark(Baseline = true)]
    public string RunningDepthCounter()
    {
        var result = new StringBuilder(_expression.Length);
        var depth = 0;

        foreach (var c in _expression)
        {
            depth = AppendIfInner(result, c, depth);
        }

        return result.ToString();
    }

    private static int AppendIfInner(StringBuilder result, char c, int depth)
    {
        if (c == '(')
        {
            if (depth > 0)
            {
                result.Append(c);
            }

            return depth + 1;
        }

        depth--;

        if (depth > 0)
        {
            result.Append(c);
        }

        return depth;
    }

    [Benchmark]
    public string StackOfOpeners()
    {
        var openers = new RepoCharStack();
        var result = new StringBuilder(_expression.Length);

        foreach (var c in _expression)
        {
            AppendIfNested(openers, result, c);
        }

        return result.ToString();
    }

    private static void AppendIfNested(RepoCharStack openers, StringBuilder result, char c)
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

    private readonly record struct ParenCounts(int Open, int Close);

    // Generates a concatenation of many independent balanced primitives (each
    // one a run back down to depth 0) with nesting depth capped at maxDepth -
    // the same shape ScoreOfParenthesesBenchmarks' generator uses, reused here
    // because it already produces the "many sibling primitives" structure this
    // problem's own worst case needs.
    private static string GenerateBalanced(int pairCount, int maxDepth, Random random)
    {
        var result = new char[pairCount * CharsPerPair];
        var counts = new ParenCounts(0, 0);

        for (var i = 0; i < result.Length; i++)
        {
            (result[i], counts) = ChooseNextParen(pairCount, maxDepth, random, counts);
        }

        return new string(result);
    }

    private static (char Chosen, ParenCounts Counts) ChooseNextParen(
        int pairCount, int maxDepth, Random random, ParenCounts counts)
    {
        var depth = counts.Open - counts.Close;
        var canOpen = counts.Open < pairCount && depth < maxDepth;
        var canClose = counts.Close < counts.Open;

        if (canOpen && (!canClose || random.Next(CoinFlipUpperBoundExclusive) == 0))
        {
            return ('(', counts with { Open = counts.Open + 1 });
        }

        return (')', counts with { Close = counts.Close + 1 });
    }
}
