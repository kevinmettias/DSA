using DSAExperimentation.LeetCode.AirplaneSeatAssignmentProbability;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AirplaneSeatAssignmentProbability;

// Harness only: both strategies live in AirplaneSeatAssignmentProbabilitySolution and
// are asserted against the same examples. The memoized recursion is the definition and
// the closed form is what it reduces to, so pinning both to one example set is exactly
// the check that the reduction holds - the benchmark previously compared them with
// only the recursion under test.
public sealed class AirplaneSeatAssignmentProbabilityTests
{
    public static TheoryData<int, double> Examples =>
        new()
        {
            { 1, 1.0 },
            { 2, 0.5 },
            { 3, 0.5 },
            { 4, 0.5 },
            { 5, 0.5 },
            { 10, 0.5 },
            { 50, 0.5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthPersonGetsNthSeatByMemoizedRecursion_LeetCodeExamples_MatchesExpectedProbability(
        int planeSize,
        double expected) =>
        Assert.Equal(
            expected,
            AirplaneSeatAssignmentProbabilitySolution.NthPersonGetsNthSeatByMemoizedRecursion(planeSize),
            precision: 9);

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthPersonGetsNthSeatByClosedForm_LeetCodeExamples_MatchesExpectedProbability(
        int planeSize,
        double expected) =>
        Assert.Equal(
            expected,
            AirplaneSeatAssignmentProbabilitySolution.NthPersonGetsNthSeatByClosedForm(planeSize),
            precision: 9);
}
