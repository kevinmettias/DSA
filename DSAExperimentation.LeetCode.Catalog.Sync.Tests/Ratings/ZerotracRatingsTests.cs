using DSAExperimentation.LeetCode.Catalog.Sync.Ratings;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.Ratings;

public sealed partial class ZerotracRatingsTests
{
    private static readonly IReadOnlyList<ZerotracRatingDto> Ratings =
    [
        new() { Rating = 1600.5, TitleSlug = "covered", ContestSlug = "weekly-contest-1", ProblemIndex = "Q2" },
        new() { Rating = 2100, TitleSlug = "not-covered", ContestSlug = "weekly-contest-2", ProblemIndex = "Q4" },
    ];

    [Fact]
    public void ForEnrolled_RatingsForEnrolledAndOtherQuestions_KeepsOnlyTheEnrolled()
    {
        var file = ZerotracRatings.ForEnrolled(Ratings, new HashSet<string>(["covered"]));

        var rating = Assert.Single(file.Questions);
        Assert.Equal(("covered", new LeetCodeContestRating(1600.5, "weekly-contest-1", "Q2")), (rating.Key, rating.Value));
    }

    [Fact]
    public void ForEnrolled_AnyRatings_NamesZerotracAsTheSource()
        => Assert.Equal(ZerotracRatings.ProjectUrl, ZerotracRatings.ForEnrolled(Ratings, new HashSet<string>()).Source);
}
