using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestRepeatingCharacterReplacement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestRepeatingCharacterReplacementSolution's, the
// same methods LongestRepeatingCharacterReplacementTests proves correct. _text is
// a single repeated character so BruteForce's inner loop never breaks early
// (every window is trivially already-repeating), forcing its full O(n^2) worst
// case instead of bottoming out after a handful of characters.
[MemoryDiagnoser]
public class LongestRepeatingCharacterReplacementBenchmarks
{
    private const int K = 2;

    private string _text = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _text = new string('A', Length);

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        LongestRepeatingCharacterReplacementSolution.LongestRunByBruteForce(_text, K);

    [Benchmark]
    public int SlidingWindowHashMap() =>
        LongestRepeatingCharacterReplacementSolution.LongestRunBySlidingWindowHashMap(_text, K);
}
