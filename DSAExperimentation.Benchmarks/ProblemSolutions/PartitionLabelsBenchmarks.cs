using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PartitionLabels;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionLabelsSolution's, the same methods
// PartitionLabelsTests proves correct. Letters are random across the whole
// alphabet so partitions stay small and the brute-force rescan is forced to
// search most of the string, over and over, instead of an early exit.
[MemoryDiagnoser]
public class PartitionLabelsBenchmarks
{
    // LC problem number, reused as the deterministic PRNG seed.
    private const int RandomSeed = 763;
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public List<int> BruteForceRescan() => PartitionLabelsSolution.PartitionLabelSizesByBruteForceRescan(_text);

    [Benchmark]
    public List<int> HashMapOnePass() => PartitionLabelsSolution.PartitionLabelSizesByHashMapOnePass(_text);
}
