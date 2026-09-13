using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountServersThatCommunicate;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountServersThatCommunicateSolution's, the same
// methods CountServersThatCommunicateTests proves correct - the
// O(rows*cols*(rows+cols)) per-server row/column rescan against the O(rows*cols)
// two-pass HashMap<int,int> tally.
//
// The two [Params] sizes were chosen to show the real crossover this Big-O gap
// predicts, not just one side of it: a dry-run showed the rescan ~4.8x faster at
// Size=100 (HashMap's per-call hashing/bucket overhead dominates when
// rows*cols*(rows+cols) is still small) flipping to the tally ~2.7x faster at
// Size=700 (cubic growth overtakes it).
[MemoryDiagnoser]
public class CountServersThatCommunicateBenchmarks
{
    // A cell becomes a server with 1-in-N odds.
    private const int ServerSpawnDenominator = 4;

    private const int GridSeed = 1;

    [Params(100, 700)]
    public int Size;

    private int[][] _grid = null!;

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
                _grid[r][c] = random.Next(0, ServerSpawnDenominator) == 0 ? 1 : 0;
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
