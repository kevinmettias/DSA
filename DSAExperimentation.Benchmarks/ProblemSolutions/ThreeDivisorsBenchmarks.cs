using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ThreeDivisors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ThreeDivisorsSolution's, the same methods
// ThreeDivisorsSolutionTests proves correct - the textbook O(num) full-range trial division
// per number against BinarySearch.LowerBound anchoring each number's scan at
// floor(sqrt(num)), then a short downward walk that bails out the moment a fourth
// divisor appears. The random numbers are built once in [GlobalSetup], so generation
// is not charged to either arm; each arm returns every number's answer, in input order.
// Each number is drawn from LC 1952's [1, 10^4].
public class ThreeDivisorsBenchmarks
{
    // LC problem number, reused as the deterministic workload seed.
    private const int RandomSeed = 1952;
    private const int MaxN = 10_000;

    private int[] _nums = [];

    private bool[] _isThree = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, MaxN + 1, random);
        _isThree = new bool[_nums.Length];
    }

    [Benchmark(Baseline = true)]
    public bool[] FullRangeScan()
    {
        for (var i = 0; i < _nums.Length; i++)
        {
            _isThree[i] = ThreeDivisorsSolution.IsThreeByFullRangeScan(_nums[i]);
        }

        return _isThree;
    }

    [Benchmark]
    public bool[] BinarySearchAnchored()
    {
        for (var i = 0; i < _nums.Length; i++)
        {
            _isThree[i] = ThreeDivisorsSolution.IsThreeByBinarySearchAnchor(_nums[i]);
        }

        return _isThree;
    }
}
