using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumStudentsTakingExam;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumStudentsTakingExamSolution's, the same methods
// MaximumStudentsTakingExamTests proves correct, each handed the SeatMasks its
// hoisted overload takes so mask construction is charged to [GlobalSetup] rather
// than to the recursion being measured.
//
// Every seat is left open (no broken seats), maximizing both the per-row branching
// factor and how often different row-by-row paths converge on the same PrevMask -
// the worst case for an unmemoized walk and the best case for memoization. RowCount
// stays modest, matching LongestIncreasingPathInAMatrixBenchmarks' "kept modest for
// exactly that reason", since the unmemoized side is exponential in RowCount.
[MemoryDiagnoser]
public class MaximumStudentsTakingExamBenchmarks
{
    private const int ColumnCount = 6;

    [Params(3, 5)]
    public int RowCount;

    private SeatMasks _seats = null!;

    [GlobalSetup]
    public void Setup() => _seats = new SeatMasks(new int[RowCount], (1 << ColumnCount) - 1);

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        MaximumStudentsTakingExamSolution.MaxStudentsByBruteForceRecursion(_seats);

    [Benchmark]
    public int MemoizedRecursion() =>
        MaximumStudentsTakingExamSolution.MaxStudentsByMemoizedBitmask(_seats);
}
