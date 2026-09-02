using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PartitionArrayForMaximumXorAndAnd;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionArrayForMaximumXorAndAndSolution's, the same
// methods PartitionArrayForMaximumXorAndAndTests proves correct. Neither strategy
// has a separable construction step - nums itself is the whole input - so there is
// nothing to hoist into [GlobalSetup] beyond building the array. ElementCount stays
// small: the brute-force arm is 3^n, and 3^14 already exercises the exponential
// blowup the subset-basis arm exists to avoid without making the benchmark itself
// impractically slow.
[MemoryDiagnoser]
public class PartitionArrayForMaximumXorAndAndBenchmarks
{
    // LC problem number, reused as the deterministic value seed.
    private const int ValueSeed = 3630;

    [Params(8, 14)]
    public int ElementCount;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = PartitionArrayForMaximumXorAndAndWorkloads.BuildNums(ElementCount, seed: ValueSeed);

    [Benchmark(Baseline = true)]
    public long BruteForce() => PartitionArrayForMaximumXorAndAndSolution.MaxPartitionValueByBruteForce(_nums);

    [Benchmark]
    public long SubsetXorBasis() => PartitionArrayForMaximumXorAndAndSolution.MaxPartitionValueBySubsetXorBasis(_nums);
}
