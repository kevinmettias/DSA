using DSAExperimentation.LeetCode.Dota2Senate;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Dota2Senate;

// Harness only: both strategies live in Dota2SenateSolution and are asserted
// against the same examples, including the single-party cases neither strategy's
// voting loop ever runs for.
public sealed class Dota2SenateTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "RD", "Radiant" },
            { "RDD", "Dire" },
            { "R", "Radiant" },
            { "D", "Dire" },
            { "RRDDD", "Radiant" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PredictPartyVictoryByCircularRescan_LeetCodeExamples_ReturnsWinningParty(
        string senate, string expected) =>
        Assert.Equal(expected, Dota2SenateSolution.PredictPartyVictoryByCircularRescan(senate));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PredictPartyVictoryByTwoQueueSimulation_LeetCodeExamples_ReturnsWinningParty(
        string senate, string expected) =>
        Assert.Equal(expected, Dota2SenateSolution.PredictPartyVictoryByTwoQueueSimulation(senate));
}
