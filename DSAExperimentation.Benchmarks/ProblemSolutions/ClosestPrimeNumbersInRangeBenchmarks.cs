using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ClosestPrimeNumbersInRange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClosestPrimeNumbersInRangeSolution's, the same
// methods ClosestPrimeNumbersInRangeTests proves correct. Trial-dividing every
// candidate in [2, right] up to sqrt(candidate) - O(right * sqrt(right)) - against
// the same Sieve of Eratosthenes CountPrimesBenchmarks runs over this repo's own
// DynamicArray<bool> composite tracker - O(right log log right). The sieve is built
// inside the measured method on purpose: paying for it once and then scanning is
// exactly what the comparison is about, so there is nothing to hoist into
// [GlobalSetup].
[MemoryDiagnoser]
public class ClosestPrimeNumbersInRangeBenchmarks
{
    private const int Left = 2;

    [Params(2_000, 20_000)]
    public int Right { get; set; }

    [Benchmark(Baseline = true)]
    public int[] TrialDivisionScan() =>
        ClosestPrimeNumbersInRangeSolution.ClosestPrimesByTrialDivision(Left, Right);

    [Benchmark]
    public int[] SieveScan() =>
        ClosestPrimeNumbersInRangeSolution.ClosestPrimesBySieve(Left, Right);
}
