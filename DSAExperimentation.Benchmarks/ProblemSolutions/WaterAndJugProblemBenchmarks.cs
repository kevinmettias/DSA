using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.WaterAndJugProblem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are WaterAndJugProblemSolution's, the same
// methods WaterAndJugProblemTests proves correct. Target is chosen
// unreachable (jugX+jugY-1, never a multiple of gcd(jugX,jugY) for jugY>1)
// so StackSearch and DepthFirstSearch both explore the identical
// O(jugX*jugY) implicit graph of fill states without ever short-circuiting -
// making that pair a constant-factor/allocation comparison. GcdFormula is
// the actual closed-form answer (Bezout's identity) and is expected to blow
// both away, showing the state-space search is the wrong tool once the
// number-theory shortcut is known.
[MemoryDiagnoser]
public class WaterAndJugProblemBenchmarks
{
    private const int JugYCapacityDivisor = 2;

    private int _jugX;

    private int _jugY;
    private int _target;
    [Params(40, 300)]
    public int Capacity { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _jugX = Capacity;
        _jugY = Capacity / JugYCapacityDivisor;
        _target = _jugX + _jugY - 1;
    }

    [Benchmark(Baseline = true)]
    public bool StackSearch() =>
        WaterAndJugProblemSolution.CanMeasureWaterByStackSearch(_jugX, _jugY, _target);

    [Benchmark]
    public bool DepthFirstSearch() =>
        WaterAndJugProblemSolution.CanMeasureWaterByDepthFirstSearch(_jugX, _jugY, _target);

    [Benchmark]
    public bool GcdFormula() =>
        WaterAndJugProblemSolution.CanMeasureWaterByGcdFormula(_jugX, _jugY, _target);
}
