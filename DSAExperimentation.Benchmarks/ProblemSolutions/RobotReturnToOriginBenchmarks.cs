using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Robot Return to Origin (LC 657): a hand-written switch per move character vs.
// this repo's own HashMap<char,(int,int)> as a move-to-displacement lookup table.
[MemoryDiagnoser]
public class RobotReturnToOriginBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private char[] _moves = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        const string alphabet = "UDLR";
        _moves = Enumerable.Range(0, Length).Select(_ => alphabet[random.Next(alphabet.Length)]).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool SwitchStatement()
    {
        var (x, y) = ApplyMoves(_moves);
        return IsAtOrigin(x, y);
    }

    private static (int X, int Y) ApplyMoves(char[] moves)
    {
        var x = 0;
        var y = 0;

        foreach (var move in moves)
        {
            var (dx, dy) = DeltaFor(move);
            x += dx;
            y += dy;
        }

        return (x, y);
    }

    private static (int Dx, int Dy) DeltaFor(char move) => move switch
    {
        'U' => (0, 1),
        'D' => (0, -1),
        'L' => (-1, 0),
        'R' => (1, 0),
        _ => (0, 0),
    };

    private static bool IsAtOrigin(int x, int y) => x == 0 && y == 0;

    [Benchmark]
    public bool HashMapLookup()
    {
        var deltas = new HashMap<char, (int Dx, int Dy)>();
        deltas.Set('U', (0, 1));
        deltas.Set('D', (0, -1));
        deltas.Set('L', (-1, 0));
        deltas.Set('R', (1, 0));

        var x = 0;
        var y = 0;

        foreach (var move in _moves)
        {
            if (deltas.TryGetValue(move, out var delta))
            {
                x += delta.Dx;
                y += delta.Dy;
            }
        }

        return IsAtOrigin(x, y);
    }
}
