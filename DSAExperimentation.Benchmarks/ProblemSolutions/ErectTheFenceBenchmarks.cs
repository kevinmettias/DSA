using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ErectTheFence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ErectTheFenceSolution's, the same methods
// ErectTheFenceTests proves correct. Points are uniform-random in a bounded grid,
// so the hull stays a small fraction of Length (h << n), which is exactly what
// makes the O(n log n + h*n) primitive-based strategy beat the O(n^3) baseline so
// decisively.
[MemoryDiagnoser]
public class ErectTheFenceBenchmarks
{
    private const int RandomSeed = 587; // LC problem number
    private const int CoordinateBound = 1_000;

    [Params(50, 300)]
    public int Length;

    private (int X, int Y)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, Length)
            .Select(_ => (random.Next(0, CoordinateBound), random.Next(0, CoordinateBound)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public List<(int X, int Y)> EveryPairHalfPlaneScan() =>
        ErectTheFenceSolution.OuterTreesByBruteForceHalfPlaneScan(_points);

    [Benchmark]
    public List<(int X, int Y)> MonotoneChainThenEdgeScan() =>
        ErectTheFenceSolution.OuterTreesByMonotoneChain(_points);
}
