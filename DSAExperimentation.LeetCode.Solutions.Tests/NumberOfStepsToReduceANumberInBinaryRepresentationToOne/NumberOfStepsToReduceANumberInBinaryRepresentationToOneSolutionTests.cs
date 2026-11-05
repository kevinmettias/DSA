using DSAExperimentation.LeetCode.Solutions.NumberOfStepsToReduceANumberInBinaryRepresentationToOne;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfStepsToReduceANumberInBinaryRepresentationToOne;

// Harness only. Both strategies are
// NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution's - the single-pass
// carry-propagation scan and the step-by-step binary-addition simulation that used to
// live untested as the benchmark's baseline - pinned to a cascading carry, an
// all-halvings case, and the alternating bit pattern the benchmark measures.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "1000", 3 },
            { "1011", 6 },
            { "1010", 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountStepsByCarryPropagationScan_Cases_ReturnsStepsToReachOne(
        string binaryString, int expected) =>
        Assert.Equal(
            expected,
            NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution
                .CountStepsByCarryPropagationScan(binaryString));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountStepsByStackAddSimulation_Cases_ReturnsStepsToReachOne(
        string binaryString, int expected) =>
        Assert.Equal(
            expected,
            NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution
                .CountStepsByStackAddSimulation(binaryString));
}
