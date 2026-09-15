using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeGridHappiness;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeGridHappinessSolution's, the same methods
// MaximizeGridHappinessTests proves correct - un-memoized profile recursion over
// (Pos, Mask, Introverts, Extroverts) against the identical recursion routed through
// this repo's own Memoizer (MaximumStudentsTakingExamBenchmarks' precedent for this
// exact shape). Each arm is handed the GridLayout its hoisted overload takes, so the
// profile-window arithmetic is charged to [GlobalSetup] rather than to the recursion
// being measured. Columns is fixed and Rows is kept modest (matching
// CherryPickupBenchmarks' "kept modest" reasoning) since the un-memoized side is
// exponential in cell count.
[MemoryDiagnoser]
public class MaximizeGridHappinessBenchmarks
{
    private const int Columns = 3;
    private const int IntrovertsCount = 3;
    private const int ExtrovertsCount = 2;

    private GridLayout _layout = null!;

    [Params(3, 4)]
    public int Rows { get; set; }

    [GlobalSetup]
    public void Setup() => _layout = GridLayout.Build(Rows, Columns);

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        MaximizeGridHappinessSolution.GetMaxGridHappinessByBruteForceRecursion(
            _layout, IntrovertsCount, ExtrovertsCount);

    [Benchmark]
    public int MemoizedRecursion() =>
        MaximizeGridHappinessSolution.GetMaxGridHappinessByMemoizedProfileDp(
            _layout, IntrovertsCount, ExtrovertsCount);
}
