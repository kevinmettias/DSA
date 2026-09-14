namespace DSAExperimentation.LeetCode.DesignAFoodRatingSystem;

// Heap element for FoodRatingsByLazyDeletionHeap: max-heap ordering is rating
// first, then LeetCode's tie-break rule for highestRated - the lexicographically
// SMALLEST food name wins an equal rating, so the name comparison is deliberately
// reversed (other before this) to make the smaller name compare greater under
// MaxHeapOrder. IComparable<FoodEntry> is what DataStructures.Heap.MaxHeapOrder
// requires; a witness meaningful only to this problem's heap ordering, so it lives
// here rather than in Domain/ (§17.3).
internal readonly record struct FoodEntry(int Rating, string Food) : IComparable<FoodEntry>
{
    public int CompareTo(FoodEntry other)
    {
        var ratingCompare = Rating.CompareTo(other.Rating);

        return ratingCompare != 0 ? ratingCompare : string.CompareOrdinal(other.Food, Food);
    }
}
