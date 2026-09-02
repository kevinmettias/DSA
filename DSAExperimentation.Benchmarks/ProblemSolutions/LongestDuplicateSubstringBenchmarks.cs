using BenchmarkDotNet.Attributes;
using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Duplicate Substring (LC 1044): the brute-force baseline directly compares
// every pair of suffixes (O(n^2) pairs, each an O(match length) character scan) to
// find the longest shared prefix between any two of them, against this repo's own
// SuffixArray, whose Kasai's-algorithm LongestCommonPrefixArray already reports the
// longest shared prefix between every pair of ADJACENT sorted suffixes in
// O(n log^2 n) total - sufficient to find the true global maximum, since any two
// non-adjacent suffixes share at most the minimum longest-common-prefix value
// along the sorted run between them (LongestDuplicateSubstringTests.cs's own
// header explains this property in full).
[MemoryDiagnoser]
public class LongestDuplicateSubstringBenchmarks
{
    private const int AlphabetSize = 4;
    private const int RandomSeed = 1044; // LC problem number

    [Params(200, 2_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int AllSuffixPairsBruteForce()
    {
        var best = 0;

        for (var i = 0; i < _text.Length; i++)
        {
            for (var j = i + 1; j < _text.Length; j++)
            {
                var commonPrefixLength = CommonPrefixLength(i, j);
                best = Math.Max(best, commonPrefixLength);
            }
        }

        return best;
    }

    private int CommonPrefixLength(int first, int second)
    {
        var length = 0;

        while (first + length < _text.Length && second + length < _text.Length
            && _text[first + length] == _text[second + length])
        {
            length++;
        }

        return length;
    }

    [Benchmark]
    public int SuffixArrayLongestCommonPrefix()
    {
        var suffixArray = new SuffixArrayStructure(_text);

        var best = 0;
        foreach (var longestCommonPrefix in suffixArray.LongestCommonPrefixArray)
        {
            best = Math.Max(best, longestCommonPrefix);
        }

        return best;
    }
}
