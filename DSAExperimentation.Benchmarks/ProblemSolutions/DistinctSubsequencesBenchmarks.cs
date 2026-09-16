using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DistinctSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is DistinctSubsequencesSolution's, the same method
// DistinctSubsequencesTests proves correct.
[MemoryDiagnoser]
public class DistinctSubsequencesBenchmarks
{
    private const string Source = "rabbbit";
    private const string Target = "rabbit";

    [Benchmark(Baseline = true)]
    public int MemoizedRecursion() =>
        DistinctSubsequencesSolution.CountDistinctSubsequencesByMemoizedRecursion(
            new SourceText(Source),
            new TargetPattern(Target));
}
