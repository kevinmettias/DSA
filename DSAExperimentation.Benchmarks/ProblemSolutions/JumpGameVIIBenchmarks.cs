using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game VII (LC 1871): plain unmemoized recursive search - re-exploring the same
// index from every jump chain that reconverges on it, since minJump=1/maxJump=2
// lets many different step sequences land on the same index - vs. this repo's own
// DepthFirstSearch.Traverse, whose HashSet-backed visited tracking (the same
// mechanism EscapeALargeMazeBenchmarks already benchmarks for an implicit 2-D grid)
// visits each index at most once. The generated string is all '0' except a blocked
// final character, so neither strategy can short-circuit on an early success -
// both must exhaust every reachable index before returning false, which is exactly
// what makes the unmemoized baseline's Fibonacci-shaped call-count blowup real
// (StringLength=28 already reaches ~500K calls, 32 ~3.5M) while Traverse stays
// linear in the string length. StringLength is kept modest for the same reason
// StoneGameVIIBenchmarks keeps PileCount modest for its own exponential baseline.
[MemoryDiagnoser]
public class JumpGameVIIBenchmarks
{
    private const int MinJump = 1;
    private const int MaxJump = 2;

    [Params(28, 32)]
    public int StringLength;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[StringLength];
        Array.Fill(chars, '0');
        chars[StringLength - 1] = '1';
        _s = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursiveSearch() => CanReachFrom(0);

    private bool CanReachFrom(int i)
    {
        if (i == _s.Length - 1)
        {
            return true;
        }

        var lastIndex = Math.Min(i + MaxJump, _s.Length - 1);

        for (var j = i + MinJump; j <= lastIndex; j++)
        {
            if (_s[j] == '0' && CanReachFrom(j))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool VisitedTrackingTraversal()
    {
        var reached = DepthFirstSearch.Traverse(0, Successors);
        return reached.Contains(_s.Length - 1);
    }

    private IEnumerable<int> Successors(int i)
    {
        var lastIndex = Math.Min(i + MaxJump, _s.Length - 1);
        var next = new List<int>();

        for (var j = i + MinJump; j <= lastIndex; j++)
        {
            if (_s[j] == '0')
            {
                next.Add(j);
            }
        }

        return next;
    }
}
