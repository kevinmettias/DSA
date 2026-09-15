using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumTotalBeautyOfTheGardens;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumTotalBeautyOfTheGardensSolution's, the same
// methods MaximumTotalBeautyOfTheGardensTests proves correct. They enumerate the
// same n+1 "complete vs. incomplete" splits over the sorted array but differ in how
// they answer "how many of the incomplete prefix are below height h" and "what is
// the tallest affordable h" - a linear scan of the prefix and a linear walk down
// from the cap (no repo primitive) vs. this repo's own MergeSort and
// BinarySearch.LowerBound. Length drives both the number of splits and, since
// Target scales with Length too, the per-split scan cost, so the O(n^2 * target) vs.
// O(n log n * log target) gap widens with it instead of staying flat at a fixed
// target. Sorting stays inside each measured method on purpose: which sort each arm
// uses is part of what is being compared.
[MemoryDiagnoser]
public class MaximumTotalBeautyOfTheGardensBenchmarks
{
    private const int Full = 50;
    private const int Partial = 10;
    private const int FlowerHeightRangeMultiplier = 2;
    private const int RandomSeed = 1;

    private int _target;

    private int[] _flowers = [];
    private long _newFlowers;
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _target = Length;
        _flowers = Enumerable.Range(0, Length).Select(_ => random.Next(1, (_target * FlowerHeightRangeMultiplier) + 1)).ToArray();

        // Deliberately scarce, not generous: a budget on the order of target alone
        // (rather than enough to raise a whole large prefix near the cap) forces the
        // achievable height down near 0 for most feasible splits, so
        // LinearSearchOnAnswer's height descent actually walks most of [0, target)
        // instead of succeeding on its very first probe at the cap.
        _newFlowers = _target;
    }

    [Benchmark(Baseline = true)]
    public long LinearSearchOnAnswer() =>
        MaximumTotalBeautyOfTheGardensSolution.MaximumBeautyByLinearSearchOnAnswer(
            _flowers, _newFlowers, _target, new BeautyWeights(Full, Partial));

    [Benchmark]
    public long SortAndBinarySearch() =>
        MaximumTotalBeautyOfTheGardensSolution.MaximumBeautyBySortAndBinarySearch(
            _flowers, _newFlowers, _target, new BeautyWeights(Full, Partial));
}
