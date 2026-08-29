namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// The parsed form of LeetCodeQuestionDto.Stats' nested JSON string. AcRate
// arrives as a display string ("58.1%"), not a number - LeetCodeQuestionMapper
// strips the trailing '%' and parses the rest.
internal sealed record LeetCodeStatsDto
{
    public required long TotalAcceptedRaw { get; init; }
    public required long TotalSubmissionRaw { get; init; }
    public required string AcRate { get; init; }
}
