using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StrangePrinterII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StrangePrinterIISolution's, the same methods
// StrangePrinterIITests proves correct. Each arm is handed the prepared
// List<ColorNode> its hoisted overload takes, so graph construction is charged to
// [GlobalSetup] rather than to the cycle check being measured. Colors form a
// guaranteed-acyclic chain (every "must print before" edge points from a lower
// color id to a higher one, capped fan-out) so both strategies run their full real
// workload instead of an early cycle bailout.
[MemoryDiagnoser]
public class StrangePrinterIIBenchmarks
{
    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int ColorCount;

    private List<ColorNode> _colors = null!;

    [GlobalSetup]
    public void Setup()
    {
        _colors = Enumerable.Range(0, ColorCount).Select(color => new ColorNode(color)).ToList();

        for (var i = 0; i < ColorCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, ColorCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _colors[i].MustPrintBefore.Add(_colors[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool NaiveRescan() => StrangePrinterIISolution.IsPrintableByNaiveRescan(_colors);

    [Benchmark]
    public bool KahnsTopologicalSort() => StrangePrinterIISolution.IsPrintableByKahnsTopologicalSort(_colors);
}
