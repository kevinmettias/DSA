using DSAExperimentation.LeetCode.LastRemainingIntegerAfterAlternatingDeletionOperations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastRemainingIntegerAfterAlternatingDeletionOperations;

// Harness only. Both strategies are
// LastRemainingIntegerAfterAlternatingDeletionOperationsSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class LastRemainingIntegerAfterAlternatingDeletionOperationsTests
{
    public static TheoryData<long, long> Examples =>
        new()
        {
            { 8, 3 },
            { 5, 1 },
            { 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLastRemainingByListSimulation_LeetCodeExamples_ReturnsSurvivingInteger(
        long startingCount, long expected) =>
        Assert.Equal(
            expected,
            LastRemainingIntegerAfterAlternatingDeletionOperationsSolution.FindLastRemainingByListSimulation(
                startingCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLastRemainingByHeadStepSimulation_LeetCodeExamples_ReturnsSurvivingInteger(
        long startingCount, long expected) =>
        Assert.Equal(
            expected,
            LastRemainingIntegerAfterAlternatingDeletionOperationsSolution.FindLastRemainingByHeadStepSimulation(
                startingCount));
}
