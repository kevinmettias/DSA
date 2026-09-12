using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TrappingRainWaterII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TrappingRainWaterIISolution's, the same methods
// TrappingRainWaterIITests proves correct.
[MemoryDiagnoser]
public class TrappingRainWaterIIBenchmarks
{
    private const int RandomSeed = 407; // LC 407: Trapping Rain Water II
    private const int MaxHeightMapValue = 50;

    [Params(15, 40)]
    public int Size;

    private int[][] _heightMap = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heightMap = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, MaxHeightMapValue)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RelaxationSweep() => TrappingRainWaterIISolution.TrapRainWaterByRelaxationSweep(_heightMap);

    [Benchmark]
    public int HeapFloodFill() => TrappingRainWaterIISolution.TrapRainWaterByHeapFloodFill(_heightMap);
}
