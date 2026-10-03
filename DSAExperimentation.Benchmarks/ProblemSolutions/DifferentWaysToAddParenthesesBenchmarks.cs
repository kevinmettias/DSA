using DSAExperimentation.LeetCode.DifferentWaysToAddParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DifferentWaysToAddParenthesesSolution's, the same
// methods DifferentWaysToAddParenthesesSolutionTests proves correct. The expression
// repeats the same operand ("1+1+...+1"), so the same substrings ("1", "1+1",
// ...) recur across many different split points - recomputed from scratch every
// time by the plain recursion, resolved once and reused by Memoizer.
public class DifferentWaysToAddParenthesesBenchmarks
{
    private const string Operand = "1";

    private string _expression = "";

    [Params(6, 10)]
    public int OperandCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var operands = Enumerable.Repeat(Operand, OperandCount);
        _expression = string.Join('+', operands);
    }

    [Benchmark(Baseline = true)]
    public List<int> PlainRecursion() =>
        DifferentWaysToAddParenthesesSolution.DiffWaysToComputeByPlainRecursion(_expression);

    [Benchmark]
    public List<int> MemoizedSubstring() =>
        DifferentWaysToAddParenthesesSolution.DiffWaysToComputeByMemoizedSubstring(_expression);
}
