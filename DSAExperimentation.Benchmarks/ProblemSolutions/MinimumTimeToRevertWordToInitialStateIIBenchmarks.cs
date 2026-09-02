using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumTimeToRevertWordToInitialStateII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumTimeToRevertWordToInitialStateIISolution's, the same methods
// MinimumTimeToRevertWordToInitialStateIITests proves correct. WordLength is
// scaled up from 3029's own benchmark (10/50) to actually exercise the gap
// the O(n) ZFunction strategy exists to close, while staying well short of
// this problem's own 10^6 bound so BruteForce's O(n^2/k) arm still finishes
// in a reasonable benchmark run. k is fixed at 1, the worst case for
// BruteForce since it forces the full n candidate shifts. The word is drawn
// from a 2-letter alphabet rather than the full 26, so most candidate shifts
// partially match before diverging - a harder workload for BruteForce's
// character-by-character comparison than a full-alphabet word, which tends
// to mismatch at the very first character.
[MemoryDiagnoser]
public class MinimumTimeToRevertWordToInitialStateIIBenchmarks
{
    private const int RandomSeed = 3031;
    private const int K = 1;
    private const int AlphabetSize = 2;

    [Params(500, 2000)]
    public int WordLength;

    private string _word = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _word = new string(
            Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MinimumTimeToRevertWordToInitialStateIISolution.MinTimeByBruteForce(_word, K);

    [Benchmark]
    public int ZFunction() => MinimumTimeToRevertWordToInitialStateIISolution.MinTimeByZFunction(_word, K);
}
