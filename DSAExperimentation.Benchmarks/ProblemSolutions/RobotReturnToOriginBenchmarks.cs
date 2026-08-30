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
        var x = 0;
        var y = 0;

        foreach (var move in _moves)
        {
            switch (move)
            {
                case 'U':
                    y++;
                    break;
                case 'D':
                    y--;
                    break;
                case 'L':
                    x--;
                    break;
                case 'R':
                    x++;
                    break;
            }
        }

        return x == 0 && y == 0;
    }

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

        return x == 0 && y == 0;
    }
}
