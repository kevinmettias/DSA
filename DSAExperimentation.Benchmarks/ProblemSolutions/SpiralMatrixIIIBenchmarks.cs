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
    private const int CenterDivisor = 2;
    private const int TurnsPerStepLength = 2;
    private const int DirectionCount = 4;

    private static readonly int[] DeltaRow = [0, 1, 0, -1];
    private static readonly int[] DeltaCol = [1, 0, -1, 0];

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
        _rStart = Size / CenterDivisor;
        _cStart = Size / CenterDivisor;
    }

    [Benchmark(Baseline = true)]
    public int[][] DirectionVectorWithVisitedSet()
    {
        var total = _rows * _cols;
        var visited = new Set<(int Row, int Col)>();
        var result = new List<int[]> { new[] { _rStart, _cStart } };
        visited.TryAdd((_rStart, _cStart));

        if (total == 1)
        {
            return result.ToArray();
        }

        var context = new SpiralWalkContext(result, total, visited);
        WalkSpiral(context, new SpiralPosition { Row = _rStart, Col = _cStart });

        return result.ToArray();
    }

    [Benchmark]
    public int[][] GrowingStepDirectionWalk()
    {
        var total = _rows * _cols;
        var result = new List<int[]> { new[] { _rStart, _cStart } };

        if (total == 1)
        {
            return result.ToArray();
        }

        var context = new SpiralWalkContext(result, total, null);
        WalkSpiral(context, new SpiralPosition { Row = _rStart, Col = _cStart });

        return result.ToArray();
    }

    private void WalkSpiral(SpiralWalkContext context, SpiralPosition position)
    {
        var step = 1;
        var direction = 0;

        while (context.Result.Count < context.Total)
        {
            for (var turn = 0; turn < TurnsPerStepLength; turn++)
            {
                if (WalkStride(context, ref position, direction, step))
                {
                    return;
                }

                direction = (direction + 1) % DirectionCount;
            }

            step++;
        }
    }

    private bool WalkStride(SpiralWalkContext context, ref SpiralPosition position, int direction, int step)
    {
        for (var i = 0; i < step; i++)
        {
            if (TryVisitCell(context, ref position, direction))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryVisitCell(SpiralWalkContext context, ref SpiralPosition position, int direction)
    {
        position.Row += DeltaRow[direction];
        position.Col += DeltaCol[direction];

        if (!IsInBounds(position.Row, position.Col))
        {
            return false;
        }

        if (context.Visited != null && !context.Visited.TryAdd((position.Row, position.Col)))
        {
            return false;
        }

        context.Result.Add([position.Row, position.Col]);

        return context.Result.Count == context.Total;
    }

    private bool IsInBounds(int row, int col) => row >= 0 && row < _rows && col >= 0 && col < _cols;

    private readonly record struct SpiralWalkContext(List<int[]> Result, int Total, Set<(int Row, int Col)>? Visited);

    private struct SpiralPosition
    {
        public int Row;
        public int Col;
    }
}
