using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DeleteGreatestValueInEachRow;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteGreatestValueInEachRowSolution's, the same
// methods DeleteGreatestValueInEachRowSolutionTests proves correct. The problem's own
// repeated simulation (O(Rows*Columns^2), and nothing short-circuits a round's
// full "find this row's current max" scan) against MergeSort over
// ArrayIndexedSequence sorting each row once plus a single column-wise max pass
// (O(Rows*Columns*log Columns)). The random grid is built once in [GlobalSetup]
// and neither strategy writes to it, so every iteration measures the same input.
// LC 2500 caps a row at 50 cells and a cell at 100, so the wider grid is 50 columns
// and every cell is drawn from 1..100.
public class DeleteGreatestValueInEachRowBenchmarks
{
    // LC problem number, reused as the deterministic grid seed.
    private const int RandomSeed = 2500;
    private const int LowestCell = 1;

    // One past LC 2500's largest cell, 100.
    private const int CellUpperBoundExclusive = 101;
    private const int Rows = 20;

    private int[][] _grid = [];

    [Params(5, 50)]
    public int Columns { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = Enumerable.Range(0, Rows)
            .Select(_ => SeededDraws.Values(Columns, LowestCell, CellUpperBoundExclusive, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedRowMaxScan() =>
        DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByRepeatedRowMaxScan(_grid);

    [Benchmark]
    public int MergeSortColumnMax() =>
        DeleteGreatestValueInEachRowSolution.DeleteGreatestValueByMergeSortColumnMax(_grid);
}
