using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// String Matching in an Array (LC 1408): every word is a run of 'a's with a
// trailing 'b' (a classic KMP-adversarial shape - most candidate start
// positions match every character but the last), lengths strictly increasing
// so shorter words are genuine substrings of longer ones. Measured result is
// the interesting part: NaiveNestedLoop's raw char[]-index double loop wins
// by 15-60x over KmpSubstringSearch here, even though PrefixFunctionSearch's
// failure-function walk really is the asymptotically better O(text+pattern)
// per pair against naive's O(text*pattern) worst case. At LC 1408's actual
// scale (words[i].length <= 30, words.length <= 100) that per-pair fixed cost
// dominates: FindAll recomputes ComputeFailureFunction from scratch on every
// (i, j) pair - even though the same pattern gets re-searched against every
// other word - allocates a fresh failure array and match list every call
// (visible in Allocated below; NaiveNestedLoop allocates nothing), and routes
// every character through IEqualityComparer<char> instead of a raw '=='. The
// primitive's real advantage only shows up once text/pattern are long enough
// to amortize that per-call overhead - well past this problem's own bounds.
[MemoryDiagnoser]
public class StringMatchingInAnArrayBenchmarks
{
    private const int MinLength = 6;

    [Params(60, 150)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
        => _words = Enumerable.Range(0, WordCount).Select(AdversarialWord).ToArray();

    private static string AdversarialWord(int index)
        => new string('a', MinLength + index - 1) + "b";

    [Benchmark(Baseline = true)]
    public int NaiveNestedLoop()
    {
        var count = 0;

        for (var i = 0; i < _words.Length; i++)
        {
            for (var j = 0; j < _words.Length; j++)
            {
                if (j != i && ContainsNaive(_words[j], _words[i]))
                {
                    count++;
                    break;
                }
            }
        }

        return count;
    }

    private static bool ContainsNaive(string text, string pattern)
    {
        for (var start = 0; start + pattern.Length <= text.Length; start++)
        {
            var matched = true;

            for (var offset = 0; offset < pattern.Length; offset++)
            {
                if (text[start + offset] != pattern[offset])
                {
                    matched = false;
                    break;
                }
            }

            if (matched)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public int KmpSubstringSearch()
    {
        var count = 0;

        for (var i = 0; i < _words.Length; i++)
        {
            for (var j = 0; j < _words.Length; j++)
            {
                if (j != i && PrefixFunctionSearch.FindAll(_words[j], _words[i]).Count > 0)
                {
                    count++;
                    break;
                }
            }
        }

        return count;
    }
}
