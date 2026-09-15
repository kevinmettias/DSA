using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StringMatchingInAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StringMatchingInAnArraySolution's, the same methods
// StringMatchingInAnArrayTests proves correct. Every word is a run of 'a's with a
// trailing 'b' (a classic KMP-adversarial shape - most candidate start positions
// match every character but the last), lengths strictly increasing so shorter words
// are genuine substrings of longer ones.
//
// Measured result is the interesting part: NaiveScan's raw char-index double loop
// wins by a wide margin over PrefixFunctionSearch here, even though the
// failure-function walk really is the asymptotically better O(text + pattern) per
// pair against naive's O(text * pattern) worst case. At LC 1408's actual scale
// (words[i].length <= 30, words.length <= 100) that per-pair fixed cost dominates:
// FindAll recomputes ComputeFailureFunction from scratch on every (i, j) pair - even
// though the same pattern gets re-searched against every other word - allocates a
// fresh failure array and match list every call (visible in Allocated below), and
// routes every character through IEqualityComparer<char> instead of a raw '=='. The
// primitive's real advantage only shows up once text/pattern are long enough to
// amortize that per-call overhead - well past this problem's own bounds.
//
// Both arms now build LC 1408's actual answer - the list of contained words - rather
// than only counting them, and the harness takes .Count; the same deliberate change
// §17.8 records for WordLadderII. The extra work is one List<string> add per
// contained word, identical in both arms.
[MemoryDiagnoser]
public class StringMatchingInAnArrayBenchmarks
{
    private const int MinLength = 6;
    private const string TrailingLetter = "b";

    private string[] _words = [];

    [Params(60, 150)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
        => _words = Enumerable.Range(0, WordCount).Select(AdversarialWord).ToArray();

    private static string AdversarialWord(int index)
        => new string('a', MinLength + index - 1) + TrailingLetter;

    [Benchmark(Baseline = true)]
    public int NaiveNestedLoop() =>
        StringMatchingInAnArraySolution.FindContainedWordsByNaiveScan(_words).Count;

    [Benchmark]
    public int KmpSubstringSearch() =>
        StringMatchingInAnArraySolution.FindContainedWordsByPrefixFunctionSearch(_words).Count;
}
