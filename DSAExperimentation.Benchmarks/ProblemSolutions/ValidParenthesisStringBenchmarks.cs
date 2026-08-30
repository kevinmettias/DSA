using BenchmarkDotNet.Attributes;
using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Parenthesis String (LC 678): the textbook DP that tracks every open-paren
// count reachable at each position (in a plain HashSet<int>, no repo primitive - the
// "one interpretation per '*'" state explosion this problem is famous for, O(n^2)
// worst case since the reachable set can grow by one value per character) vs. this
// repo's own two Stack<int> (ValidParenthesesBenchmarks/NextGreaterElementIBenchmarks
// precedent) tracking unmatched '(' and '*' indices in one O(n) left-to-right pass.
// _s is "(" + all '*' + ")", the shape that maximizes the DP's reachable-set growth
// every step while the stack sweep never even inspects its star stack's contents.
[MemoryDiagnoser]
public class ValidParenthesisStringBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup() => _s = BuildInput(Length);

    [Benchmark(Baseline = true)]
    public bool ReachableOpenCountDp()
    {
        var reachable = new HashSet<int> { 0 };

        foreach (var c in _s)
        {
            var next = new HashSet<int>();

            foreach (var openCount in reachable)
            {
                switch (c)
                {
                    case '(':
                        next.Add(openCount + 1);
                        break;
                    case ')':
                        if (openCount > 0)
                        {
                            next.Add(openCount - 1);
                        }
                        break;
                    default:
                        next.Add(openCount + 1);
                        next.Add(openCount);

                        if (openCount > 0)
                        {
                            next.Add(openCount - 1);
                        }
                        break;
                }
            }

            reachable = next;

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
        var starIndices = new RepoIndexStack();

        for (var i = 0; i < _s.Length; i++)
        {
            switch (_s[i])
            {
                case '(':
                    openIndices.Push(i);
                    break;
                case '*':
                    starIndices.Push(i);
                    break;
                default:
                    if (!openIndices.TryPop(out _) && !starIndices.TryPop(out _))
                    {
                        return false;
                    }
                    break;
            }
        }

        while (openIndices.TryPop(out var openIndex))
        {
            if (!starIndices.TryPop(out var starIndex) || starIndex < openIndex)
            {
                return false;
            }
        }

        return true;
    }

    private static string BuildInput(int length) => "(" + new string('*', length - 2) + ")";
}
