using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAFoodRatingSystem;

// LeetCode 2353. Design a Food Rating System: each cuisine's foods live in a max
// Heap<(int Rating, string Food),TOrder> ordered by rating descending, then food name
// ascending for LeetCode's tie-break rule - the same tie-break shape
// MinimumIntervalToIncludeEachQueryTests' BySizeOrder already demonstrates for a
// different field pair, just comparing a secondary field instead of stopping at one.
// ChangeRating never removes the food's old heap entry; a HashMap<string,int> tracks
// each food's *current* rating so HighestRated can lazily discard stale entries from
// the top instead of updating mid-heap.
public sealed partial class DesignAFoodRatingSystemTests
{
    [Fact]
    public void FoodRatings_LeetCodeExample_TracksHighestRatedAcrossRatingChanges()
    {
        string[] foods = ["kimchi", "miso", "sushi", "moussaka", "ramen", "bulgogi"];
        string[] cuisines = ["korean", "japanese", "japanese", "greek", "japanese", "korean"];
        int[] ratings = [9, 12, 8, 15, 14, 7];

        var foodRatings = new FoodRatings(foods, cuisines, ratings);

        Assert.Equal("kimchi", foodRatings.HighestRated("korean"));
        Assert.Equal("ramen", foodRatings.HighestRated("japanese"));

        foodRatings.ChangeRating("sushi", 16);
        Assert.Equal("sushi", foodRatings.HighestRated("japanese"));

        foodRatings.ChangeRating("ramen", 16);
        Assert.Equal("ramen", foodRatings.HighestRated("japanese"));
    }

    [Fact]
    public void HighestRated_TiedRatings_ReturnsLexicographicallySmallerName()
    {
        string[] foods = ["bravo", "alpha"];
        string[] cuisines = ["mex", "mex"];
        int[] ratings = [5, 5];

        var foodRatings = new FoodRatings(foods, cuisines, ratings);

        Assert.Equal("alpha", foodRatings.HighestRated("mex"));
    }

    private sealed class FoodRatings
    {
        private readonly HashMap<string, int> _ratingByFood = new();
        private readonly HashMap<string, string> _cuisineByFood = new();
        private readonly HashMap<string, Heap<(int Rating, string Food), ByRatingThenFoodOrder>> _heapByCuisine = new();

        public FoodRatings(string[] foods, string[] cuisines, int[] ratings)
        {
            for (var i = 0; i < foods.Length; i++)
            {
                _ratingByFood.Set(foods[i], ratings[i]);
                _cuisineByFood.Set(foods[i], cuisines[i]);
                HeapForCuisine(cuisines[i]).Push((ratings[i], foods[i]));
            }
        }

        public void ChangeRating(string food, int newRating)
        {
            _ratingByFood.Set(food, newRating);
            _cuisineByFood.TryGetValue(food, out var cuisine);
            HeapForCuisine(cuisine).Push((newRating, food));
        }

        public string HighestRated(string cuisine)
        {
            var heap = HeapForCuisine(cuisine);

            while (heap.TryPeek(out var top))
            {
                if (_ratingByFood.TryGetValue(top.Food, out var current) && current == top.Rating)
                {
                    return top.Food;
                }

                heap.TryPop(out _);
            }

            return string.Empty;
        }

        private Heap<(int Rating, string Food), ByRatingThenFoodOrder> HeapForCuisine(string cuisine)
        {
            if (!_heapByCuisine.TryGetValue(cuisine, out var heap))
            {
                heap = new Heap<(int Rating, string Food), ByRatingThenFoodOrder>();
                _heapByCuisine.Set(cuisine, heap);
            }

            return heap;
        }
    }

    private readonly struct ByRatingThenFoodOrder : IHeapOrder<(int Rating, string Food)>
    {
        public static bool HasPriority((int Rating, string Food) candidate, (int Rating, string Food) incumbent)
            => candidate.Rating != incumbent.Rating
                ? candidate.Rating > incumbent.Rating
                : string.CompareOrdinal(candidate.Food, incumbent.Food) < 0;
    }
}
