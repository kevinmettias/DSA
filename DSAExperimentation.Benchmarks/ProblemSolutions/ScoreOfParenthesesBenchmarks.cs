using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Score of Parentheses (LC 856): the same balanced string (nesting depth capped
// so 2^depth never overflows int - LeetCode's own constraints keep real inputs
// this small too) is scored two ways. NestedDepthScan locates every atomic "()"
// pair and, for each one, rescans everything before it to recompute its
// nesting depth from scratch - O(n) work per pair, O(n^2) overall.
// MonotonicStackFold instead composes this repo's own Stack<int>
// (DailyTemperatures/AsteroidCollision precedent), one sentinel-seeded
// left-to-right pass where each ')' folds its own just-closed scope's score
// into the score of the scope enclosing it - O(n), one push/pop per character.
[MemoryDiagnoser]
public class ScoreOfParenthesesBenchmarks
{
    private const int MaxDepth = 10;

    [Params(100, 2_000)]
    public int PairCount;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(856);
        _expression = GenerateBalanced(PairCount, MaxDepth, random);
    }

    [Benchmark(Baseline = true)]
    public int NestedDepthScan()
    {
        var s = _expression;
        var total = 0;

        for (var i = 0; i < s.Length - 1; i++)
        {
            if (s[i] != '(' || s[i + 1] != ')')
            {
                continue;
            }

            var depth = 0;

            for (var j = 0; j < i; j++)
            {
                depth += s[j] == '(' ? 1 : -1;
            }

            total += 1 << depth;
        }

        return total;
    }

    [Benchmark]
    public int MonotonicStackFold()
    {
        var s = _expression;
        var scores = new RepoIntStack();
        scores.Push(0);

        foreach (var c in s)
        {
            if (c == '(')
            {
                scores.Push(0);
                continue;
            }

            scores.TryPop(out var inner);
            scores.TryPop(out var outer);
            scores.Push(outer + Math.Max(2 * inner, 1));
        }

        scores.TryPop(out var result);
        return result;
    }

    // Generates a valid balanced-parentheses string of PairCount atomic pairs
    // with nesting depth capped at maxDepth - unbounded depth would make
    // NestedDepthScan's 1 << depth (and MonotonicStackFold's 2 * inner)
    // silently overflow int, which real LeetCode inputs (length <= 50) never
    // approach either.
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
