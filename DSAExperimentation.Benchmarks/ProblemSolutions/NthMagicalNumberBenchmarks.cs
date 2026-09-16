using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NthMagicalNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NthMagicalNumberSolution's, the same methods
// NthMagicalNumberTests proves correct. firstFactor = 6 and secondFactor = 10 share a
// factor, so the inclusion-exclusion term does real work instead of collapsing to
// zero, and counting candidates one at a time (O(answer)) is measured against
// BinarySearch.LowerBound over the monotone "count(x) >= rank" sequence
// (O(log(answer))).
[MemoryDiagnoser]
public class NthMagicalNumberBenchmarks
{
    private const int FirstFactor = 6;
    private const int SecondFactor = 10;

    [Params(2_000, 50_000)]
    public int Rank { get; set; }

    [Benchmark(Baseline = true)]
    public int BruteForceCount() =>
        NthMagicalNumberSolution.NthMagicalNumberByCountScan(Rank, FirstFactor, SecondFactor);

    [Benchmark]
    public int BinarySearchOnCount() =>
        NthMagicalNumberSolution.NthMagicalNumberByBinarySearch(Rank, FirstFactor, SecondFactor);
}
