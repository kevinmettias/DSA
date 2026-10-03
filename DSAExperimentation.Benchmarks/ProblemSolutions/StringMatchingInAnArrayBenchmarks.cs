using DSAExperimentation.LeetCode.StringMatchingInAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StringMatchingInAnArraySolution's, the same methods
// StringMatchingInAnArraySolutionTests proves correct. Every word is a run of 'a's with
// one trailing letter (a classic KMP-adversarial shape - most candidate start positions
// match every character but the last). Words run from MinLength to LC 1408's longest,
// 30 letters, in families of FamilySize that share a trailing letter - 'b', then 'c',
// and so on - so within a family each shorter word is a genuine substring of every
// longer one, and WordCount reaches LC 1408's 100 distinct words without a word past
// 30 letters.
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
// Both arms now build and return LC 1408's actual answer - the list of contained
// words - rather than only counting them; the same deliberate change §17.8 records
// for WordLadderII. The extra work is one List<string> add per contained word,
// identical in both arms.
public class StringMatchingInAnArrayBenchmarks
{
    private const int MinLength = 6;
    private const int MaxLength = 30;
    private const int FamilySize = MaxLength - MinLength + 1;
    private const char FirstTrailingLetter = 'b';

    private string[] _words = [];

    [Params(60, 100)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
        => _words = Enumerable.Range(0, WordCount).Select(AdversarialWord).ToArray();

    // Word index's family picks its trailing letter, and its place in the family its length.
    private static string AdversarialWord(int index)
    {
        var length = MinLength + (index % FamilySize);
        var trailingLetter = (char)(FirstTrailingLetter + (index / FamilySize));

        return new string('a', length - 1) + trailingLetter;
    }

    [Benchmark(Baseline = true)]
    public List<string> NaiveNestedLoop() =>
        StringMatchingInAnArraySolution.FindContainedWordsByNaiveScan(_words);

    [Benchmark]
    public List<string> KmpSubstringSearch() =>
        StringMatchingInAnArraySolution.FindContainedWordsByPrefixFunctionSearch(_words);
}
