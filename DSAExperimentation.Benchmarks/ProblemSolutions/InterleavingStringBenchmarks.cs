using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InterleavingStringSolution's, the same methods
// InterleavingStringTests proves correct. The previous class was a
// compile-smoke placeholder (`=> 1` on both arms) that measured nothing; this
// measures the memoized recursion against the roll-forward row, both over
// LeetCode's own example.
[MemoryDiagnoser]
public class InterleavingStringBenchmarks
{
    private const string First = "aabcc";
    private const string Second = "dbbca";
    private const string Target = "aadbbcbcac";

    [Benchmark(Baseline = true)]
    public bool IsInterleaveByMemoizedRecursion() =>
        InterleavingStringSolution.IsInterleaveByMemoizedRecursion(
            First,
            Second,
            new InterleavingStringSolution.TargetText(Target));

    [Benchmark]
    public bool IterativeTable() =>
        InterleavingStringSolution.IsInterleaveByIterativeTable(
            First,
            Second,
            new InterleavingStringSolution.TargetText(Target));
}
