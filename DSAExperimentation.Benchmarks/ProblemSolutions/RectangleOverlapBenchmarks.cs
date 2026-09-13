using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RectangleOverlap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RectangleOverlapSolution's, the same methods
// RectangleOverlapTests proves correct. Rectangle 2 is offset by half of rectangle
// 1's side along both axes, so the two always overlap and the shared region scales
// with Side - the unit-grid arm still has to paint the whole bounding box before it
// can answer, while the closed-form arm stays O(1).
[MemoryDiagnoser]
public class RectangleOverlapBenchmarks
{
    private const int OffsetDivisor = 2;

    [Params(60, 400)]
    public int Side;

    private int[] _rec1 = [];
    private int[] _rec2 = [];

    [GlobalSetup]
    public void Setup()
    {
        var offset = Side / OffsetDivisor;

        _rec1 = [0, 0, Side, Side];
        _rec2 = [offset, offset, offset + Side, offset + Side];
    }

    [Benchmark(Baseline = true)]
    public bool UnitGridIntersectionScan() =>
        RectangleOverlapSolution.OverlapsByUnitGridIntersectionScan(_rec1, _rec2);

    [Benchmark]
    public bool ClosedFormAxisOverlap() =>
        RectangleOverlapSolution.OverlapsByClosedFormAxisIntervals(_rec1, _rec2);
}
