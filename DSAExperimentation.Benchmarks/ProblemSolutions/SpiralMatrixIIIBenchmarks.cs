using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Spiral Matrix III (LC 885): the same "defensive visited tracking" shape
// Spiral Matrix II's own baseline uses - this repo's Set<(int,int)> guards
// every step against re-adding a cell - against the direct growing-step
// direction-vector walk that trusts the strictly increasing stride lengths
// to never revisit a cell and skips tracking entirely. Both visit exactly
// rows*cols in-bounds cells; the gap under [MemoryDiagnoser] is the Set's
// per-step hashing/allocation overhead, not algorithm class.
[MemoryDiagnoser]
public class SpiralMatrixIIIBenchmarks
{
    [Params(20, 200)]
    public int Size;

    private int _rows;
    private int _cols;
    private int _rStart;
    private int _cStart;

    [GlobalSetup]
    public void Setup()
    {
        _rows = Size;
        _cols = Size;
        _rStart = Size / 2;
        _cStart = Size / 2;
    }

    [Benchmark(Baseline = true)]
    public int[][] DirectionVectorWithVisitedSet()
    {
        var total = _rows * _cols;
        var visited = new Set<(int Row, int Col)>();
        var result = new List<int[]>();
        result.Add([_rStart, _cStart]);
        visited.TryAdd((_rStart, _cStart));

        if (total == 1)
        {
            return result.ToArray();
        }

        int[] deltaRow = [0, 1, 0, -1];
        int[] deltaCol = [1, 0, -1, 0];
        var row = _rStart;
        var col = _cStart;
        var step = 1;
        var direction = 0;

        while (result.Count < total)
        {
            for (var turn = 0; turn < 2; turn++)
            {
                for (var i = 0; i < step; i++)
                {
                    row += deltaRow[direction];
                    col += deltaCol[direction];

                    if (row >= 0 && row < _rows && col >= 0 && col < _cols && visited.TryAdd((row, col)))
                    {
                        result.Add([row, col]);
                        if (result.Count == total)
                        {
                            return result.ToArray();
                        }
                    }
                }

                direction = (direction + 1) % 4;
            }

            step++;
        }

        return result.ToArray();
    }

    [Benchmark]
    public int[][] GrowingStepDirectionWalk()
    {
        var total = _rows * _cols;
        var result = new List<int[]>();
        result.Add([_rStart, _cStart]);

        if (total == 1)
        {
            return result.ToArray();
        }

        int[] deltaRow = [0, 1, 0, -1];
        int[] deltaCol = [1, 0, -1, 0];
        var row = _rStart;
        var col = _cStart;
        var step = 1;
        var direction = 0;

        while (result.Count < total)
        {
            for (var turn = 0; turn < 2; turn++)
            {
                for (var i = 0; i < step; i++)
                {
                    row += deltaRow[direction];
                    col += deltaCol[direction];

                    if (row >= 0 && row < _rows && col >= 0 && col < _cols)
                    {
                        result.Add([row, col]);
                        if (result.Count == total)
                        {
                            return result.ToArray();
                        }
                    }
                }

                direction = (direction + 1) % 4;
            }

            step++;
        }

        return result.ToArray();
    }
}
