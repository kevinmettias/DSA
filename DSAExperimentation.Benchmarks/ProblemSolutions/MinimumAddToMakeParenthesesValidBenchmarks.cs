using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Add to Make Parentheses Valid (LC 921): a plain running-counter balance
// walk (baseline - no unmatched opener is ever stored, just its count) vs. this
// repo's Stack<char> holding each unmatched opener explicitly, the same LIFO
// primitive ValidParenthesesTests/ReverseIntegerBenchmarks already use.
[MemoryDiagnoser]
public class MinimumAddToMakeParenthesesValidBenchmarks
{
    [Params(1_000, 20_000)]
    public int Length;

    private string _brackets = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(921);
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
        var openBalance = 0;
        var insertions = 0;

        foreach (var ch in _brackets)
        {
            if (ch == '(')
            {
                openBalance++;
            }
            else if (openBalance > 0)
            {
                openBalance--;
            }
            else
            {
                insertions++;
            }
        }

        return insertions + openBalance;
    }

    [Benchmark]
    public int StackOfOpeners()
    {
        var openers = new RepoCharStack();
        var unmatchedClosers = 0;

        foreach (var ch in _brackets)
        {
            if (ch == '(')
            {
                openers.Push(ch);
            }
            else if (!openers.TryPop(out _))
            {
                unmatchedClosers++;
            }
        }

        return unmatchedClosers + openers.Count;
    }
}
