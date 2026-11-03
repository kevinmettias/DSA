namespace DSAExperimentation.LeetCode.Catalog.Sync.Ratings;

// One entry of zerotrac's data.json, the members the catalog keeps; the file also carries the
// question's id, title and Chinese title and contest names, which the catalog has from LeetCode.
internal sealed record ZerotracRatingDto
{
    public required double Rating { get; init; }

    public required string TitleSlug { get; init; }

    public required string ContestSlug { get; init; }

    public required string ProblemIndex { get; init; }
}
