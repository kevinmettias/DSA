using static DSAExperimentation.LeetCode.DesignAFoodRatingSystem.DesignAFoodRatingSystemSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAFoodRatingSystem;

// Harness only. Both strategies are DesignAFoodRatingSystemSolution's - this file
// replays LeetCode's published call sequence against each IFoodRatingStrategy
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a constructor plus a
// sequence of mutating calls rather than a single argument tuple. FoodRatingOp.Apply
// is pure dispatch (which method to call with which arguments) - no rating or
// tie-break logic of its own.
public sealed class DesignAFoodRatingSystemTests
{
    public static TheoryData<string[], string[], int[], FoodRatingOp[], string?[]> Examples =>
        new()
        {
            // LeetCode's published example, including the change that makes ramen
            // and sushi tie at 16 so the lexicographically smaller name wins.
            {
                ["kimchi", "miso", "sushi", "moussaka", "ramen", "bulgogi"],
                ["korean", "japanese", "japanese", "greek", "japanese", "korean"],
                [9, 12, 8, 15, 14, 7],
                [
                    FoodRatingOp.HighestRated("korean"),
                    FoodRatingOp.HighestRated("japanese"),
                    FoodRatingOp.ChangeRating("sushi", 16),
                    FoodRatingOp.HighestRated("japanese"),
                    FoodRatingOp.ChangeRating("ramen", 16),
                    FoodRatingOp.HighestRated("japanese"),
                ],
                ["kimchi", "ramen", null, "sushi", null, "ramen"]
            },

            // Tied from the start: the smaller name wins without any rating change.
            {
                ["bravo", "alpha"],
                ["mex", "mex"],
                [5, 5],
                [FoodRatingOp.HighestRated("mex")],
                ["alpha"]
            },

            // A rating lowered rather than raised, so the leader changes and the
            // superseded entry has to be discarded; then restored into a tie, where
            // the smaller name takes the lead back.
            {
                ["alpha", "bravo"],
                ["thai", "thai"],
                [9, 5],
                [
                    FoodRatingOp.HighestRated("thai"),
                    FoodRatingOp.ChangeRating("alpha", 1),
                    FoodRatingOp.HighestRated("thai"),
                    FoodRatingOp.ChangeRating("alpha", 5),
                    FoodRatingOp.HighestRated("thai"),
                ],
                ["alpha", null, "bravo", null, "alpha"]
            },

            // Cuisines do not interfere: a change in one leaves the other's answer
            // alone, and a single-food cuisine always reports that food.
            {
                ["udon", "pho", "laksa"],
                ["japanese", "viet", "malay"],
                [3, 4, 5],
                [
                    FoodRatingOp.ChangeRating("udon", 100),
                    FoodRatingOp.HighestRated("viet"),
                    FoodRatingOp.HighestRated("malay"),
                    FoodRatingOp.HighestRated("japanese"),
                ],
                [null, "pho", "laksa", "udon"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FoodRatingsByLinearScan_LeetCodeExamples_ReturnsHighestRatedFoodPerCuisine(
        string[] foods, string[] cuisines, int[] ratings, FoodRatingOp[] operations, string?[] expected) =>
        RunScript(new FoodRatingsByLinearScan(foods, cuisines, ratings), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void FoodRatingsByLazyDeletionHeap_LeetCodeExamples_ReturnsHighestRatedFoodPerCuisine(
        string[] foods, string[] cuisines, int[] ratings, FoodRatingOp[] operations, string?[] expected) =>
        RunScript(new FoodRatingsByLazyDeletionHeap(foods, cuisines, ratings), operations, expected);

    private static void RunScript(IFoodRatingStrategy strategy, FoodRatingOp[] operations, string?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}

// One call in a FoodRatings script: which method to invoke and with what arguments.
// Pure dispatch, built via the named factories below so a script (like Examples
// above) reads like the LeetCode call sequence it replays.
public readonly record struct FoodRatingOp
{
    private readonly bool _isQuery;
    private readonly string _subject;
    private readonly int _rating;

    private FoodRatingOp(bool isQuery, string subject, int rating)
    {
        _isQuery = isQuery;
        _subject = subject;
        _rating = rating;
    }

    public static FoodRatingOp ChangeRating(string food, int newRating) => new(isQuery: false, food, newRating);

    public static FoodRatingOp HighestRated(string cuisine) => new(isQuery: true, cuisine, rating: 0);

    // null for the void ChangeRating call, the reported food name for
    // HighestRated - so a script runner can assert against one expected value per
    // operation uniformly. Internal, not public: IFoodRatingStrategy is internal to
    // DesignAFoodRatingSystemSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal string? Apply(IFoodRatingStrategy strategy)
    {
        if (_isQuery)
        {
            return strategy.HighestRated(_subject);
        }

        strategy.ChangeRating(_subject, _rating);
        return null;
    }
}
