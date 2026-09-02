using BenchmarkDotNet.Attributes;
using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check if a Parentheses String Can Be Valid (LC 2116): the same reachable-open-count
// DP ValidParenthesisStringBenchmarks (LC 678) uses for '*' - generalized so a free
// position (locked[i] == '0') gets a two-way transition (it must become a real
// bracket, unlike '*' it can never vanish) - vs. this repo's own two Stack<int>
// (RepoIndexStack, ValidParenthesisStringBenchmarks precedent) sweeping unmatched
// '(' and unmatched free-position indices in one O(n) left-to-right pass. _s/_locked
// is a leading locked '(' + an all-free middle + a trailing locked ')', maximizing
// the DP's reachable-set growth every step while the stack sweep never even
// inspects its free-index stack's contents.
[MemoryDiagnoser]
public class CheckIfAParenthesesStringCanBeValidBenchmarks
{
    private const char OpenParenthesis = '(';
    private const int BoundaryParenthesisCount = 2; // one leading locked '(' + one trailing locked ')'
    private const string LockedMarker = "1"; // locked-position digit in the `locked` string

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private string _locked = null!;

    [GlobalSetup]
    public void Setup()
    {
        _s = OpenParenthesis + new string('(', Length - BoundaryParenthesisCount) + ')';
        _locked = LockedMarker + new string('0', Length - BoundaryParenthesisCount) + LockedMarker;
    }

    [Benchmark(Baseline = true)]
    public bool ReachableOpenCountDp()
    {
        var reachable = new HashSet<int> { 0 };

        for (var i = 0; i < _s.Length; i++)
        {
            reachable = ComputeNextReachable(reachable, _s[i], _locked[i]);

            if (reachable.Count == 0)
            {
                return false;
            }
        }

        return reachable.Contains(0);
    }

    [Benchmark]
    public bool TwoIndexStackSweep()
    {
        var openIndices = new RepoIndexStack();
        var freeIndices = new RepoIndexStack();

        if (!TryMatchClosingParens(_s, _locked, openIndices, freeIndices))
        {
            return false;
        }

        return AllOpensMatched(openIndices, freeIndices);
    }

    private static HashSet<int> ComputeNextReachable(HashSet<int> reachable, char c, char lockedChar)
    {
        var next = new HashSet<int>();

        foreach (var openCount in reachable)
        {
            if (lockedChar == '0')
            {
                AddFreeTransition(next, openCount);
            }
            else if (c == OpenParenthesis)
            {
                next.Add(openCount + 1);
            }
            else if (openCount > 0)
            {
                next.Add(openCount - 1);
            }
        }

        return next;
    }

    private static void AddFreeTransition(HashSet<int> next, int openCount)
    {
        next.Add(openCount + 1);

        if (openCount > 0)
        {
            next.Add(openCount - 1);
        }
    }

    private static bool TryMatchClosingParens(string s, string locked, RepoIndexStack openIndices, RepoIndexStack freeIndices)
    {
        for (var i = 0; i < s.Length; i++)
        {
            if (locked[i] == '0')
            {
                freeIndices.Push(i);
            }
            else if (s[i] == OpenParenthesis)
            {
                openIndices.Push(i);
            }
            else if (!openIndices.TryPop(out _) && !freeIndices.TryPop(out _))
            {
                return false;
            }
        }

        return true;
    }

    private static bool AllOpensMatched(RepoIndexStack openIndices, RepoIndexStack freeIndices)
    {
        while (openIndices.TryPop(out var openIndex))
        {
            if (!freeIndices.TryPop(out var freeIndex) || freeIndex < openIndex)
            {
                return false;
            }
        }

        return true;
    }
}
