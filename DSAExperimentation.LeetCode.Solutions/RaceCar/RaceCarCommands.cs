using System.Collections.Immutable;
using DSAExperimentation.DataStructures.Graph.States;
using CarState = (int Position, int Speed);

namespace DSAExperimentation.LeetCode.Solutions.RaceCar;

// LC 818's commands as a state space over RaceCarStateSpace's box, for tier 1's StateGraph: every
// (position, speed) in the box is a state, numbered position by position from the box's far left,
// each position's speeds in the order RaceCarStateSpace lists them. 'A' moves to (position + speed,
// speed * 2) and 'R' to (position, -+1); an 'A' that leaves the box lands on no state, and the graph
// leaves it out. The state is a value tuple, so the graph's topology compiles to exact code rather
// than the shared code a reference-type state gets (ARCHITECTURE.md section 12.4).
internal readonly struct RaceCarCommands(RaceCarStateSpace box) : IStateSpace<CarState>
{
    // A car takes one of two commands: 'A' or 'R'.
    private const int CommandCount = 2;

    private readonly ImmutableArray<int> _speeds = [.. box.Speeds()];

    public int StateCount => (box.PositionBound + 1 + box.PositionBound) * _speeds.Length;

    public int MaxSuccessors => CommandCount;

    public CarState StateAt(int index)
    {
        var (fromFarLeft, speed) = Math.DivRem(index, _speeds.Length);

        return (fromFarLeft - box.PositionBound, _speeds[speed]);
    }

    public int WriteSuccessors(CarState state, Span<CarState> successors)
    {
        successors[0] = (state.Position + state.Speed, state.Speed * RaceCarMotion.SpeedDoublingFactor);
        successors[1] = (state.Position, ReversedSpeed(state.Speed));

        return CommandCount;
    }

    // 'R' does not move the car: it drops the speed to a single step in the opposite direction.
    private static int ReversedSpeed(int speed)
    {
        if (speed > 0)
        {
            return -RaceCarMotion.StartSpeed;
        }

        return RaceCarMotion.StartSpeed;
    }
}
