using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DesignAFoodRatingSystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAFoodRatingSystemSolution's, the same classes
// DesignAFoodRatingSystemSolutionTests proves correct. [GlobalSetup] builds the workload -
// the constructor's foods/cuisines/ratings, one superseding rating per food, and
// the query cuisines - so generating it is charged to setup rather than to the
// replay each arm measures.
//
// Every food's initial rating is immediately superseded by one ChangeRating, so
// both strategies answer every query off the changed value. LinearScan rescans all
// `Count` foods for the best-rated match in the queried cuisine on every call;
// LazyDeletionHeap only ever pops the entries a rating change superseded, amortized
// O(log n) per call. Food and cuisine names are LowercaseNames after a fixed prefix,
// since LC 2353 spells them in lowercase letters, and ratings start at its floor of 1.
public class DesignAFoodRatingSystemBenchmarks
{
    private const int RandomSeed = 2353;
    private const int CuisineDomain = 15;
    private const int MaxRatingExclusive = 100;

    private const string FoodPrefix = "food";

    private const string CuisinePrefix = "cuisine";

    private string[] _foods = [];

    private string[] _cuisines = [];
    private int[] _initialRatings = [];
    private int[] _changeRatings = [];
    private string[] _queryCuisines = [];

    // Every name HighestRated reports, in query order; sized in setup so the replay allocates nothing.
    private string[] _reported = [];
    [Params(200, 3_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _foods = Enumerable.Range(0, Count).Select(i => FoodPrefix + LowercaseNames.Of(i)).ToArray();
        _cuisines = Enumerable.Range(0, Count).Select(_ => DrawCuisine(random)).ToArray();
        _initialRatings = SeededDraws.Values(Count, 1, MaxRatingExclusive, random);
        _changeRatings = SeededDraws.Values(Count, 1, MaxRatingExclusive, random);
        _queryCuisines = Enumerable.Range(0, Count).Select(_ => DrawCuisine(random)).ToArray();
        _reported = new string[_queryCuisines.Length];
    }

    [Benchmark(Baseline = true)]
    public string[] LinearScan() => Replay(new DesignAFoodRatingSystemSolution.FoodRatingsByLinearScan(_foods, _cuisines, _initialRatings));

    [Benchmark]
    public string[] LazyDeletionHeap() => Replay(new DesignAFoodRatingSystemSolution.FoodRatingsByLazyDeletionHeap(_foods, _cuisines, _initialRatings));

    private static string DrawCuisine(Random random)
    {
        var cuisine = random.Next(0, CuisineDomain);

        return CuisinePrefix + LowercaseNames.Of(cuisine);
    }

    // Returns every reported food name, in query order, so the JIT can't eliminate
    // the replay as dead code.
    private string[] Replay(DesignAFoodRatingSystemSolution.IFoodRatingStrategy strategy)
    {
        for (var i = 0; i < _foods.Length; i++)
        {
            strategy.ChangeRating(_foods[i], _changeRatings[i]);
        }

        for (var i = 0; i < _queryCuisines.Length; i++)
        {
            _reported[i] = strategy.HighestRated(_queryCuisines[i]);
        }

        return _reported;
    }
}
