using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MakeArrayNonDecreasing;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MakeArrayNonDecreasingSolution's, the same methods
// MakeArrayNonDecreasingTests proves correct. Sized well below LeetCode's own
// 2*10^5 bound: the DP baseline is real recursion (Memoizer, not an explicit stack),
// so its call depth tracks n directly, and its O(n^2) trial of every split point
// would dominate the benchmark at LeetCode's own scale.
[MemoryDiagnoser]
public class MakeArrayNonDecreasingBenchmarks
{
    private const int RandomSeed = 3523; // LeetCode problem number
    private const int ValueUpperBound = 1_000;

    [Params(100, 1_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _nums[i] = random.Next(1, ValueUpperBound);
        }
    }

    [Benchmark(Baseline = true)]
    public int PrefixDynamicProgramming() => MakeArrayNonDecreasingSolution.MaxSizeByPrefixDynamicProgramming(_nums);

    [Benchmark]
    public int GreedyScan() => MakeArrayNonDecreasingSolution.MaxSizeByGreedyScan(_nums);
}
