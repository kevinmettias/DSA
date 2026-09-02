using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfDigitOne;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfDigitOneSolution's, the same methods
// NumberOfDigitOneTests proves correct. BruteForceScan materializes and
// mod/div-scans every integer 1..N (O(n log n)); DigitPositionTally is this
// repo's own Stack<int> digit-extraction feeding a place-value tally that
// visits only N's own decimal digits (O(log n)).
[MemoryDiagnoser]
public class NumberOfDigitOneBenchmarks
{
    [Params(20_000, 500_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public long BruteForceScan() => NumberOfDigitOneSolution.CountDigitOneByBruteForceScan(N);

    [Benchmark]
    public long DigitPositionTally() => NumberOfDigitOneSolution.CountDigitOneByDigitPositionTally(N);
}
