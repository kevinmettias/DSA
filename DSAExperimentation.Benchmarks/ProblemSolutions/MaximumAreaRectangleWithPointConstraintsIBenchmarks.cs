using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.MaximumAreaRectangleWithPointConstraintsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumAreaRectangleWithPointConstraintsISolution's,
// the same methods MaximumAreaRectangleWithPointConstraintsITests proves correct.
// The corner-lookup arm is handed its prepared Set<(int,int)> from [GlobalSetup]
// so building it is never part of the measured search - the same reason
// OpenTheLockBenchmarks hands ReduceGraphBfs a pre-built LockGraph. Points are a
// dense grid so a large fraction of candidate diagonals actually have both
// opposite corners present, rather than the corner-lookup arm's Set.Has calls
// missing on every candidate.
[MemoryDiagnoser]
public class MaximumAreaRectangleWithPointConstraintsIBenchmarks
{
    private int[][] _points = [];

    private Set<(int X, int Y)> _corners = new();
    // Part I's own constraints keep n tiny - both arms are polynomial-in-n but the
    // quadruple scan is O(n^5), so sizes stay small enough for it to finish.
    [Params(4, 8)]
    public int GridSide { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _points = new int[GridSide * GridSide][];
        var index = 0;

        for (var x = 0; x < GridSide; x++)
        {
            for (var y = 0; y < GridSide; y++)
            {
                _points[index++] = [x, y];
            }
        }

        _corners = new Set<(int X, int Y)>(_points.Select(p => (p[0], p[1])));
    }

    [Benchmark(Baseline = true)]
    public long QuadrupleScan() => MaximumAreaRectangleWithPointConstraintsISolution.MaxAreaByQuadrupleScan(_points);

    [Benchmark]
    public long CornerLookup() =>
        MaximumAreaRectangleWithPointConstraintsISolution.MaxAreaByCornerLookup(_points, _corners);
}
