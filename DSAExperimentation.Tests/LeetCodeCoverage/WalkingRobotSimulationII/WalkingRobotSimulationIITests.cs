namespace DSAExperimentation.Tests.LeetCodeCoverage.WalkingRobotSimulationII;

// LeetCode 2069. Walking Robot Simulation II: width/height and each move()'s step
// count can both reach 1e9, so a naive per-unit-step walk (this repo's own
// WalkingRobotSimulation, LC 874, precedent) is too slow here - move() has to be
// O(1). The robot's path is a fixed perimeter loop of length
// 2*(width+height-2), so every position is just an index into that loop: only the
// total distance traveled so far (mod the perimeter) is kept as state, and
// GetPos/GetDir decode it with direct modular-arithmetic boundary-segment math -
// the same "boundary-shrinking index arithmetic, no repo Representation/Operations
// primitive to compose" category this repo's Spiral Matrix / Spiral Matrix II
// coverage already established.
public sealed class WalkingRobotSimulationIITests
{
    [Fact]
    public void MoveGetPosGetDir_LeetCodeStyleSequence_MatchesHandSimulatedPath()
    {
        var robot = new Robot(6, 3);

        robot.Move(2);
        robot.Move(2);
        robot.Move(2);
        Assert.Equal((5, 1), robot.GetPos());
        Assert.Equal("North", robot.GetDir());

        robot.Move(3);
        robot.Move(3);
        robot.Move(3);
        robot.Move(3);
        robot.Move(3);
        Assert.Equal((5, 2), robot.GetPos());
        Assert.Equal("North", robot.GetDir());
    }

    [Fact]
    public void GetDir_CompletesExactlyOneFullPerimeterLoop_ReturnsSouthAtOrigin()
    {
        var robot = new Robot(6, 3);

        robot.Move(14); // exactly one full perimeter loop: 2*((6-1)+(3-1))

        Assert.Equal((0, 0), robot.GetPos());
        Assert.Equal("South", robot.GetDir());
    }

    [Fact]
    public void GetDir_NoMoveYet_ReturnsEastAtOrigin()
    {
        var robot = new Robot(6, 3);

        Assert.Equal((0, 0), robot.GetPos());
        Assert.Equal("East", robot.GetDir());
    }

    private sealed class Robot
    {
        private readonly int _maxX;
        private readonly int _maxY;
        private readonly long _perimeter;
        private long _totalSteps;

        public Robot(int width, int height)
        {
            _maxX = width - 1;
            _maxY = height - 1;
            _perimeter = 2L * (_maxX + _maxY);
        }

        public void Move(int num) => _totalSteps += num;

        // Which of the perimeter's 4 straight runs the robot is currently on.
        private enum Edge
        {
            East,
            North,
            West,
            South,
        }

        public (int X, int Y) GetPos()
        {
            var d = _totalSteps % _perimeter;
            if (d == 0)
            {
                return (0, 0);
            }

            var (edge, offset) = LocateOnEastEdge(d);
            return ToPosition(edge, offset);
        }

        public string GetDir()
        {
            if (_totalSteps == 0)
            {
                return "East";
            }

            var d = _totalSteps % _perimeter;
            if (d == 0)
            {
                return "South";
            }

            var (edge, _) = LocateOnEastEdge(d);
            return ToDirection(edge);
        }

        // The perimeter walk shared by GetPos/GetDir: which edge the remaining
        // distance `d` lands on, and the offset along that edge.
        private (Edge Edge, long Offset) LocateOnEastEdge(long d)
            => d <= _maxX ? (Edge.East, d) : LocateOnNorthEdge(d - _maxX);

        private (Edge Edge, long Offset) LocateOnNorthEdge(long d)
            => d <= _maxY ? (Edge.North, d) : LocateOnWestEdge(d - _maxY);

        private (Edge Edge, long Offset) LocateOnWestEdge(long d)
            => d <= _maxX ? (Edge.West, d) : (Edge.South, d - _maxX);

        private (int X, int Y) ToPosition(Edge edge, long offset) => edge switch
        {
            Edge.East => ((int)offset, 0),
            Edge.North => (_maxX, (int)offset),
            Edge.West => (_maxX - (int)offset, _maxY),
            _ => (0, _maxY - (int)offset),
        };

        private static string ToDirection(Edge edge) => edge switch
        {
            Edge.East => "East",
            Edge.North => "North",
            Edge.West => "West",
            _ => "South",
        };
    }
}
