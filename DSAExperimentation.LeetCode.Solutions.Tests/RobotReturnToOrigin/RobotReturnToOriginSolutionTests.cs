using DSAExperimentation.LeetCode.Solutions.RobotReturnToOrigin;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RobotReturnToOrigin;

// Harness only. Both strategies are RobotReturnToOriginSolution's - this file
// pins them to a ten-move sequence of all four directions that ends one step right
// and one step up of the origin. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class RobotReturnToOriginSolutionTests
{
    public static TheoryData<JudgeCircleCase> Cases =>
        new()
        {
            new JudgeCircleCase(Moves: "LDRRLRUULR", ReturnsToOrigin: false),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsAtOriginBySwitch_VariousMoveSequences_ReturnsWhetherRobotReturnsToOrigin(
        JudgeCircleCase testCase) =>
        Assert.Equal(
            testCase.ReturnsToOrigin,
            RobotReturnToOriginSolution.IsAtOriginBySwitch(testCase.Moves));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsAtOriginByHashMapLookup_VariousMoveSequences_ReturnsWhetherRobotReturnsToOrigin(
        JudgeCircleCase testCase) =>
        Assert.Equal(
            testCase.ReturnsToOrigin,
            RobotReturnToOriginSolution.IsAtOriginByHashMapLookup(testCase.Moves));

    // One case: the move sequence and whether it brings the robot back to the
    // origin. The expectation is named rather than carried by its position, so the
    // row reads as an assertion instead of as a bare `true`.
    public readonly record struct JudgeCircleCase(string Moves, bool ReturnsToOrigin);
}
