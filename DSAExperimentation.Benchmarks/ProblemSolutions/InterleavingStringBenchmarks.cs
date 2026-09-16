using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is InterleavingStringSolution's, the same method
// InterleavingStringTests proves correct. The previous class was a
// compile-smoke placeholder (`=> 1` on both arms) that measured nothing; this
// measures the actual memoized recursion against LeetCode's own example.
[MemoryDiagnoser]
public class InterleavingStringBenchmarks
{
    private const string First = "aabcc";
    private const string Second = "dbbca";
    private const string Target = "aadbbcbcac";

    [Benchmark]
    public bool IsInterleaveByMemoizedRecursion() =>
        InterleavingStringSolution.IsInterleaveByMemoizedRecursion(
            First,
            Second,
            new InterleavingStringSolution.TargetText(Target));
}
