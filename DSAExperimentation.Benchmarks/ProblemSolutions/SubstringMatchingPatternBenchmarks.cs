using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubstringMatchingPattern;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubstringMatchingPatternSolution's, the same methods
// SubstringMatchingPatternTests proves correct. subject is a run of 'a's and both halves
// of pattern are a long run of 'a's followed by a 'b' - a character subject never contains - so
// every candidate window matches almost the whole prefix/suffix before failing on
// the last character, forcing both strategies through their full worst case instead
// of an early first-character mismatch making brute force look artificially
// competitive.
[MemoryDiagnoser]
public class SubstringMatchingPatternBenchmarks
{
    private string _subject = "";

    private string _pattern = "";
    [Params(50, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var half = new string('a', Length / 4) + 'b';
        _subject = new string('a', Length);
        _pattern = $"{half}*{half}";
    }

    [Benchmark(Baseline = true)]
    public bool HasMatchByBruteForce() =>
        SubstringMatchingPatternSolution.HasMatchByBruteForce(new SubjectText(_subject), new WildcardPattern(_pattern));

    [Benchmark]
    public bool HasMatchByPrefixFunctionSearch() =>
        SubstringMatchingPatternSolution.HasMatchByPrefixFunctionSearch(new SubjectText(_subject), new WildcardPattern(_pattern));
}
