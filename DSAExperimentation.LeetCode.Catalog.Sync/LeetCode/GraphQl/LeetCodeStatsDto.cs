namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

// The parsed form of LeetCodeQuestionDto.Stats' nested JSON string. Only the raw totals are read:
// the acceptance rate comes from the question's own acRate, which is not rounded to one place.
internal sealed record LeetCodeStatsDto
{
    public required long TotalAcceptedRaw { get; init; }

    public required long TotalSubmissionRaw { get; init; }
}
