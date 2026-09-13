using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReverseSubstringsBetweenEachPairOfParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// ReverseSubstringsBetweenEachPairOfParenthesesSolution's, the same methods
// ReverseSubstringsBetweenEachPairOfParenthesesTests proves correct. The input is
// LeetCode's own shape - a run of GroupCount sibling (non-nested) groups, so the
// quadratic baseline's cost comes from re-splicing the string once per pair rather
// than from nesting depth - and building it is charged to [GlobalSetup].
[MemoryDiagnoser]
public class ReverseSubstringsBetweenEachPairOfParenthesesBenchmarks
{
    private const string GroupBody = "abcdef";

    [Params(500, 5_000)]
    public int GroupCount;

    private string _input = null!;

    [GlobalSetup]
    public void Setup()
    {
        var builder = new StringBuilder();

        for (var i = 0; i < GroupCount; i++)
        {
            builder.Append('(').Append(GroupBody).Append(')');
        }

        _input = builder.ToString();
    }

    [Benchmark(Baseline = true)]
    public string NaiveRepeatedSplice() =>
        ReverseSubstringsBetweenEachPairOfParenthesesSolution.ReverseParenthesesByRepeatedSplice(_input);

    [Benchmark]
    public string StackOfCharBuffers() =>
        ReverseSubstringsBetweenEachPairOfParenthesesSolution.ReverseParenthesesByCharBufferStack(_input);
}
