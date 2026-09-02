using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Form Array by Concatenating Subarrays of Another Array (LC 1764): does a
// pattern occur as a contiguous subarray of nums, an O(n*m) naive
// re-comparison-from-scratch-at-every-start baseline vs. compressing both int
// arrays into a shared char alphabet (this repo's own HashMap<int,char>) and
// searching with PrefixFunctionSearch (KMP), O(n+m) guaranteed
// (FormArrayByConcatenatingSubarraysOfAnotherArrayTests precedent). nums is
// almost entirely zeros with a single distinguishing 1 at the very end, and
// pattern is the same shape at half the length - the classic KMP-worst-case
// adversarial input, since the naive scan re-walks almost the whole pattern
// at nearly every start position before failing on its last element.
[MemoryDiagnoser]
public class FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks
{
    private const int PatternLengthDivisor = 2; // pattern is built at half the length of nums

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[] _pattern = null!;

    [GlobalSetup]
    public void Setup()
    {
        _nums = BuildAlmostAllZeros(Length);
        _pattern = BuildAlmostAllZeros(Length / PatternLengthDivisor);
    }

    private static int[] BuildAlmostAllZeros(int length)
    {
        var values = new int[length];
        values[^1] = 1;
        return values;
    }

    [Benchmark(Baseline = true)]
    public bool NaiveSubarrayScan()
    {
        for (var start = 0; start + _pattern.Length <= _nums.Length; start++)
        {
            if (MatchesAt(start))
            {
                return true;
            }
        }

        return false;
    }

    private bool MatchesAt(int start)
    {
        for (var i = 0; i < _pattern.Length; i++)
        {
            if (_nums[start + i] != _pattern[i])
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool CharCompressedKmpSearch()
    {
        var codes = new HashMap<int, char>();
        AssignCodes(_nums, codes);
        AssignCodes(_pattern, codes);

        var text = Encode(_nums, codes);
        var pattern = Encode(_pattern, codes);
        return PrefixFunctionSearch.FindAll(text, pattern).Count > 0;
    }

    private static void AssignCodes(int[] values, HashMap<int, char> codes)
    {
        foreach (var value in values)
        {
            if (!codes.HasKey(value))
            {
                codes.Set(value, (char)codes.Count);
            }
        }
    }

    private static char[] Encode(int[] values, HashMap<int, char> codes)
    {
        var encoded = new char[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            codes.TryGetValue(values[i], out encoded[i]);
        }

        return encoded;
    }
}
