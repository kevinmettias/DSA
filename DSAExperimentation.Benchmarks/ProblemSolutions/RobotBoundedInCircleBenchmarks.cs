using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Robot Bounded In Circle (LC 1041): a hand-written switch per instruction
// character vs. this repo's own HashMap<int,(int,int)> as a facing-direction-to-
// step-delta lookup table, the same contrast RobotReturnToOriginBenchmarks already
// draws for its simpler fixed-per-character delta case.
[MemoryDiagnoser]
public class RobotBoundedInCircleBenchmarks
{
    private const int SouthDirection = 2;
    private const int WestDirection = 3;
    private const int DirectionCount = 4;
    private const int LeftTurnOffset = 3;

    [Params(200, 5_000)]
    public int Length;

    private char[] _instructions = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        const string alphabet = "GLR";
        _instructions = Enumerable.Range(0, Length).Select(_ => alphabet[random.Next(alphabet.Length)]).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool SwitchStatement()
    {
        var state = new RobotState();

        foreach (var instruction in _instructions)
        {
            ApplySwitchInstruction(state, instruction);
        }

        return IsBounded(state);
    }

    private static void ApplySwitchInstruction(RobotState state, char instruction)
    {
        switch (instruction)
        {
            case 'G':
                MoveForward(state);
                break;
            case 'L':
                TurnLeft(state);
                break;
            case 'R':
                TurnRight(state);
                break;
        }
    }

    private static void MoveForward(RobotState state)
    {
        switch (state.Direction)
        {
            case 0: state.Y++; break;
            case 1: state.X++; break;
            case SouthDirection: state.Y--; break;
            default: state.X--; break;
        }
    }

    [Benchmark]
    public bool HashMapLookup()
    {
        var stepDeltas = BuildStepDeltas();
        var state = new RobotState();

        foreach (var instruction in _instructions)
        {
            ApplyHashMapInstruction(stepDeltas, state, instruction);
        }

        return IsBounded(state);
    }

    private static HashMap<int, (int Dx, int Dy)> BuildStepDeltas()
    {
        var stepDeltas = new HashMap<int, (int Dx, int Dy)>();
        stepDeltas.Set(0, (0, 1));
        stepDeltas.Set(1, (1, 0));
        stepDeltas.Set(SouthDirection, (0, -1));
        stepDeltas.Set(WestDirection, (-1, 0));
        return stepDeltas;
    }

    private static void ApplyHashMapInstruction(HashMap<int, (int Dx, int Dy)> stepDeltas, RobotState state, char instruction)
    {
        switch (instruction)
        {
            case 'G':
                stepDeltas.TryGetValue(state.Direction, out var delta);
                state.X += delta.Dx;
                state.Y += delta.Dy;
                break;
            case 'L':
                TurnLeft(state);
                break;
            case 'R':
                TurnRight(state);
                break;
        }
    }

    private static void TurnLeft(RobotState state) => state.Direction = (state.Direction + LeftTurnOffset) % DirectionCount;

    private static void TurnRight(RobotState state) => state.Direction = (state.Direction + 1) % DirectionCount;

    private static bool IsBounded(RobotState state) => (state.X == 0 && state.Y == 0) || state.Direction != 0;

    private sealed class RobotState
    {
        public int Direction;
        public int X;
        public int Y;
    }
}
