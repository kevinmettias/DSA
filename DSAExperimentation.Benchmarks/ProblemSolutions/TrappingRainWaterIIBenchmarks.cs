using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TrappingRainWaterII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TrappingRainWaterIISolution's, the same methods
// TrappingRainWaterIISolutionTests proves correct.
public class TrappingRainWaterIIBenchmarks
{
    private const int RandomSeed = 407; // LC 407: Trapping Rain Water II
    private const int MaxHeightMapValue = 50;

    private int[][] _heightMap = [];

    [Params(15, 40)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heightMap = Enumerable.Range(0, Size)
            .Select(_ => SeededDraws.Values(Size, 0, MaxHeightMapValue, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RelaxationSweep() => TrappingRainWaterIISolution.TrapRainWaterByRelaxationSweep(_heightMap);

    [Benchmark]
    public int HeapFloodFill() => TrappingRainWaterIISolution.TrapRainWaterByHeapFloodFill(_heightMap);
}
