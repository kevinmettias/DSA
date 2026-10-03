using DSAExperimentation.LeetCode.DistinctSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistinctSubsequencesSolution's, the same methods
// DistinctSubsequencesTests proves correct.
public class DistinctSubsequencesBenchmarks
{
    private const string Source = "rabbbit";
    private const string Target = "rabbit";

    [Benchmark(Baseline = true)]
    public int MemoizedRecursion() =>
        DistinctSubsequencesSolution.CountDistinctSubsequencesByMemoizedRecursion(
            new SourceText(Source),
            new TargetPattern(Target));

    [Benchmark]
    public int IterativeTable() =>
        DistinctSubsequencesSolution.CountDistinctSubsequencesByIterativeTable(
            new SourceText(Source),
            new TargetPattern(Target));
}
