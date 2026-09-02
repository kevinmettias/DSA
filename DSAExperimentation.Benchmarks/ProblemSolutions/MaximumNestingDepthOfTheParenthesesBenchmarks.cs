using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Nesting Depth of the Parentheses (LC 1614): a plain running-depth counter
// (baseline - no unmatched opener is ever stored, just its count) vs. this repo's
// own Stack<char> holding each unmatched opener explicitly, the same LIFO primitive
// RemoveOutermostParenthesesBenchmarks already compares against a counter for a
// sibling parentheses-depth problem - here its Count stands in for the running
// depth directly, with no output string to rebuild.
[MemoryDiagnoser]
public class MaximumNestingDepthOfTheParenthesesBenchmarks
{
    private const int MaxDepthCap = 20;

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1614;

    private const int CharsPerPair = 2;

    private const int CoinFlipRange = 2;

    [Params(1_000, 20_000)]
    public int PairCount;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _expression = GenerateBalanced(PairCount, MaxDepthCap, random);
    }

    [Benchmark(Baseline = true)]
    public int RunningDepthCounter()
    {
        var depth = 0;
        var maxDepth = 0;

        foreach (var c in _expression)
        {
            if (c == '(')
            {
                depth++;
                maxDepth = Math.Max(maxDepth, depth);
            }
            else if (c == ')')
            {
                depth--;
            }
        }

        return maxDepth;
    }

    [Benchmark]
    public int StackOfOpeners()
    {
        var openers = new RepoCharStack();
        var maxDepth = 0;

        foreach (var c in _expression)
        {
            if (c == '(')
            {
                openers.Push(c);
                maxDepth = Math.Max(maxDepth, openers.Count);
            }
            else if (c == ')')
            {
                openers.TryPop(out _);
            }
        }

        return maxDepth;
    }

    // Same "many independent balanced primitives, nesting depth capped" generator
    // shape ScoreOfParenthesesBenchmarks/RemoveOutermostParenthesesBenchmarks use.
    private static string GenerateBalanced(int pairCount, int maxDepth, Random random)
    {
        var result = new char[pairCount * CharsPerPair];
        var counts = (Open: 0, Close: 0);

        for (var i = 0; i < result.Length; i++)
        {
            (result[i], counts) = NextChar(pairCount, maxDepth, counts, random);
        }

        return new string(result);
    }

    private static (char Char, (int Open, int Close) Counts) NextChar(
        int pairCount,
        int maxDepth,
        (int Open, int Close) counts,
        Random random)
    {
        var depth = counts.Open - counts.Close;
        var canOpen = counts.Open < pairCount && depth < maxDepth;
        var canClose = counts.Close < counts.Open;

        if (canOpen && (!canClose || random.Next(CoinFlipRange) == 0))
        {
            return ('(', (counts.Open + 1, counts.Close));
        }

        return (')', (counts.Open, counts.Close + 1));
    }
}
