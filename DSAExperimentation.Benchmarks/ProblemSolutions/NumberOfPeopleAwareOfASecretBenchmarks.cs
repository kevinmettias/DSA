using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfPeopleAwareOfASecret;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfPeopleAwareOfASecretSolution's, the same
// methods NumberOfPeopleAwareOfASecretTests proves correct. [GlobalSetup] picks
// the delay/forget pair (workload sizing) so the sharing window stays a fixed
// fraction of n - which is what makes the sliding-window baseline effectively
// O(n^2) against the Fenwick arm's O(n log n). The inputs are three integers, so
// there is nothing to hoist and neither arm needs a prepared-input overload.
[MemoryDiagnoser]
public class NumberOfPeopleAwareOfASecretBenchmarks
{
    private const int DelayDivisor = 20;
    private const int ForgetDivisor = 2;

    private int _delay;

    private int _forget;
    [Params(2_000, 6_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _delay = Math.Max(1, Length / DelayDivisor);
        _forget = Math.Max(_delay + 1, Length / ForgetDivisor);
    }

    [Benchmark(Baseline = true)]
    public long SlidingWindowSum() =>
        NumberOfPeopleAwareOfASecretSolution.PeopleWithSecretBySlidingWindowSum(Length, _delay, _forget);

    [Benchmark]
    public long FenwickRangeSum() =>
        NumberOfPeopleAwareOfASecretSolution.PeopleWithSecretByFenwickRangeSum(Length, _delay, _forget);
}
