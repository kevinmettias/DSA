using DSAExperimentation.LeetCode.NumberOfStepsToReduceANumberInBinaryRepresentationToOne;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfStepsToReduceANumberInBinaryRepresentationToOne;

// Harness only. Both strategies are
// NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution's - the single-pass
// carry-propagation scan and the step-by-step binary-addition simulation that used to
// live untested as the benchmark's baseline - pinned to LeetCode's published examples
// plus a cascading carry, an all-halvings case, and the alternating bit pattern the
// benchmark measures.
public sealed class NumberOfStepsToReduceANumberInBinaryRepresentationToOneTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "1101", 6 },
            { "10", 1 },
            { "1", 0 },
            { "111", 4 },
            { "1000", 3 },
            { "1011", 6 },
            { "1010", 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumStepsByCarryPropagationScan_LeetCodeExamples_ReturnsStepsToReachOne(
        string s, int expected) =>
        Assert.Equal(
            expected,
            NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution
                .NumStepsByCarryPropagationScan(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumStepsByStackAddSimulation_LeetCodeExamples_ReturnsStepsToReachOne(
        string s, int expected) =>
        Assert.Equal(
            expected,
            NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution
                .NumStepsByStackAddSimulation(s));
}
