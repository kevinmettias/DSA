namespace DSAExperimentation.LeetCode.WalkingRobotSimulationII;

// LeetCode 2069. Walking Robot Simulation II: a robot starts at (0, 0) facing east
// on a width x height grid and only ever walks the outer perimeter, turning
// counterclockwise whenever the next cell would leave the grid. Move(num) walks num
// cells, GetPos reports where it stands and GetDir which way it faces.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and three operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md section 17.3) takes the form of two full classes
// implementing the shared IRobot surface below, the same shape
// DesignBrowserHistorySolution uses for its own instance-API problem (LC 1472).
//
// Both strategies walk the identical perimeter loop; only the cost of Move differs.
// The baseline takes the cells one at a time, which is what this repo's own
// WalkingRobotSimulation (LC 874) does and is exactly what LC 2069 raises the limits
// to defeat: width, height and each Move's step count all reach 1e9. The perimeter
// strategy keeps only the total distance travelled and decodes position and heading
// from it modulo the loop length, so Move is O(1).
internal static class WalkingRobotSimulationIISolution
{
    // LeetCode's own direction spellings, shared by both strategies so the two
    // cannot drift apart on the answer's wording.
    private const string East = "East";
    private const string North = "North";
    private const string West = "West";
    private const string South = "South";

    // Indexed by the step simulation's heading, which turns counterclockwise:
    // east -> north -> west -> south.
    private static readonly string[] DirectionNames = [East, North, West, South];

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IRobot
    {
        void Move(int num);

        (int X, int Y) GetPos();

        string GetDir();
    }

    // The textbook baseline this composition has to justify itself against: hold the
    // cell and the heading, take one step at a time, and turn when the next cell
    // would fall off the grid. Deliberately BCL-only, and O(num) per Move.
    internal sealed class RobotByStepSimulation(int width, int height) : IRobot
    {
        // Counterclockwise from east, so a turn is +1 modulo four.
        private static readonly int[] DeltaX = [1, 0, -1, 0];
        private static readonly int[] DeltaY = [0, 1, 0, -1];

        private int _x;
        private int _y;
        private int _direction;

        public void Move(int num)
        {
            for (var step = 0; step < num; step++)
            {
                TakeStep();
            }
        }

        private void TakeStep()
        {
            var nextX = _x + DeltaX[_direction];
            var nextY = _y + DeltaY[_direction];

            if (IsOffGrid(nextX, nextY))
            {
                _direction = (_direction + 1) % DeltaX.Length;
                nextX = _x + DeltaX[_direction];
                nextY = _y + DeltaY[_direction];
            }

            _x = nextX;
            _y = nextY;
        }

        private bool IsOffGrid(int x, int y) => x < 0 || x >= width || y < 0 || y >= height;

        public (int X, int Y) GetPos() => (_x, _y);

        public string GetDir() => DirectionNames[_direction];
    }

    // The O(1)-per-Move answer: the robot's path is a fixed loop of length
    // 2 * (width + height - 2), so its whole state is the total distance travelled
    // and every query is index arithmetic on that loop - the same
    // "boundary-shrinking index arithmetic, no repo Representation/Operations
    // primitive to compose" category this repo's Spiral Matrix coverage established.
    internal sealed class RobotByPerimeterFormula(int width, int height) : IRobot
    {
        private const int PerimeterSides = 2;

        private readonly int _maxX = width - 1;
        private readonly int _maxY = height - 1;
        private readonly long _perimeter = PerimeterSides * ((long)width - 1 + height - 1);
        private long _totalSteps;

        public void Move(int num) => _totalSteps += num;

        public (int X, int Y) GetPos()
        {
            var travelled = _totalSteps % _perimeter;

            if (travelled == 0)
            {
                return (0, 0);
            }

            var (edge, offset) = Locate(travelled);

            return ToPosition(edge, offset);
        }

        private (int X, int Y) ToPosition(Edge edge, long offset) => edge switch
        {
            Edge.East => ((int)offset, 0),
            Edge.North => (_maxX, (int)offset),
            Edge.West => (_maxX - (int)offset, _maxY),
            _ => (0, _maxY - (int)offset),
        };

        // The origin is the one cell the loop passes through twice over: the robot
        // faces east there before it has moved at all, and south when it has come
        // all the way back round to it.
        public string GetDir()
        {
            if (_totalSteps == 0)
            {
                return East;
            }

            var travelled = _totalSteps % _perimeter;

            if (travelled == 0)
            {
                return South;
            }

            var (edge, _) = Locate(travelled);

            return ToDirection(edge);
        }

        private static string ToDirection(Edge edge) => edge switch
        {
            Edge.East => East,
            Edge.North => North,
            Edge.West => West,
            _ => South,
        };

        // The perimeter walk shared by GetPos and GetDir: which edge the distance
        // travelled lands on, and how far along that edge.
        private (Edge Edge, long Offset) Locate(long travelled)
            => travelled <= _maxX ? EdgePoint(Edge.East, travelled) : LocateOnNorthEdge(travelled - _maxX);

        private (Edge Edge, long Offset) LocateOnNorthEdge(long travelled)
            => travelled <= _maxY ? EdgePoint(Edge.North, travelled) : LocateOnWestEdge(travelled - _maxY);

        private (Edge Edge, long Offset) LocateOnWestEdge(long travelled)
            => travelled <= _maxX ? EdgePoint(Edge.West, travelled) : EdgePoint(Edge.South, travelled - _maxX);

        private static (Edge Edge, long Offset) EdgePoint(Edge edge, long offset) => (edge, offset);

        // Which of the perimeter's four straight runs the robot is currently on.
        private enum Edge
        {
            East,
            North,
            West,
            South,
        }
    }
}
