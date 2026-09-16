using FoodRatingsByLazyDeletionHeap = DSAExperimentation.LeetCode.DesignAFoodRatingSystem.DesignAFoodRatingSystemSolution.FoodRatingsByLazyDeletionHeap;
using FoodRatingsByLinearScan = DSAExperimentation.LeetCode.DesignAFoodRatingSystem.DesignAFoodRatingSystemSolution.FoodRatingsByLinearScan;
using IFoodRatingStrategy = DSAExperimentation.LeetCode.DesignAFoodRatingSystem.DesignAFoodRatingSystemSolution.IFoodRatingStrategy;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAFoodRatingSystem;

// Harness only. Both strategies are DesignAFoodRatingSystemSolution's - this file
// replays LeetCode's published call sequence against each IFoodRatingStrategy
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a constructor plus a
// sequence of mutating calls rather than a single argument tuple. FoodRatingOp.Apply
// is pure dispatch (which method to call with which arguments) - no rating or
// tie-break logic of its own.
//
// The solution's nested types are named by the aliases at the top rather than pulled
// in by a `using static`: a wildcard import drops every member in as a bare
// identifier, so a reader meeting FoodRatingsByLinearScan has nothing on the line
// telling them whose it is.
public sealed partial class DesignAFoodRatingSystemTests
{
    public static TheoryData<FoodRatingScript> Examples =>
        new()
        {
            // LeetCode's published example, including the change that makes ramen
            // and sushi tie at 16 so the lexicographically smaller name wins.
            {
                new FoodRatingScript(
                    Foods: ["kimchi", "miso", "sushi", "moussaka", "ramen", "bulgogi"],
                    Cuisines: ["korean", "japanese", "japanese", "greek", "japanese", "korean"],
                    Ratings: [9, 12, 8, 15, 14, 7],
                    Operations:
                    [
                        FoodRatingOp.HighestRated("korean"),
                        FoodRatingOp.HighestRated("japanese"),
                        FoodRatingOp.ChangeRating("sushi", 16),
                        FoodRatingOp.HighestRated("japanese"),
                        FoodRatingOp.ChangeRating("ramen", 16),
                        FoodRatingOp.HighestRated("japanese"),
                    ],
                    Expected: ["kimchi", "ramen", null, "sushi", null, "ramen"])
            },

            // Tied from the start: the smaller name wins without any rating change.
            {
                new FoodRatingScript(
                    Foods: ["bravo", "alpha"],
                    Cuisines: ["mex", "mex"],
                    Ratings: [5, 5],
                    Operations: [FoodRatingOp.HighestRated("mex")],
                    Expected: ["alpha"])
            },

            // A rating lowered rather than raised, so the leader changes and the
            // superseded entry has to be discarded; then restored into a tie, where
            // the smaller name takes the lead back.
            {
                new FoodRatingScript(
                    Foods: ["alpha", "bravo"],
                    Cuisines: ["thai", "thai"],
                    Ratings: [9, 5],
                    Operations:
                    [
                        FoodRatingOp.HighestRated("thai"),
                        FoodRatingOp.ChangeRating("alpha", 1),
                        FoodRatingOp.HighestRated("thai"),
                        FoodRatingOp.ChangeRating("alpha", 5),
                        FoodRatingOp.HighestRated("thai"),
                    ],
                    Expected: ["alpha", null, "bravo", null, "alpha"])
            },

            // Cuisines do not interfere: a change in one leaves the other's answer
            // alone, and a single-food cuisine always reports that food.
            {
                new FoodRatingScript(
                    Foods: ["udon", "pho", "laksa"],
                    Cuisines: ["japanese", "viet", "malay"],
                    Ratings: [3, 4, 5],
                    Operations:
                    [
                        FoodRatingOp.ChangeRating("udon", 100),
                        FoodRatingOp.HighestRated("viet"),
                        FoodRatingOp.HighestRated("malay"),
                        FoodRatingOp.HighestRated("japanese"),
                    ],
                    Expected: [null, "pho", "laksa", "udon"])
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FoodRatingsByLinearScan_LeetCodeExamples_ReturnsHighestRatedFoodPerCuisine(
        FoodRatingScript script) =>
        Assert.Equal(script.Expected, RunScript(
            new FoodRatingsByLinearScan(script.Foods, script.Cuisines, script.Ratings), script.Operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FoodRatingsByLazyDeletionHeap_LeetCodeExamples_ReturnsHighestRatedFoodPerCuisine(
        FoodRatingScript script) =>
        Assert.Equal(script.Expected, RunScript(
            new FoodRatingsByLazyDeletionHeap(script.Foods, script.Cuisines, script.Ratings), script.Operations));

    private static string?[] RunScript(IFoodRatingStrategy strategy, FoodRatingOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(strategy))];

    // One LeetCode example: the constructor's three parallel arrays, the calls to replay
    // against the built rating system, and the judge output for each call. Every argument
    // is named where it is passed, because a food, a cuisine and a rating are three
    // strings and an int whose order nothing else pins down. Nested because it is only
    // ever used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct FoodRatingScript(
        string[] Foods, string[] Cuisines, int[] Ratings, FoodRatingOp[] Operations, string?[] Expected);

    // One call in a FoodRatings script: which method to invoke and with what arguments.
    // Pure dispatch, built via the named factories below so a script (like Examples
    // above) reads like the LeetCode call sequence it replays. Nested because it is only
    // ever used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct FoodRatingOp(bool isQuery, string subject, int rating)
    {
        public static FoodRatingOp ChangeRating(string food, int newRating) => new(isQuery: false, food, newRating);

        public static FoodRatingOp HighestRated(string cuisine) => new(isQuery: true, cuisine, rating: 0);

        // null for the void ChangeRating call, the reported food name for
        // HighestRated - so a script runner can assert against one expected value per
        // operation uniformly. Internal, not public: IFoodRatingStrategy is internal to
        // DesignAFoodRatingSystemSolution, and only this same assembly's RunScript ever
        // calls Apply.
        internal string? Apply(IFoodRatingStrategy strategy)
        {
            if (isQuery)
            {
                return strategy.HighestRated(subject);
            }

            strategy.ChangeRating(subject, rating);
            return null;
        }
    }
}
