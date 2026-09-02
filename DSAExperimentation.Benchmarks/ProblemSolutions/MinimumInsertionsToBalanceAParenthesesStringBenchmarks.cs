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
    private const int RandomSeed = 1541; // LC problem number
    private const int BracketKindCount = 2;
    private const int ClosersPerOpener = 2;

    [Params(1_000, 20_000)]
    public int Length;

    private string _brackets = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = random.Next(0, BracketKindCount) == 0 ? '(' : ')';
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
            needed = ch == '(' ? ProcessOpener(needed, ref insertions) : ProcessCloser(needed, ref insertions);
        }

        return insertions + needed;
    }

    private static int ProcessOpener(int needed, ref int insertions)
    {
        needed += ClosersPerOpener;

        if (needed % ClosersPerOpener == 1)
        {
            insertions++;
            needed--;
        }

        return needed;
    }

    private static int ProcessCloser(int needed, ref int insertions)
    {
        needed--;

        if (needed == -1)
        {
            insertions++;
            needed = 1;
        }

        return needed;
    }

    [Benchmark]
    public int StackOfOpeners()
    {
        var openers = new RepoCharStack();
        var insertions = 0;
        var i = 0;

        while (i < _brackets.Length)
        {
            i = ProcessOpenerStackStep(openers, i, ref insertions);
        }

        return insertions + (openers.Count * ClosersPerOpener);
    }

    private int ProcessOpenerStackStep(RepoCharStack openers, int i, ref int insertions)
    {
        if (_brackets[i] == '(')
        {
            openers.Push(_brackets[i]);
            return i + 1;
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

        return i + (hasAdjacentCloser ? ClosersPerOpener : 1);
    }
}
