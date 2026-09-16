using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidParenthesisString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidParenthesisStringSolution's, the same methods
// ValidParenthesisStringTests proves correct. _text is "(" + all '*' + ")", the
// shape that maximizes the DP's reachable-set growth every step while the stack
// sweep never even inspects its star stack's contents.
[MemoryDiagnoser]
public class ValidParenthesisStringBenchmarks
{
    private const string OpenParenthesis = "(";
    private const string CloseParenthesis = ")";
    private const int BoundaryParenthesisCount = 2; private string _text = "";

    // one leading '(' + one trailing ')'

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _text = BuildInput(Length);

    private static string BuildInput(int length) => OpenParenthesis + new string('*', length - BoundaryParenthesisCount) + CloseParenthesis;

    [Benchmark(Baseline = true)]
    public bool IsValidStringByReachableOpenCountDp() =>
        ValidParenthesisStringSolution.IsValidStringByReachableOpenCountDp(_text);

    [Benchmark]
    public bool IsValidStringByTwoIndexStackSweep() =>
        ValidParenthesisStringSolution.IsValidStringByTwoIndexStackSweep(_text);
}
