using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.ValidParenthesisString.ValidParenthesisStringSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidParenthesisStringSolution's, the same methods
// ValidParenthesisStringTests proves correct. _s is "(" + all '*' + ")", the
// shape that maximizes the DP's reachable-set growth every step while the stack
// sweep never even inspects its star stack's contents.
[MemoryDiagnoser]
public class ValidParenthesisStringBenchmarks
{
    private const string OpenParenthesis = "(";
    private const string CloseParenthesis = ")";
    private const int BoundaryParenthesisCount = 2; // one leading '(' + one trailing ')'

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup() => _s = BuildInput(Length);

    [Benchmark(Baseline = true)]
    public bool ReachableOpenCountDp() => CheckValidStringByReachableOpenCountDp(_s);

    [Benchmark]
    public bool TwoIndexStackSweep() => CheckValidStringByTwoIndexStackSweep(_s);

    private static string BuildInput(int length) => OpenParenthesis + new string('*', length - BoundaryParenthesisCount) + CloseParenthesis;
}
