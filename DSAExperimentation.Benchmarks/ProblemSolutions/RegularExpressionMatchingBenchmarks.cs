using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RegularExpressionMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RegularExpressionMatchingSolution's, the same
// methods RegularExpressionMatchingTests proves correct. A repeated "a*" pattern
// against a text with a mismatched trailing character forces both strategies to
// explore the whole branching search space rather than short-circuiting early.
[MemoryDiagnoser]
public class RegularExpressionMatchingBenchmarks
{
    private const string RepeatedPatternUnit = "a*";
    private const string TrailingChar = "b";

    private string _text = "";
    private string _pattern = "";

    [Params(8, 14)]
    public int Repetitions { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _text = new string('a', Repetitions) + TrailingChar;
        var repeatedUnits = Enumerable.Repeat(RepeatedPatternUnit, Repetitions);
        _pattern = string.Concat(repeatedUnits) + TrailingChar;
    }

    [Benchmark(Baseline = true)]
    public bool IsMatchByRecursion() =>
        RegularExpressionMatchingSolution.IsMatchByRecursion(
            new RegularExpressionMatchingSolution.SubjectText(_text),
            new RegularExpressionMatchingSolution.RegexPattern(_pattern));

    [Benchmark]
    public bool IsMatchByMemoization() =>
        RegularExpressionMatchingSolution.IsMatchByMemoization(
            new RegularExpressionMatchingSolution.SubjectText(_text),
            new RegularExpressionMatchingSolution.RegexPattern(_pattern));
}
