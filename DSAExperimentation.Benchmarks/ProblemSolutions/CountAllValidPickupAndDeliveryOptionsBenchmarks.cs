using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountAllValidPickupAndDeliveryOptions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountAllValidPickupAndDeliveryOptionsSolution's, the
// same strategies CountAllValidPickupAndDeliveryOptionsTests proves correct.
[MemoryDiagnoser]
public class CountAllValidPickupAndDeliveryOptionsBenchmarks
{
    [Params(100, 10_000)]
    public int Orders;

    [Benchmark(Baseline = true)]
    public long Tabulation() =>
        CountAllValidPickupAndDeliveryOptionsSolution.CountOrdersByTabulation(Orders);

    [Benchmark]
    public long Memoized() =>
        CountAllValidPickupAndDeliveryOptionsSolution.CountOrdersByMemoizedRecurrence(Orders);
}
