using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CyclicallyRotatingAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CyclicallyRotatingAGridSolution's, the same methods
// CyclicallyRotatingAGridTests proves correct. [GlobalSetup] fills the template
// grid once; each strategy copies it internally, which is the same per-call clone
// both arms already paid before the migration, so what the comparison shows is the
// O(k)-per-ring stepwise walk against the same walk with k reduced modulo each
// ring's length first.
[MemoryDiagnoser]
public class CyclicallyRotatingAGridBenchmarks
{
    // Far more turns than any ring is long, and prime, so no ring's rotation
    // happens to fall out even.
    private const int UnreducedSteps = 1_000_003;

    private int[][] _template = [];

    [Params(10, 40)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _template = new int[Size][];
        var value = 0;

        for (var row = 0; row < Size; row++)
        {
            _template[row] = new int[Size];
            for (var col = 0; col < Size; col++)
            {
                _template[row][col] = value++;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] StepwiseQueue() =>
        CyclicallyRotatingAGridSolution.RotateGridByStepwiseQueue(_template, UnreducedSteps);

    [Benchmark]
    public int[][] DequeRings() =>
        CyclicallyRotatingAGridSolution.RotateGridByDequeRings(_template, UnreducedSteps);
}
