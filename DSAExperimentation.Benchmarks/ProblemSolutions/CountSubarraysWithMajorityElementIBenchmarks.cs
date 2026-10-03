using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubarraysWithMajorityElementISolution's, the
// same methods CountSubarraysWithMajorityElementISolutionTests proves correct.
//
// A small 5-value alphabet keeps target frequent enough that a meaningful share
// of subarrays qualify as majority, instead of degenerating to "almost none do".
// Length reaches LC's own 1000 upper bound - the O(n^2) brute force is still fast
// enough to finish there, which is exactly what distinguishes LC 3737 from its
// LC 3739 sequel.
public class CountSubarraysWithMajorityElementIBenchmarks
{
    private const int RandomSeed = 3737; // LC problem number
    private const int AlphabetSize = 5;
    private const int Target = 1;

    private int[] _nums = [];

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, AlphabetSize + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => CountSubarraysWithMajorityElementISolution.CountByBruteForce(_nums, Target);

    [Benchmark]
    public int FenwickPrefixSum() => CountSubarraysWithMajorityElementISolution.CountByFenwickPrefixSum(_nums, Target);
}
