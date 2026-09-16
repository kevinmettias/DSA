using DSAExperimentation.LeetCode.RobotReturnToOrigin;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotReturnToOrigin;

// Harness only. Both strategies are RobotReturnToOriginSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class RobotReturnToOriginTests
{
    public static TheoryData<JudgeCircleExample> Examples =>
        new()
        {
            new JudgeCircleExample(Moves: "UD", ReturnsToOrigin: true),
            new JudgeCircleExample(Moves: "LL", ReturnsToOrigin: false),
            new JudgeCircleExample(Moves: "RRDD", ReturnsToOrigin: false),
            new JudgeCircleExample(Moves: "LDRRLRUULR", ReturnsToOrigin: false),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void JudgeCircleBySwitch_VariousMoveSequences_ReturnsWhetherRobotReturnsToOrigin(
        JudgeCircleExample example) =>
        Assert.Equal(
            example.ReturnsToOrigin,
            RobotReturnToOriginSolution.JudgeCircleBySwitch(example.Moves));

    [Theory]
    [MemberData(nameof(Examples))]
    public void JudgeCircleByHashMapLookup_VariousMoveSequences_ReturnsWhetherRobotReturnsToOrigin(
        JudgeCircleExample example) =>
        Assert.Equal(
            example.ReturnsToOrigin,
            RobotReturnToOriginSolution.JudgeCircleByHashMapLookup(example.Moves));

    // One example: the move sequence and whether it brings the robot back to the
    // origin. The expectation is named rather than carried by its position, so the
    // row reads as an assertion instead of as a bare `true`.
    public readonly record struct JudgeCircleExample(string Moves, bool ReturnsToOrigin);
}
