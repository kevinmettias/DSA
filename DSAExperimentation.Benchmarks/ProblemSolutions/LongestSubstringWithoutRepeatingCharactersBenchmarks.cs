using DSAExperimentation.LeetCode.LongestSubstringWithoutRepeatingCharacters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// LongestSubstringWithoutRepeatingCharactersSolution's, the same methods
// LongestSubstringWithoutRepeatingCharactersSolutionTests proves correct. _text
// cycles through all 95 printable ASCII characters - LC 3's whole alphabet of
// letters, digits, symbols and spaces - so every window of 95 is repeat-free, the
// longest run any input can hold, and BOTH strategies are pushed through the
// worst-case scan the problem allows. A small, repeat-heavy alphabet would let
// BruteForce's inner loop break out after only a handful of characters every time
// (pigeonhole caps any repeat-free run at the alphabet size), making it look
// artificially competitive. That same cap is why, on any input LC 3 poses,
// BruteForce costs O(95n) rather than O(n^2).
public class LongestSubstringWithoutRepeatingCharactersBenchmarks
{
    private const char FirstPrintableCharacter = ' ';
    private const int PrintableCharacterCount = 95;

    private string _text = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _text = new string(Enumerable.Range(0, Length).Select(PrintableCharacterAt).ToArray());

    private static char PrintableCharacterAt(int index) =>
        (char)(FirstPrintableCharacter + (index % PrintableCharacterCount));

    [Benchmark(Baseline = true)]
    public int BruteForce() => LongestSubstringWithoutRepeatingCharactersSolution.FindLengthByBruteForce(_text);

    [Benchmark]
    public int SlidingWindowHashMap() =>
        LongestSubstringWithoutRepeatingCharactersSolution.FindLengthBySlidingWindowHashMap(_text);
}
