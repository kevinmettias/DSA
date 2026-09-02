using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GuessNumberHigherOrLower;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GuessNumberHigherOrLowerSolution's, the same methods
// GuessNumberHigherOrLowerTests proves correct. Pick sits at 70% of NumberCount so the
// linear scan pays close to its full O(n) worst case every call.
[MemoryDiagnoser]
public class GuessNumberHigherOrLowerBenchmarks
{
    private const double PickFraction = 0.7;

    [Params(1_000, 1_000_000)]
    public int NumberCount;

    private int _pick;

    [GlobalSetup]
    public void Setup() => _pick = (int)(NumberCount * PickFraction);

    [Benchmark(Baseline = true)]
    public int LinearScan() => GuessNumberHigherOrLowerSolution.GuessNumberByLinearScan(NumberCount, _pick);

    [Benchmark]
    public int BinarySearch() =>
        GuessNumberHigherOrLowerSolution.GuessNumberByBinarySearch(NumberCount, _pick);
}
