using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckIfAParenthesesStringCanBeValid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfAParenthesesStringCanBeValidSolution's, the
// same methods CheckIfAParenthesesStringCanBeValidTests proves correct. The workload
// is a leading locked '(' plus an all-free middle plus a trailing locked ')', which
// maximizes the DP's reachable-set growth at every position while the stack sweep
// never even inspects its free-index stack's contents.
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
    public bool ReachableOpenCountDp() =>
        CheckIfAParenthesesStringCanBeValidSolution.CanBeValidByReachableOpenCountDp(_s, _locked);

    [Benchmark]
    public bool IndexStackSweep() =>
        CheckIfAParenthesesStringCanBeValidSolution.CanBeValidByIndexStackSweep(_s, _locked);
}
