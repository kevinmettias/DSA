using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.IntegerToEnglishWords;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IntegerToEnglishWordsSolution's, the same methods
// IntegerToEnglishWordsTests proves correct. Params span a single-group number
// (no prepend ever happens) up to Int32.MaxValue (all four groups), so the
// stack strategy's saved reallocations actually have groups to save on.
[MemoryDiagnoser]
public class IntegerToEnglishWordsBenchmarks
{
    [Params(123, 2_147_483_647)]
    public int Number;

    [Benchmark(Baseline = true)]
    public string StringPrepend() => IntegerToEnglishWordsSolution.NumberToWordsByStringPrepend(Number);

    [Benchmark]
    public string WordStack() => IntegerToEnglishWordsSolution.NumberToWordsByWordStack(Number);
}
