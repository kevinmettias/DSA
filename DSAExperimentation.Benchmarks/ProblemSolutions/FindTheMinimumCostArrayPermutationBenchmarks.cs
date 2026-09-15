using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheMinimumCostArrayPermutation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheMinimumCostArrayPermutationSolution's, the
// same methods FindTheMinimumCostArrayPermutationTests proves correct. Neither
// strategy needs input construction beyond the permutation itself, so [GlobalSetup]
// only builds a random permutation of the given size - workload sizing that fixes
// no domain content, the same inline-randomized shape AddTwoNumbersBenchmarks uses
// for its own random digit list. Kept small: the brute-force arm is O(n!), so N is
// bounded well under LC's own n <= 14 to keep every configuration's baseline run
// finishing in reasonable benchmark time.
[MemoryDiagnoser]
public class FindTheMinimumCostArrayPermutationBenchmarks
{
    private const int Seed = 3149;

    private int[] _nums = [];

    [Params(6, 8)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var nums = Enumerable.Range(0, N).ToArray();

        for (var i = nums.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (nums[i], nums[j]) = (nums[j], nums[i]);
        }

        _nums = nums;
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceSearch() =>
        FindTheMinimumCostArrayPermutationSolution.FindPermutationByBruteForceSearch(_nums);

    [Benchmark]
    public int[] BitmaskMemoization() =>
        FindTheMinimumCostArrayPermutationSolution.FindPermutationByBitmaskMemoization(_nums);
}
