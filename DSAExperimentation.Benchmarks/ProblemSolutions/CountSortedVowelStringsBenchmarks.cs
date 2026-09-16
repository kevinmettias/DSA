using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSortedVowelStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSortedVowelStringsSolution's, the same methods
// CountSortedVowelStringsTests proves correct. The string length is pushed past
// LeetCode's own worked example (33) specifically so MemoryDiagnoser shows a real
// allocation gap - the baseline allocates one string per answer, the memoized
// recurrence allocates one cache entry per state - and not just a wall-clock one.
[MemoryDiagnoser]
public class CountSortedVowelStringsBenchmarks
{
    [Params(20, 35)]
    public int StringLength { get; set; }

    [Benchmark(Baseline = true)]
    public int BacktrackingEnumeration() =>
        CountSortedVowelStringsSolution.CountVowelStringsByBacktrackingEnumeration(StringLength);

    [Benchmark]
    public int MemoizedRecurrence() =>
        CountSortedVowelStringsSolution.CountVowelStringsByMemoizedRecurrence(StringLength);
}
