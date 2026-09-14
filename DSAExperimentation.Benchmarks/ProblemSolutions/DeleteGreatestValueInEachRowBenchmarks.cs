using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DeleteGreatestValueInEachRow;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteGreatestValueInEachRowSolution's, the same
// methods DeleteGreatestValueInEachRowTests proves correct. The problem's own
// repeated simulation (O(Rows*Columns^2), and nothing short-circuits a round's
// full "find this row's current max" scan) against MergeSort over
// ArrayIndexedSequence sorting each row once plus a single column-wise max pass
// (O(Rows*Columns*log Columns)). The random grid is built once in [GlobalSetup]
// and neither strategy writes to it, so every iteration measures the same input.
[MemoryDiagnoser]
public class DeleteGreatestValueInEachRowBenchmarks
{
    // LC problem number, reused as the deterministic grid seed.
    private const int RandomSeed = 2500;
    private const int ValueBound = 100_000;
    private const int Rows = 20;

    private int[][] _grid = null!;

    [Params(50, 400)]
    public int Columns;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = Enumerable.Range(0, Rows)
            .Select(_ => Enumerable.Range(0, Columns).Select(_ => random.Next(ValueBound)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedRowMaxScan() =>
        DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByRepeatedRowMaxScan(_grid);

    [Benchmark]
    public int MergeSortColumnMax() =>
        DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByMergeSortColumnMax(_grid);
}
