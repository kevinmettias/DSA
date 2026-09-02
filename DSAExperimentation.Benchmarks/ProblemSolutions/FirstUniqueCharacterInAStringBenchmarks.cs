using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FirstUniqueCharacterInAString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FirstUniqueCharacterInAStringSolution's, the same
// methods FirstUniqueCharacterInAStringTests proves correct. Length is kept well
// past the 26-letter alphabet so no character ever occurs exactly once, forcing
// both strategies through their full worst-case scan - the same "force the real
// worst case" convention TwoSumBenchmarks/LongestSubstringWithoutRepeatingCharactersBenchmarks
// already use.
[MemoryDiagnoser]
public class FirstUniqueCharacterInAStringBenchmarks
{
    private const int AlphabetSize = 26;

    [Params(500, 5_000)]
    public int Length;

    private string _value = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, AlphabetSize));
        }

        _value = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FirstUniqueCharacterInAStringSolution.FirstUniqCharByBruteForce(_value);

    [Benchmark]
    public int HashMapTwoPass() => FirstUniqueCharacterInAStringSolution.FirstUniqCharByHashMapTwoPass(_value);
}
