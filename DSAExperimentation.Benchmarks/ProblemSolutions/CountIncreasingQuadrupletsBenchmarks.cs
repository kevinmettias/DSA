using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountIncreasingQuadruplets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountIncreasingQuadrupletsSolution's, the same
// methods CountIncreasingQuadrupletsTests proves correct. Params stay small on
// purpose - the O(n^4) baseline would otherwise dominate the run, so both arms
// are measured on the same modest permutation sizes rather than letting the
// baseline set an unreasonably tiny Length just for itself.
[MemoryDiagnoser]
public class CountIncreasingQuadrupletsBenchmarks
{
    private const int RandomSeed = 2552; // LeetCode problem number

    [Params(8, 16)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountIncreasingQuadrupletsSolution.CountQuadrupletsByBruteForce(_nums);

    [Benchmark]
    public long FenwickTreeSweep() => CountIncreasingQuadrupletsSolution.CountQuadrupletsByFenwickTreeSweep(_nums);
}
