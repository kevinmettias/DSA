using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumBalancedShipments;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumBalancedShipmentsSolution's, the same methods
// MaximumBalancedShipmentsTests proves correct. Neither strategy has a separable
// construction step - weight itself is the whole input - so there is nothing to
// hoist into [GlobalSetup] beyond building the array.
[MemoryDiagnoser]
public class MaximumBalancedShipmentsBenchmarks
{
    // LC problem number, reused as the deterministic weight seed.
    private const int WeightSeed = 3638;

    [Params(200, 2000)]
    public int ParcelCount;

    private int[] _weight = null!;

    [GlobalSetup]
    public void Setup() => _weight = MaximumBalancedShipmentsWorkloads.BuildWeights(ParcelCount, seed: WeightSeed);

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumBalancedShipmentsSolution.MaxBalancedShipmentsByBruteForce(_weight);

    [Benchmark]
    public int PreviousGreaterStack() =>
        MaximumBalancedShipmentsSolution.MaxBalancedShipmentsByPreviousGreaterStack(_weight);
}
