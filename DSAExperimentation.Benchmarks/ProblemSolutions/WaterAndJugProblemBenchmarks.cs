using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Water and Jug Problem (LC 365): three genuinely distinct algorithms for the same
// reachability question, ShortestPathAlgorithmBenchmarks.cs's "3+ algorithms"
// shape. HandRolledStackSearch and TraverseComposed both explore the identical
// O(jugX*jugY) implicit graph of fill states - Target is chosen unreachable
// (jugX+jugY-1, never a multiple of gcd(jugX,jugY) for jugY>1) so neither ever
// short-circuits, making that pair a constant-factor/allocation comparison, the
// same framing ExpressionAddOperatorsBenchmarks uses. GcdFormula is the actual
// closed-form answer (Bezout's identity) and is expected to blow both away - the
// point, mirroring FloydWarshall's role there, is showing the state-space search
// is the wrong tool once the number-theory shortcut is known, not just timing it.
[MemoryDiagnoser]
public class WaterAndJugProblemBenchmarks
{
    private const int JugYCapacityDivisor = 2;

    [Params(40, 300)]
    public int Capacity;

    private int _jugX;
    private int _jugY;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        _jugX = Capacity;
        _jugY = Capacity / JugYCapacityDivisor;
        _target = _jugX + _jugY - 1;
    }

    // Iterative, not recursive: this state graph's DFS order can nest thousands of
    // frames deep at the large Capacity param, so a hand-rolled BCL Stack<T> loop
    // stands in for "no repo primitive" here instead of risking a stack overflow -
    // the exact shape DepthFirstSearch.Traverse itself uses internally, just without
    // reusing this repo's own Stack<T> for the frontier.
    [Benchmark(Baseline = true)]
    public bool HandRolledStackSearch()
    {
        var visited = new HashSet<(int X, int Y)>();
        var pending = new Stack<(int X, int Y)>();
        pending.Push((0, 0));

        while (pending.TryPop(out var state))
        {
            if (ProcessState(state, visited, pending))
            {
                return true;
            }
        }

        return false;
    }

    private bool ProcessState(
        (int X, int Y) state, HashSet<(int X, int Y)> visited, Stack<(int X, int Y)> pending)
    {
        if (!visited.Add(state))
        {
            return false;
        }

        if (state.X + state.Y == _target)
        {
            return true;
        }

        foreach (var next in Successors(state))
        {
            if (!visited.Contains(next))
            {
                pending.Push(next);
            }
        }

        return false;
    }

    [Benchmark]
    public bool TraverseComposed()
    {
        var start = (X: 0, Y: 0);
        var visited = DepthFirstSearch.Traverse(start, Successors);
        return visited.Any(state => state.X + state.Y == _target);
    }

    [Benchmark]
    public bool GcdFormula()
    {
        if (_target > _jugX + _jugY)
        {
            return false;
        }

        return _target % Gcd(_jugX, _jugY) == 0;
    }

    private IEnumerable<(int X, int Y)> Successors((int X, int Y) state)
    {
        yield return (_jugX, state.Y);
        yield return (state.X, _jugY);
        yield return (0, state.Y);
        yield return (state.X, 0);

        var pourXtoY = Math.Min(state.X, _jugY - state.Y);
        yield return (state.X - pourXtoY, state.Y + pourXtoY);

        var pourYtoX = Math.Min(state.Y, _jugX - state.X);
        yield return (state.X + pourYtoX, state.Y - pourYtoX);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
