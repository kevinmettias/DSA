using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumMovesToSpreadStonesOverGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumMovesToSpreadStonesOverGridSolution's, the
// same methods MinimumMovesToSpreadStonesOverGridTests proves correct. The grid is
// fixed-size 3x3 by the problem itself, so there is no "Length" axis to grow -
// ExcessConcentration instead varies how lopsided the distribution is (all 6
// spare stones piled on one cell vs. spread across three), which is what drives
// how many excess stones (and therefore how many permutation leaves) each case
// actually walks.
[MemoryDiagnoser]
public class MinimumMovesToSpreadStonesOverGridBenchmarks
{
    [Params(1, 3)]
    public int PileCount;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        // 9 stones total, always. PileCount=1 concentrates all 6 spare stones on
        // one cell (6 excess stones to permute); PileCount=3 spreads the same 6
        // spare stones across three cells (still 6 excess stones, just grouped
        // differently) so both cases exercise the same 6!-sized search space with
        // a different distribution shape.
        _grid = PileCount == 1
            ? [[7, 0, 0], [0, 1, 0], [0, 0, 1]]
            : [[3, 0, 0], [0, 3, 0], [0, 0, 3]];
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePermutation() => MinimumMovesToSpreadStonesOverGridSolution.MinimumMovesByBruteForcePermutation(_grid);

    [Benchmark]
    public int BacktrackPermutation() => MinimumMovesToSpreadStonesOverGridSolution.MinimumMovesByBacktrackPermutation(_grid);
}
