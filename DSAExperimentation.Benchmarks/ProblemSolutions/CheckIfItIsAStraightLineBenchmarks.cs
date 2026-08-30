using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check If It Is a Straight Line (LC 1232): the O(n^3) "check every triple of
// points for collinearity" brute force vs. the optimal O(n) single-pass
// cross-product check anchored on the first two points - no repo primitive applies,
// the same "bare array, plain arithmetic" shape as GasStationBenchmarks/
// JumpGameBenchmarks. _coordinates is always collinear so BOTH strategies are
// forced through their full worst-case scan instead of an early-exit on the first
// bad triple/point making brute force look artificially competitive.
[MemoryDiagnoser]
public class CheckIfItIsAStraightLineBenchmarks
{
    [Params(20, 100)]
    public int Length;

    private int[][] _coordinates = null!;

    [GlobalSetup]
    public void Setup()
    {
        _coordinates = Enumerable.Range(0, Length).Select(i => new[] { i, i }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceEveryTriple()
    {
        for (var i = 0; i < _coordinates.Length; i++)
        {
            for (var j = i + 1; j < _coordinates.Length; j++)
            {
                for (var k = j + 1; k < _coordinates.Length; k++)
                {
                    var dx1 = (long)(_coordinates[j][0] - _coordinates[i][0]);
                    var dy1 = (long)(_coordinates[j][1] - _coordinates[i][1]);
                    var dx2 = (long)(_coordinates[k][0] - _coordinates[i][0]);
                    var dy2 = (long)(_coordinates[k][1] - _coordinates[i][1]);

                    if (dx1 * dy2 != dy1 * dx2)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    [Benchmark]
    public bool AnchoredCrossProductScan()
    {
        var x0 = _coordinates[0][0];
        var y0 = _coordinates[0][1];
        var dx = (long)(_coordinates[1][0] - x0);
        var dy = (long)(_coordinates[1][1] - y0);

        for (var i = 2; i < _coordinates.Length; i++)
        {
            var px = (long)(_coordinates[i][0] - x0);
            var py = (long)(_coordinates[i][1] - y0);

            if (dx * py != dy * px)
            {
                return false;
            }
        }

        return true;
    }
}
