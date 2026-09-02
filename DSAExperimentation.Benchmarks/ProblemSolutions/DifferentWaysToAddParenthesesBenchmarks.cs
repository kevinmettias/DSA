using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DifferentWaysToAddParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DifferentWaysToAddParenthesesSolution's, the same
// methods DifferentWaysToAddParenthesesTests proves correct. The expression
// repeats the same operand ("1+1+...+1"), so the same substrings ("1", "1+1",
// ...) recur across many different split points - recomputed from scratch every
// time by the plain recursion, resolved once and reused by Memoizer.
[MemoryDiagnoser]
public class DifferentWaysToAddParenthesesBenchmarks
{
    private const string Operand = "1";

    [Params(6, 10)]
    public int OperandCount;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var operands = Enumerable.Repeat(Operand, OperandCount);
        _expression = string.Join('+', operands);
    }

    [Benchmark(Baseline = true)]
    public int PlainRecursion() =>
        DifferentWaysToAddParenthesesSolution.DiffWaysToComputeByPlainRecursion(_expression).Count;

    [Benchmark]
    public int MemoizedSubstring() =>
        DifferentWaysToAddParenthesesSolution.DiffWaysToComputeByMemoizedSubstring(_expression).Count;
}
