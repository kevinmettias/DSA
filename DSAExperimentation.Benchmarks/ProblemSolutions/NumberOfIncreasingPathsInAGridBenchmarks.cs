using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfIncreasingPathsInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfIncreasingPathsInAGridSolution's, the same
// methods NumberOfIncreasingPathsInAGridTests proves correct, run over the same
// row-major strictly-increasing Size x Size matrix
// LongestIncreasingPathInAMatrixBenchmarks uses (every cell's only increasing
// neighbors are right/down, the classic Unique-Paths-shaped DAG with heavy path
// overlap). NaiveRecursion re-walks every shared sub-path from scratch per candidate
// start cell - here the call count itself, not just the returned value, grows with the
// number of increasing paths, which is why Size stays modest. MemoizedRecurrence
// dogfoods this repo's own Memoizer per start cell, collapsing shared sub-paths within
// one start's search from exponential to polynomial.
[MemoryDiagnoser]
public class NumberOfIncreasingPathsInAGridBenchmarks
{
    private int[,] _matrix = new int[0, 0];

    [Params(6, 9)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _matrix = new int[Size, Size];

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                _matrix[row, col] = row * Size + col;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => NumberOfIncreasingPathsInAGridSolution.CountPathsByNaiveRecursion(_matrix);

    [Benchmark]
    public int MemoizedRecurrence() =>
        NumberOfIncreasingPathsInAGridSolution.CountPathsByMemoizedRecurrence(_matrix);
}
