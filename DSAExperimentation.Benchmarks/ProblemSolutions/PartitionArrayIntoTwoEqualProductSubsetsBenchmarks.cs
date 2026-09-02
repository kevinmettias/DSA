using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PartitionArrayIntoTwoEqualProductSubsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionArrayIntoTwoEqualProductSubsetsSolution's,
// the same methods PartitionArrayIntoTwoEqualProductSubsetsTests proves correct.
//
// Target sits well below the workload's full product rather than at either
// extreme: too small and every branch is pruned after its very first element
// (both arms finish instantly, showing nothing); at or above the full product and
// nothing is ever pruned (both arms degrade to the same full 2^n walk). This size
// lets CandidateAssignments cut real branches while still forcing a near-complete
// search, since these distinct random values essentially never land on an exact
// equal-product split.
[MemoryDiagnoser]
public class PartitionArrayIntoTwoEqualProductSubsetsBenchmarks
{
    private const int Seed = 3566;
    private const long Target = 500_000_000_000L;

    [Params(12, 18)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var values = new HashSet<int>();

        while (values.Count < Length)
        {
            values.Add(random.Next(2, 101));
        }

        _nums = values.ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BitmaskEnumeration() =>
        PartitionArrayIntoTwoEqualProductSubsetsSolution.CheckEqualPartitionsByBitmaskEnumeration(_nums, Target);

    [Benchmark]
    public bool PrunedBacktracking() =>
        PartitionArrayIntoTwoEqualProductSubsetsSolution.CheckEqualPartitionsByPrunedBacktracking(_nums, Target);
}
