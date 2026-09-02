using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SearchA2DMatrixII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are SearchA2DMatrixIISolution's, the same methods
// SearchA2DMatrixIITests proves correct. FullScan is the textbook O(rows*cols)
// baseline. PerRowBinarySearch composes this repo's own BinarySearch.Find over an
// ArraySequence<int> witness per row - O(rows*log(cols)), a genuine repo-primitive
// fit. StaircaseSearch is the specialized O(rows+cols) corner walk this problem is
// famous for - it is expected to win, and the point of including it is exactly
// that: showing when the general BinarySearch primitive is not the asymptotically
// optimal tool, the same lesson FloydWarshall teaches in the shortest-path
// cluster. Target is fixed below every matrix value so all three are forced
// through their full worst-case walk instead of an early exit making one look
// artificially competitive.
[MemoryDiagnoser]
public class SearchA2DMatrixIIBenchmarks
{
    private const int Target = -1;

    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
        => _matrix = Enumerable.Range(0, Size)
            .Select(row => Enumerable.Range(0, Size).Select(col => (row * Size) + col).ToArray())
            .ToArray();

    [Benchmark(Baseline = true)]
    public bool FullScan() => SearchA2DMatrixIISolution.SearchMatrixByFullScan(_matrix, Target);

    [Benchmark]
    public bool PerRowBinarySearch() => SearchA2DMatrixIISolution.SearchMatrixByPerRowBinarySearch(_matrix, Target);

    [Benchmark]
    public bool StaircaseSearch() => SearchA2DMatrixIISolution.SearchMatrixByStaircaseSearch(_matrix, Target);
}
