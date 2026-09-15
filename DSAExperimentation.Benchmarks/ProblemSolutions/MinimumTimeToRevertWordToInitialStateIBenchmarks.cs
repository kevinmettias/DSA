using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumTimeToRevertWordToInitialStateI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumTimeToRevertWordToInitialStateISolution's, the same methods
// MinimumTimeToRevertWordToInitialStateITests proves correct. WordLength
// matches LC 3029's own bound (word.Length <= 50); k is fixed at 1, the
// worst case for the O(n^2/k) baseline since it forces the full n candidate
// shifts rather than letting a larger k skip most of them. The word is drawn
// from a 2-letter alphabet rather than the full 26, so most candidate shifts
// partially match before diverging - a harder workload for BruteForce's
// character-by-character comparison than a full-alphabet word, which tends
// to mismatch at the very first character.
[MemoryDiagnoser]
public class MinimumTimeToRevertWordToInitialStateIBenchmarks
{
    private const int RandomSeed = 3029;
    private const int K = 1;
    private const int AlphabetSize = 2;

    private string _word = "";

    [Params(10, 50)]
    public int WordLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _word = new string(
            Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MinimumTimeToRevertWordToInitialStateISolution.MinTimeByBruteForce(_word, K);

    [Benchmark]
    public int ZFunction() => MinimumTimeToRevertWordToInitialStateISolution.MinTimeByZFunction(_word, K);
}
