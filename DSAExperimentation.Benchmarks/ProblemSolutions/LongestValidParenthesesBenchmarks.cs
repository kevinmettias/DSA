using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestValidParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestValidParenthesesSolution's, the same
// methods LongestValidParenthesesTests proves correct.
[MemoryDiagnoser]
public class LongestValidParenthesesBenchmarks
{
    private const string RepeatingPattern = "(()())";

    private string _value = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var repeatedPattern = Enumerable.Repeat(RepeatingPattern, (Length / RepeatingPattern.Length) + 1);
        var repeated = string.Concat(repeatedPattern);
        _value = repeated[..Length];
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgrammingArray() => LongestValidParenthesesSolution.LengthByDynamicProgrammingArray(_value);

    [Benchmark]
    public int StackScan() => LongestValidParenthesesSolution.LengthByStackScan(_value);
}
