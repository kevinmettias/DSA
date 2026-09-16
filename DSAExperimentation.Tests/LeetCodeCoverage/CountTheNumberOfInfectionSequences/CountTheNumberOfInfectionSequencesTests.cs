using DSAExperimentation.LeetCode.CountTheNumberOfInfectionSequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfInfectionSequences;

// Harness only: both strategies live in CountTheNumberOfInfectionSequencesSolution.
// One test method per strategy over one shared set of LeetCode's published
// examples, so a failure names the strategy that broke.
public sealed partial class CountTheNumberOfInfectionSequencesTests
{
    public static TheoryData<int, int[], long> Examples =>
        new()
        {
            { 5, [0, 4], 4 },
            { 4, [1], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSequencesByBruteForceSimulation_LeetCodeExamples_ReturnsInfectionSequenceCount(
        int childCount, int[] sick, long expected)
    {
        var actual = CountTheNumberOfInfectionSequencesSolution.CountSequencesByBruteForceSimulation(childCount, sick);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSequencesByGapCombinatorics_LeetCodeExamples_ReturnsInfectionSequenceCount(
        int childCount, int[] sick, long expected)
    {
        var actual = CountTheNumberOfInfectionSequencesSolution.CountSequencesByGapCombinatorics(childCount, sick);
        Assert.Equal(expected, actual);
    }
}
