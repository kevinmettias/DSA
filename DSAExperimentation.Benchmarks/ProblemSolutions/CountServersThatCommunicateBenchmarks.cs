using DSAExperimentation.LeetCode.CountServersThatCommunicate;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountServersThatCommunicateSolution's, the same
// methods CountServersThatCommunicateSolutionTests proves correct - the
// O(rows*cols*(rows+cols)) per-server row/column rescan against the O(rows*cols)
// two-pass HashMap<int,int> tally.
//
// A dry run once showed the rescan ~4.8x faster at Size=100, where HashMap's
// per-call hashing/bucket overhead dominates while rows*cols*(rows+cols) is still
// small. Size stops at LC 1267's 250 x 250 grid, so the rescan's cubic growth shows
// only as far as that bound allows.
public class CountServersThatCommunicateBenchmarks
{
    // A cell becomes a server with 1-in-N odds.
    private const int ServerSpawnDenominator = 4;

    private const int GridSeed = 1;

    private int[][] _grid = [];

    [Params(100, 250)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(GridSeed);
        _grid = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _grid[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                var isServer = random.Next(0, ServerSpawnDenominator) == 0;
                _grid[r][c] = isServer ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRowAndColumnRescan() =>
        CountServersThatCommunicateSolution.CountServersByRowAndColumnRescan(_grid);

    [Benchmark]
    public int HashMapRowAndColumnCounts() =>
        CountServersThatCommunicateSolution.CountServersByRowAndColumnCounts(_grid);
}
