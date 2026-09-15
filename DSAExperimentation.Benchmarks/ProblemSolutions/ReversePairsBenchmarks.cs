using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReversePairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReversePairsSolution's, the same methods
// ReversePairsTests proves correct.
[MemoryDiagnoser]
public class ReversePairsBenchmarks
{
    private const int RandomSeed = 493; // LC problem number
    private const int ValueBound = 10_000;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan() => ReversePairsSolution.CountByPairwiseScan(_nums);

    [Benchmark]
    public int FenwickTreeSweep() => ReversePairsSolution.CountByFenwickTreeSweep(_nums);
}
