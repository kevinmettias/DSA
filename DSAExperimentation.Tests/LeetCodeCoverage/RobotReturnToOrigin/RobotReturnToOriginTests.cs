using DSAExperimentation.LeetCode.RobotReturnToOrigin;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotReturnToOrigin;

// Harness only. Both strategies are RobotReturnToOriginSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class RobotReturnToOriginTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "UD", true },
            { "LL", false },
            { "RRDD", false },
            { "LDRRLRUULR", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void JudgeCircleBySwitch_VariousMoveSequences_ReturnsWhetherRobotReturnsToOrigin(
        string moves, bool expected) =>
        Assert.Equal(expected, RobotReturnToOriginSolution.JudgeCircleBySwitch(moves));

    [Theory]
    [MemberData(nameof(Examples))]
    public void JudgeCircleByHashMapLookup_VariousMoveSequences_ReturnsWhetherRobotReturnsToOrigin(
        string moves, bool expected) =>
        Assert.Equal(expected, RobotReturnToOriginSolution.JudgeCircleByHashMapLookup(moves));
}
