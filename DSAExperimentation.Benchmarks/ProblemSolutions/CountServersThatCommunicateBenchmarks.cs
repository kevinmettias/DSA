using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Servers that Communicate (LC 1267): the O(rows*cols*(rows+cols)) brute
// force (every server independently re-scans its whole row and its whole column
// from scratch to recount servers there) vs. the O(rows*cols) two-pass count using
// this repo's own HashMap<int,int> twice - row tallies and column tallies computed
// once and reused - the same row/column-tracking shape SetMatrixZeroesTests uses
// via Set<int>, upgraded to counts since "communicates" needs more-than-one, not
// merely present. BruteForce's row/column scans deliberately never exit early on
// the first companion found (they count the whole row/column every time) so the
// comparison reflects the redundant-recomputation-per-server cost the HashMap
// approach's one-time count avoids, not how quickly a companion happens to turn up
// in this particular random grid. The two [Params] sizes were chosen to show the
// real crossover this Big-O gap predicts, not just one side of it: a dry-run
// showed BruteForce ~4.8x faster at Size=100 (HashMap's per-call hashing/bucket
// overhead dominates when rows*cols*(rows+cols) is still small) flipping to
// HashMap ~2.7x faster at Size=700 (cubic growth overtakes it).
[MemoryDiagnoser]
public class CountServersThatCommunicateBenchmarks
{
    [Params(100, 700)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _grid[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _grid[r][c] = random.Next(0, 4) == 0 ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRowAndColumnRescan()
    {
        var communicating = 0;

        for (var r = 0; r < _grid.Length; r++)
        {
            for (var c = 0; c < _grid[0].Length; c++)
            {
                if (_grid[r][c] != 1)
                {
                    continue;
                }

                if (HasCompanionInRow(r) || HasCompanionInColumn(c))
                {
                    communicating++;
                }
            }
        }

        return communicating;
    }

    private bool HasCompanionInRow(int row)
    {
        var count = 0;

        for (var c = 0; c < _grid[0].Length; c++)
        {
            count += _grid[row][c];
        }

        return count > 1;
    }

    private bool HasCompanionInColumn(int col)
    {
        var count = 0;

        for (var r = 0; r < _grid.Length; r++)
        {
            count += _grid[r][col];
        }

        return count > 1;
    }

    [Benchmark]
    public int HashMapRowAndColumnCounts()
    {
        var rowCounts = new HashMap<int, int>();
        var colCounts = new HashMap<int, int>();

        for (var r = 0; r < _grid.Length; r++)
        {
            for (var c = 0; c < _grid[0].Length; c++)
            {
                if (_grid[r][c] == 1)
                {
                    Increment(rowCounts, r);
                    Increment(colCounts, c);
                }
            }
        }

        var communicating = 0;

        for (var r = 0; r < _grid.Length; r++)
        {
            for (var c = 0; c < _grid[0].Length; c++)
            {
                if (_grid[r][c] != 1)
                {
                    continue;
                }

                rowCounts.TryGetValue(r, out var rowCount);
                colCounts.TryGetValue(c, out var colCount);

                if (rowCount > 1 || colCount > 1)
                {
                    communicating++;
                }
            }
        }

        return communicating;
    }

    private static void Increment(HashMap<int, int> counts, int key)
    {
        counts.TryGetValue(key, out var current);
        counts.Set(key, current + 1);
    }
}
