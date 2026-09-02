using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Walking Robot Simulation II (LC 2069): a naive per-unit-step walk (mirroring
// this repo's own WalkingRobotSimulation, LC 874) vs. the O(1)-per-move modular
// arithmetic formula this problem actually needs once move()'s step count can
// reach 1e9 (WalkingRobotSimulationIITests precedent). Both track the same fixed
// perimeter loop; only the per-Move cost differs - O(num steps) vs O(1).
[MemoryDiagnoser]
public class WalkingRobotSimulationIIBenchmarks
{
    private const int Width = 100_000;
    private const int Height = 100_000;
    private const int MoveCount = 20;

    [Params(1_000, 50_000)]
    public int StepsPerMove;

    private int[] _moves = null!;

    [GlobalSetup]
    public void Setup()
    {
        _moves = Enumerable.Repeat(StepsPerMove, MoveCount).ToArray();
    }

    [Benchmark(Baseline = true)]
    public (int X, int Y) NaiveStepSimulation()
    {
        var robot = new NaiveRobotSim(Width, Height);

        foreach (var steps in _moves)
        {
            robot.Move(steps);
        }

        return robot.GetPos();
    }

    [Benchmark]
    public (int X, int Y) FormulaBasedMove()
    {
        var robot = new FormulaRobotSim(Width, Height);

        foreach (var steps in _moves)
        {
            robot.Move(steps);
        }

        return robot.GetPos();
    }

    private sealed class NaiveRobotSim(int width, int height)
    {
        private static readonly int[] DeltaX = [1, 0, -1, 0];
        private static readonly int[] DeltaY = [0, 1, 0, -1];

        private int _x;
        private int _y;
        private int _direction;

        public void Move(int num)
        {
            for (var step = 0; step < num; step++)
            {
                var nextX = _x + DeltaX[_direction];
                var nextY = _y + DeltaY[_direction];

                if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)
                {
                    _direction = (_direction + 1) % DeltaX.Length;
                    nextX = _x + DeltaX[_direction];
                    nextY = _y + DeltaY[_direction];
                }

                _x = nextX;
                _y = nextY;
            }
        }

        public (int X, int Y) GetPos() => (_x, _y);
    }

    private sealed class FormulaRobotSim(int width, int height)
    {
        private readonly int _maxX = width - 1;
        private readonly int _maxY = height - 1;
        private readonly long _perimeter = 2L * (width - 1 + height - 1);
        private long _totalSteps;

        public void Move(int num) => _totalSteps += num;

        public (int X, int Y) GetPos()
        {
            var d = _totalSteps % _perimeter;
            return d == 0 ? (0, 0) : LocateOnEastEdge(d);
        }

        private (int X, int Y) LocateOnEastEdge(long d)
            => d <= _maxX ? ((int)d, 0) : LocateOnNorthEdge(d - _maxX);

        private (int X, int Y) LocateOnNorthEdge(long d)
            => d <= _maxY ? (_maxX, (int)d) : LocateOnWestEdge(d - _maxY);

        private (int X, int Y) LocateOnWestEdge(long d)
            => d <= _maxX ? (_maxX - (int)d, _maxY) : LocateOnSouthEdge(d - _maxX);

        private (int X, int Y) LocateOnSouthEdge(long d) => (0, _maxY - (int)d);
    }
}
