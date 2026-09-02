using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SeparateSquaresII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SeparateSquaresIISolution's, the same methods
// SeparateSquaresIITests proves correct. Each arm takes the already-parsed
// IReadOnlyList<Square>, so building Square records from LC's raw int[][] shape is
// charged to [GlobalSetup] rather than the sweep being measured. Coordinates are
// drawn from a small range relative to SquareCount so squares actually overlap -
// disjoint squares would make every band's active set size 1 and hide the
// coverage-length step's own cost entirely.
[MemoryDiagnoser]
public class SeparateSquaresIIBenchmarks
{
    // LC problem number, reused as the deterministic square seed.
    private const int SquareSeed = 3454;
    private const int CoordinateRange = 100;
    private const int MaxSide = 20;

    [Params(20, 200)]
    public int SquareCount;

    private Square[] _squares = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(SquareSeed);
        _squares = Enumerable.Range(0, SquareCount)
            .Select(_ => new Square(
                random.Next(0, CoordinateRange),
                random.Next(0, CoordinateRange),
                random.Next(1, MaxSide)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public double EventSweep() => SeparateSquaresIISolution.MinYByEventSweep(_squares);

    [Benchmark]
    public double IntervalSetSweep() => SeparateSquaresIISolution.MinYByIntervalSetSweep(_squares);
}
