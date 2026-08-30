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
        var direction = 0;
        var x = 0;
        var y = 0;

        foreach (var instruction in _instructions)
        {
            switch (instruction)
            {
                case 'G':
                    switch (direction)
                    {
                        case 0: y++; break;
                        case 1: x++; break;
                        case 2: y--; break;
                        default: x--; break;
                    }
                    break;
                case 'L':
                    direction = (direction + 3) % 4;
                    break;
                case 'R':
                    direction = (direction + 1) % 4;
                    break;
            }
        }

        return (x == 0 && y == 0) || direction != 0;
    }

    [Benchmark]
    public bool HashMapLookup()
    {
        var stepDeltas = new HashMap<int, (int Dx, int Dy)>();
        stepDeltas.Set(0, (0, 1));
        stepDeltas.Set(1, (1, 0));
        stepDeltas.Set(2, (0, -1));
        stepDeltas.Set(3, (-1, 0));

        var direction = 0;
        var x = 0;
        var y = 0;

        foreach (var instruction in _instructions)
        {
            switch (instruction)
            {
                case 'G':
                    stepDeltas.TryGetValue(direction, out var delta);
                    x += delta.Dx;
                    y += delta.Dy;
                    break;
                case 'L':
                    direction = (direction + 3) % 4;
                    break;
                case 'R':
                    direction = (direction + 1) % 4;
                    break;
            }
        }

        return (x == 0 && y == 0) || direction != 0;
    }
}
