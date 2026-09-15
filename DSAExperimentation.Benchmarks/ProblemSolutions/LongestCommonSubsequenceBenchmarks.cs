using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestCommonSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCommonSubsequenceSolution's, the same methods
// LongestCommonSubsequenceTests proves correct. Tabulation is plain bottom-up 2D
// array filling against this repo's Memoizer running the same suffix-pair recurrence
// EditDistanceBenchmarks/MaximumLengthOfRepeatedSubarrayBenchmarks already use - both
// O(n*m), just walking the table from opposite directions. Both strings are
// identical, all-one-character strings so every cell of the table is genuinely
// reachable and does real work, the same "force the real worst case" intent
// TwoSumBenchmarks' own setup comment names.
[MemoryDiagnoser]
public class LongestCommonSubsequenceBenchmarks
{
    private string _first = "";

    private string _second = "";
    [Params(60, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _first = new string('a', Length);
        _second = new string('a', Length);
    }

    [Benchmark(Baseline = true)]
    public int Tabulation() => LongestCommonSubsequenceSolution.LengthByTabulation(_first, _second);

    [Benchmark]
    public int MemoizedRecurrence() =>
        LongestCommonSubsequenceSolution.LengthByMemoizedSuffixPairDp(_first, _second);
}
