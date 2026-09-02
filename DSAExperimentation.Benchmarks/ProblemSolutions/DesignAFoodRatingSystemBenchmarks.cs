using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design a Food Rating System (LC 2353): both strategies replay the same
// ChangeRating-then-HighestRated workload (every food's initial rating is immediately
// superseded by one change, so both strategies serve every query off the changed value).
// LinearScanHighestRated tracks each food's current rating in a plain array and answers
// a query with a fresh O(n) scan for the best-rated match in that cuisine.
// HeapPerCuisineHighestRated instead composes this repo's own HashMap<TKey,TValue> and a
// max Heap<(int,string),TOrder> per cuisine (ordered by rating, then food name for ties)
// so a query only ever pops entries that have since been superseded by a rating change,
// amortized O(log n) per call.
[MemoryDiagnoser]
public class DesignAFoodRatingSystemBenchmarks
{
    private const int RandomSeed = 2353;
    private const int CuisineDomain = 15;
    private const int MaxRatingExclusive = 100;

    [Params(200, 3_000)]
    public int Count;

    private string[] _foods = null!;
    private string[] _cuisines = null!;
    private int[] _initialRatings = null!;
    private int[] _changeRatings = null!;
    private string[] _queryCuisines = null!;

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
    public int LinearScanHighestRated()
    {
        var ratings = _changeRatings.ToArray();

        var total = 0;
        foreach (var cuisine in _queryCuisines)
        {
            total += ScanForBestRating(ratings, cuisine);
        }

        return total;
    }

    private int ScanForBestRating(int[] ratings, string cuisine)
    {
        var bestIndex = -1;

        for (var i = 0; i < _foods.Length; i++)
        {
            if (_cuisines[i] != cuisine)
            {
                continue;
            }

            if (bestIndex == -1
                || ratings[i] > ratings[bestIndex]
                || (ratings[i] == ratings[bestIndex] && string.CompareOrdinal(_foods[i], _foods[bestIndex]) < 0))
            {
                bestIndex = i;
            }
        }

        return bestIndex == -1 ? 0 : ratings[bestIndex];
    }

    [Benchmark]
    public int HeapPerCuisineHighestRated()
    {
        var ratingByFood = new HashMap<string, int>();
        var heapByCuisine = new HashMap<string, Heap<(int Rating, string Food), ByRatingThenFoodOrder>>();

        for (var i = 0; i < _foods.Length; i++)
        {
            ratingByFood.Set(_foods[i], _initialRatings[i]);
            HeapForCuisine(heapByCuisine, _cuisines[i]).Push((_initialRatings[i], _foods[i]));
        }

        for (var i = 0; i < _foods.Length; i++)
        {
            ratingByFood.Set(_foods[i], _changeRatings[i]);
            HeapForCuisine(heapByCuisine, _cuisines[i]).Push((_changeRatings[i], _foods[i]));
        }

        var total = 0;
        foreach (var cuisine in _queryCuisines)
        {
            total += HighestRated(ratingByFood, heapByCuisine, cuisine);
        }

        return total;
    }

    private static Heap<(int Rating, string Food), ByRatingThenFoodOrder> HeapForCuisine(
        HashMap<string, Heap<(int Rating, string Food), ByRatingThenFoodOrder>> heapByCuisine, string cuisine)
    {
        if (!heapByCuisine.TryGetValue(cuisine, out var heap))
        {
            heap = new Heap<(int Rating, string Food), ByRatingThenFoodOrder>();
            heapByCuisine.Set(cuisine, heap);
        }

        return heap;
    }

    private static int HighestRated(
        HashMap<string, int> ratingByFood,
        HashMap<string, Heap<(int Rating, string Food), ByRatingThenFoodOrder>> heapByCuisine,
        string cuisine)
    {
        var heap = HeapForCuisine(heapByCuisine, cuisine);

        while (heap.TryPeek(out var top))
        {
            if (ratingByFood.TryGetValue(top.Food, out var current) && current == top.Rating)
            {
                return top.Rating;
            }

            heap.TryPop(out _);
        }

        return 0;
    }

    private readonly struct ByRatingThenFoodOrder : IHeapOrder<(int Rating, string Food)>
    {
        public static bool HasPriority((int Rating, string Food) candidate, (int Rating, string Food) incumbent)
            => candidate.Rating != incumbent.Rating
                ? candidate.Rating > incumbent.Rating
                : string.CompareOrdinal(candidate.Food, incumbent.Food) < 0;
    }
}
