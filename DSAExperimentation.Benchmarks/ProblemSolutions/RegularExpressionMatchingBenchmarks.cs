using DSAExperimentation.LeetCode.RegularExpressionMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RegularExpressionMatchingSolution's, the same
// methods RegularExpressionMatchingSolutionTests proves correct. A repeated "a*" pattern
// ends in a literal the text never contains, so no way of dividing the text's a's among
// the "a*" units can match: the plain recursion has to try every one of those divisions
// before it can answer false, which the memoized recurrence collapses to one table.
// Repetitions stops at 9: the pattern runs to 2 * 9 + 1 = 19 characters, the longest
// inside LC 10's 20.
public class RegularExpressionMatchingBenchmarks
{
    private const string RepeatedPatternUnit = "a*";
    private const string TextTrailingChar = "b";
    private const string PatternTrailingChar = "c";

    private string _text = "";
    private string _pattern = "";

    [Params(8, 9)]
    public int Repetitions { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _text = new string('a', Repetitions) + TextTrailingChar;
        var repeatedUnits = Enumerable.Repeat(RepeatedPatternUnit, Repetitions);
        _pattern = string.Concat(repeatedUnits) + PatternTrailingChar;
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
