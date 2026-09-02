using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PartitionString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionStringSolution's, the same methods
// PartitionStringTests proves correct. _text is drawn from a small alphabet
// (seeded, deterministic) so segments repeat often and both arms are forced
// through their real "keep extending until unique" worst case, rather than
// every one-character segment being unique on the first try.
[MemoryDiagnoser]
public class PartitionStringBenchmarks
{
    private const int Seed = 3597;
    private const int AlphabetSize = 4;

    [Params(1_000, 20_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public List<string> HashSetScan() => PartitionStringSolution.PartitionByHashSetScan(_text);

    [Benchmark]
    public List<string> SetScan() => PartitionStringSolution.PartitionBySetScan(_text);
}
