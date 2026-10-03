using DSAExperimentation.LeetCode.CountAllValidPickupAndDeliveryOptions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountAllValidPickupAndDeliveryOptionsSolution's, the
// same strategies CountAllValidPickupAndDeliveryOptionsSolutionTests proves correct.
public class CountAllValidPickupAndDeliveryOptionsBenchmarks
{
    // LC 1359 asks about at most 500 orders.
    [Params(100, 500)]
    public int Orders { get; set; }

    [Benchmark(Baseline = true)]
    public long Tabulation() =>
        CountAllValidPickupAndDeliveryOptionsSolution.CountOrdersByTabulation(Orders);

    [Benchmark]
    public long Memoized() =>
        CountAllValidPickupAndDeliveryOptionsSolution.CountOrdersByMemoizedRecurrence(Orders);
}
