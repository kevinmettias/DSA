using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.DesignAFoodRatingSystem;

// LeetCode 2353. Design a Food Rating System: changeRating(food, rating) updates a
// food's rating, highestRated(cuisine) reports that cuisine's highest-rated food,
// ties broken by the lexicographically smallest name.
//
// An instance API rather than a pure function, so "every strategy for the problem"
// (§17.3) takes the form of two classes implementing the shared
// IFoodRatingStrategy surface below rather than two static methods sharing an
// <Operation>By<Strategy> name - the same shape DesignTaskManagerSolution uses, and
// for the same reason: a Design problem is a sequence of mutating calls against one
// instance, so there is no prepared input to hoist into a benchmark's
// [GlobalSetup]. Each [Benchmark] arm constructs its own instance from LeetCode's
// own constructor arguments and replays the same call script instead.
internal static class DesignAFoodRatingSystemSolution
{
    // The shared surface both strategies implement, so a harness can replay one
    // call script against either without restating it.
    internal interface IFoodRatingStrategy
    {
        void ChangeRating(string food, int newRating);

        string HighestRated(string cuisine);
    }

    // The textbook answer: three parallel BCL arrays plus a Dictionary from food
    // name to its position, and a full scan of every food for the best-rated one in
    // the queried cuisine on every call - the arm the lazy-deletion heap below has
    // to justify itself against. Deliberately without this repo's primitives
    // (§17.5).
    internal sealed class FoodRatingsByLinearScan : IFoodRatingStrategy
    {
        private readonly string[] _foods;
        private readonly string[] _cuisines;
        private readonly int[] _ratings;
        private readonly Dictionary<string, int> _positionByFood = new();

        public FoodRatingsByLinearScan(string[] foods, string[] cuisines, int[] ratings)
        {
            _foods = [.. foods];
            _cuisines = [.. cuisines];
            _ratings = [.. ratings];

            for (var i = 0; i < _foods.Length; i++)
            {
                _positionByFood[_foods[i]] = i;
            }
        }

        public void ChangeRating(string food, int newRating) => _ratings[_positionByFood[food]] = newRating;

        public string HighestRated(string cuisine)
        {
            var best = -1;

            for (var i = 0; i < _foods.Length; i++)
            {
                if (_cuisines[i] != cuisine)
                {
                    continue;
                }

                if (best < 0 || IsHigherRanked(i, best))
                {
                    best = i;
                }
            }

            if (best < 0)
            {
                return string.Empty;
            }

            return _foods[best];
        }

        private bool IsHigherRanked(int candidate, int incumbent)
        {
            var candidateRating = _ratings[candidate];
            var incumbentRating = _ratings[incumbent];

            if (candidateRating != incumbentRating)
            {
                return candidateRating > incumbentRating;
            }

            return string.CompareOrdinal(_foods[candidate], _foods[incumbent]) < 0;
        }
    }

    // This repo's own primitives: every rating a food has ever carried is pushed as
    // a FoodEntry onto its cuisine's max Heap<FoodEntry, MaxHeapOrder<FoodEntry>>
    // (rating descending, then name ascending - FoodEntry's own comparison), and a
    // HashMap<food, rating> holds each food's current authoritative rating.
    // ChangeRating never removes the food's superseded heap entry - Heap has no
    // decrease-key or arbitrary remove (see Heap.cs) - so HighestRated lazily
    // discards entries from the top whose rating no longer matches the HashMap's
    // current record, the standard lazy-deletion trick for a priority queue without
    // one (the same shape DesignTaskManager's ExecTop uses).
    internal sealed class FoodRatingsByLazyDeletionHeap : IFoodRatingStrategy
    {
        private readonly HashMap<string, int> _ratingByFood = new();
        private readonly HashMap<string, string> _cuisineByFood = new();
        private readonly HashMap<string, Heap<FoodEntry, MaxHeapOrder<FoodEntry>>> _heapByCuisine = new();

        public FoodRatingsByLazyDeletionHeap(string[] foods, string[] cuisines, int[] ratings)
        {
            for (var i = 0; i < foods.Length; i++)
            {
                _ratingByFood.Set(foods[i], ratings[i]);
                _cuisineByFood.Set(foods[i], cuisines[i]);
                HeapForCuisine(cuisines[i]).Push(new FoodEntry(ratings[i], foods[i]));
            }
        }

        public void ChangeRating(string food, int newRating)
        {
            _ratingByFood.Set(food, newRating);
            _cuisineByFood.TryGetValue(food, out var cuisine);
            HeapForCuisine(cuisine).Push(new FoodEntry(newRating, food));
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

        private Heap<FoodEntry, MaxHeapOrder<FoodEntry>> HeapForCuisine(string cuisine)
        {
            if (!_heapByCuisine.TryGetValue(cuisine, out var heap))
            {
                heap = new Heap<FoodEntry, MaxHeapOrder<FoodEntry>>();
                _heapByCuisine.Set(cuisine, heap);
            }

            return heap;
        }
    }
}
