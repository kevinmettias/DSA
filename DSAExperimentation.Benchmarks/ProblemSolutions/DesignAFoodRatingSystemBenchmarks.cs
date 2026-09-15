using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignAFoodRatingSystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAFoodRatingSystemSolution's, the same classes
// DesignAFoodRatingSystemTests proves correct. [GlobalSetup] builds the workload -
// the constructor's foods/cuisines/ratings, one superseding rating per food, and
// the query cuisines - so generating it is charged to setup rather than to the
// replay each arm measures.
//
// Every food's initial rating is immediately superseded by one ChangeRating, so
// both strategies answer every query off the changed value. LinearScan rescans all
// `Count` foods for the best-rated match in the queried cuisine on every call;
// LazyDeletionHeap only ever pops the entries a rating change superseded, amortized
// O(log n) per call.
[MemoryDiagnoser]
public class DesignAFoodRatingSystemBenchmarks
{
    private const int RandomSeed = 2353;
    private const int CuisineDomain = 15;
    private const int MaxRatingExclusive = 100;

    private string[] _foods = [];

    private string[] _cuisines = [];
    private int[] _initialRatings = [];
    private int[] _changeRatings = [];
    private string[] _queryCuisines = [];
    [Params(200, 3_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _foods = Enumerable.Range(0, Count).Select(i => $"food{i}").ToArray();
        _cuisines = Enumerable.Range(0, Count).Select(_ => $"cuisine{random.Next(0, CuisineDomain)}").ToArray();
        _initialRatings = Enumerable.Range(0, Count).Select(_ => random.Next(0, MaxRatingExclusive)).ToArray();
        _changeRatings = Enumerable.Range(0, Count).Select(_ => random.Next(0, MaxRatingExclusive)).ToArray();
        _queryCuisines = Enumerable.Range(0, Count).Select(_ => $"cuisine{random.Next(0, CuisineDomain)}").ToArray();
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new DesignAFoodRatingSystemSolution.FoodRatingsByLinearScan(_foods, _cuisines, _initialRatings));

    [Benchmark]
    public long LazyDeletionHeap() => Replay(new DesignAFoodRatingSystemSolution.FoodRatingsByLazyDeletionHeap(_foods, _cuisines, _initialRatings));

    // Sums the length of every reported food name rather than discarding the
    // answer, so the JIT can't eliminate the replay as dead code.
    private long Replay(DesignAFoodRatingSystemSolution.IFoodRatingStrategy strategy)
    {
        for (var i = 0; i < _foods.Length; i++)
        {
            strategy.ChangeRating(_foods[i], _changeRatings[i]);
        }

        var reportedNameLengthSum = 0L;

        foreach (var cuisine in _queryCuisines)
        {
            reportedNameLengthSum += strategy.HighestRated(cuisine).Length;
        }

        return reportedNameLengthSum;
    }
}
