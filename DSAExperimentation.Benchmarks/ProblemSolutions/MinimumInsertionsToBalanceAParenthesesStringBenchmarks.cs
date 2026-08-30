using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Insertions to Balance a Parentheses String (LC 1541): a plain running
// counter of "closers still needed for currently-open '('" (baseline - no opener is
// ever stored, just a scalar need-count) vs. this repo's Stack<char> holding each
// unmatched opener explicitly, the same LIFO primitive
// MinimumAddToMakeParenthesesValidBenchmarks already uses for the sibling problem.
[MemoryDiagnoser]
public class MinimumInsertionsToBalanceAParenthesesStringBenchmarks
{
    [Params(1_000, 20_000)]
    public int Length;

    private string _brackets = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1541);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = random.Next(0, 2) == 0 ? '(' : ')';
        }

        _brackets = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int RunningCounter()
    {
        var needed = 0;
        var insertions = 0;

        foreach (var ch in _brackets)
        {
            if (ch == '(')
            {
                needed += 2;
                if (needed % 2 == 1)
                {
                    insertions++;
                    needed--;
                }
            }
            else
            {
                needed--;
                if (needed == -1)
                {
                    insertions++;
                    needed = 1;
                }
            }
        }

        return insertions + needed;
    }

    [Benchmark]
    public int StackOfOpeners()
    {
        var openers = new RepoCharStack();
        var insertions = 0;
        var i = 0;

        while (i < _brackets.Length)
        {
            if (_brackets[i] == '(')
            {
                openers.Push(_brackets[i]);
                i++;
                continue;
            }

            var hasAdjacentCloser = i + 1 < _brackets.Length && _brackets[i + 1] == ')';
            if (!hasAdjacentCloser)
            {
                insertions++;
            }

            if (!openers.TryPop(out _))
            {
                insertions++;
            }

            i += hasAdjacentCloser ? 2 : 1;
        }

        return insertions + (openers.Count * 2);
    }
}
