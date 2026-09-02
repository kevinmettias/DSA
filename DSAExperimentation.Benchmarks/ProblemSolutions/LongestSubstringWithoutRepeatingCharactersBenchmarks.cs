using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestSubstringWithoutRepeatingCharacters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// LongestSubstringWithoutRepeatingCharactersSolution's, the same methods
// LongestSubstringWithoutRepeatingCharactersTests proves correct. _text is
// deliberately built from all-distinct characters (no repeat anywhere) so BOTH
// strategies are forced through their full worst-case scan - a small,
// repeat-heavy alphabet would let BruteForce's inner loop break out after only a
// handful of characters every time (pigeonhole caps any repeat-free run at the
// alphabet size), making it look artificially competitive instead of exposing
// its real O(n^2) cost.
[MemoryDiagnoser]
public class LongestSubstringWithoutRepeatingCharactersBenchmarks
{
    private const int DistinctCharacterBase = 256;

    [Params(200, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        _text = new string(Enumerable.Range(0, Length).Select(i => (char)(DistinctCharacterBase + i)).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => LongestSubstringWithoutRepeatingCharactersSolution.FindLengthByBruteForce(_text);

    [Benchmark]
    public int SlidingWindowHashMap() =>
        LongestSubstringWithoutRepeatingCharactersSolution.FindLengthBySlidingWindowHashMap(_text);
}
