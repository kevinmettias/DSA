using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PermutationsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationsIISolution's, now returning the same
// permutations the test proves correct instead of merely counting them. Values are
// drawn from a seeded Random over five of LC 47's [-10, 10], so the larger size
// must repeat some by pigeonhole and both searches do the duplicate skipping the
// "II" variant exists for. Length stops at LC 47's 8-element cap.
public class PermutationsIIBenchmarks
{
    private const int RandomSeed = 47; // LC problem number
    private const int MinValue = -2;
    private const int MaxValue = 2;

    private int[] _nums = [];

    [Params(4, 8)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = SeededDraws.Values(Length, MinValue, MaxValue + 1, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public List<List<int>> SpecializedRecursive() =>
        PermutationsIISolution.PermuteUniqueBySpecializedRecursion(_nums);

    [Benchmark]
    public List<List<int>> Backtracking() =>
        PermutationsIISolution.PermuteUniqueByBacktracking(_nums);
}
