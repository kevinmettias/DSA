using DSAExperimentation.LeetCode.Dota2Senate;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Dota2Senate;

// Harness only: both strategies live in Dota2SenateSolution and are asserted
// against the same examples, including the single-party cases neither strategy's
// voting loop ever runs for.
public sealed partial class Dota2SenateTests
{
    public static TheoryData<SenateCase> Examples =>
        new()
        {
            { new SenateCase(Senate: "RD", Expected: "Radiant") },
            { new SenateCase(Senate: "RDD", Expected: "Dire") },
            { new SenateCase(Senate: "R", Expected: "Radiant") },
            { new SenateCase(Senate: "D", Expected: "Dire") },
            { new SenateCase(Senate: "RRDDD", Expected: "Radiant") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PredictPartyVictoryByCircularRescan_LeetCodeExamples_ReturnsWinningParty(
        SenateCase example) =>
        Assert.Equal(
            example.Expected,
            Dota2SenateSolution.PredictPartyVictoryByCircularRescan(example.Senate));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PredictPartyVictoryByTwoQueueSimulation_LeetCodeExamples_ReturnsWinningParty(
        SenateCase example) =>
        Assert.Equal(
            example.Expected,
            Dota2SenateSolution.PredictPartyVictoryByTwoQueueSimulation(example.Senate));

    // One LeetCode example: the senate string and the party that ends up voting last.
    // The two values are named fields rather than two adjacent `string` parameters, so a
    // row is written `new SenateCase(Senate: ..., Expected: ...)` and a senate/expected
    // swap has to be typed out by name instead of falling out of a position the compiler
    // would have accepted either way. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct SenateCase(string Senate, string Expected);
}
