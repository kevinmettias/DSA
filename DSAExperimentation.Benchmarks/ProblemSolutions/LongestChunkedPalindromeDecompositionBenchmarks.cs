using DSAExperimentation.LeetCode.LongestChunkedPalindromeDecomposition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestChunkedPalindromeDecompositionSolution's, the
// same strategies LongestChunkedPalindromeDecompositionSolutionTests proves correct. Setup
// builds a lowercase text whose only 'a' is its first character, with 'b'..'z' cycling
// after it: every prefix starts with 'a' and every suffix of the same length does not,
// so no chunk ever closes early - the same "force the real worst case" intent
// TwoSumBenchmarks' own setup comment names. Both strategies are forced to grow their
// pending window all the way to the middle, which is exactly where the
// string-concatenation arm's repeated O(len) build+compare costs the most. Length stops
// at LC 1147's 1,000-character cap.
public class LongestChunkedPalindromeDecompositionBenchmarks
{
    private const char UniqueFirstLetter = 'a';
    private const char FirstCycledLetter = 'b';
    private const int CycledLetterCount = 25;

    private string _text = "";

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[Length];
        chars[0] = UniqueFirstLetter;

        for (var i = 1; i < Length; i++)
        {
            chars[i] = (char)(FirstCycledLetter + ((i - 1) % CycledLetterCount));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int StringConcatenation() =>
        LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByStringConcatenation(_text);

    [Benchmark]
    public int RollingHashChunking() =>
        LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByRollingHash(_text);
}
