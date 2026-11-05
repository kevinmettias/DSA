using DSAExperimentation.LeetCode.Solutions.Dota2Senate;

namespace DSAExperimentation.LeetCode.Solutions.Tests.Dota2Senate;

// Harness only: both strategies live in Dota2SenateSolution and are asserted
// against the same cases, among them the single-party cases neither strategy's
// voting loop ever runs for.
public sealed partial class Dota2SenateSolutionTests
{
    public static TheoryData<SenateCase> Cases =>
        new()
        {
            { new SenateCase(Senate: "R", Expected: "Radiant") },
            { new SenateCase(Senate: "D", Expected: "Dire") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PredictPartyVictoryByCircularRescan_Cases_ReturnsWinningParty(
        SenateCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            Dota2SenateSolution.PredictPartyVictoryByCircularRescan(testCase.Senate));

    [Theory]
    [MemberData(nameof(Cases))]
    public void PredictPartyVictoryByTwoQueueSimulation_Cases_ReturnsWinningParty(
        SenateCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            Dota2SenateSolution.PredictPartyVictoryByTwoQueueSimulation(testCase.Senate));

    // One case: the senate string and the party that ends up voting last.
    // The two values are named fields rather than two adjacent `string` parameters, so a
    // row is written `new SenateCase(Senate: ..., Expected: ...)` and a senate/expected
    // swap has to be typed out by name instead of falling out of a position the compiler
    // would have accepted either way. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct SenateCase(string Senate, string Expected);
}
