using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Chunked Palindrome Decomposition (LC 1147): the classic greedy solution
// written the textbook way - growing `left`/`right` via repeated string
// concatenation and comparing them with == - vs. the same greedy walk using this
// repo's own RollingHash for an O(1) chunk-equality screen per growth step instead
// of an O(len) string build+compare (LongestChunkedPalindromeDecompositionTests'
// exact composition). _text has every character distinct (Unicode code points
// starting at 1000, so no accidental match ever fires), the same "force the real
// worst case" intent TwoSumBenchmarks' own setup comment names - both strategies
// are forced to grow their pending window all the way to the middle, which is
// exactly where NaiveConcatenation's repeated O(len) string work costs the most.
[MemoryDiagnoser]
public class LongestChunkedPalindromeDecompositionBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)(1000 + i);
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int NaiveConcatenation()
    {
        var i = 0;
        var j = _text.Length - 1;
        var left = string.Empty;
        var right = string.Empty;
        var count = 0;

        while (i < j)
        {
            left += _text[i];
            right = _text[j] + right;

            if (left == right)
            {
                count += 2;
                left = string.Empty;
                right = string.Empty;
            }

            i++;
            j--;
        }

        if (left.Length > 0 || i == j)
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int RollingHashChunking()
    {
        var hash = new RollingHash(_text);
        var matchStart = 0;
        var i = 0;
        var j = _text.Length - 1;
        var count = 0;

        while (i < j)
        {
            var len = i - matchStart + 1;

            if (hash.Hash(matchStart, len) == hash.Hash(j, len)
                && _text.AsSpan(matchStart, len).SequenceEqual(_text.AsSpan(j, len)))
            {
                count += 2;
                matchStart = i + 1;
            }

            i++;
            j--;
        }

        if (matchStart <= j)
        {
            count++;
        }

        return count;
    }
}
