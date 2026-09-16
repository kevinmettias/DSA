using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SearchA2DMatrixII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are SearchA2DMatrixIISolution's, the same methods
// SearchA2DMatrixIITests proves correct. HasTargetByFullScan is the textbook
// O(rows*cols) baseline. HasTargetByPerRowBinarySearch composes this repo's own
// BinarySearch.Find over an ArraySequence<int> witness per row - O(rows*log(cols)),
// a genuine repo-primitive fit. HasTargetByStaircaseSearch is the specialized
// O(rows+cols) corner walk this problem is famous for - it is expected to win, and
// the point of including it is exactly
// that: showing when the general BinarySearch primitive is not the asymptotically
// optimal tool, the same lesson FloydWarshall teaches in the shortest-path
// cluster. Target is fixed below every matrix value so all three are forced
// through their full worst-case walk instead of an early exit making one look
// artificially competitive.
[MemoryDiagnoser]
public class SearchA2DMatrixIIBenchmarks
{
    private const int Target = -1;

    private int[][] _matrix = [];

    [Params(50, 300)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
        => _matrix = Enumerable.Range(0, Size)
            .Select(row => Enumerable.Range(0, Size).Select(col => (row * Size) + col).ToArray())
            .ToArray();

    [Benchmark(Baseline = true)]
    public bool HasTargetByFullScan() => SearchA2DMatrixIISolution.HasTargetByFullScan(_matrix, Target);

    [Benchmark]
    public bool HasTargetByPerRowBinarySearch() =>
        SearchA2DMatrixIISolution.HasTargetByPerRowBinarySearch(_matrix, Target);

    [Benchmark]
    public bool HasTargetByStaircaseSearch() =>
        SearchA2DMatrixIISolution.HasTargetByStaircaseSearch(_matrix, Target);
}
