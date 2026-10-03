using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

namespace DSAExperimentation.LeetCode.Tests.CheckIfDigitsAreEqualInStringAfterOperationsI;

// Harness only. Both the direct reduction and the Pascal-row closed form live in
// CheckIfDigitsAreEqualInStringAfterOperationsISolution - this file just pins both
// strategies to LeetCode's published examples, and to three strings at LeetCode's
// longest length of 100, where Pascal's row reaches C(98, 49) - far past what an int
// holds, so the closed form must reduce its coefficients as it builds them. Their
// expected answers come from repeated reduction done independently of this repo.
public sealed partial class CheckIfDigitsAreEqualInStringAfterOperationsISolutionTests
{
    public static TheoryData<DigitsMatchExample> Examples =>
        new()
        {
            { new DigitsMatchExample(S: "3902", Expected: true) },
            { new DigitsMatchExample(S: "34789", Expected: false) },
            { new DigitsMatchExample(S: "3281682764585725185345670408093172451108086218537328827453519803664066968922927668189788859525366237", Expected: true) },
            { new DigitsMatchExample(S: "6143413446969357935179378979297611757009661270150571369430652055494458324962936058446164134478978850", Expected: true) },
            { new DigitsMatchExample(S: "4802812871324202104328356138699172537069112389069562464370188877711179881377534360117578841151390885", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsEqualByAdjacentSumReduction_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        DigitsMatchExample example) =>
        Assert.Equal(
            example.Expected,
            CheckIfDigitsAreEqualInStringAfterOperationsISolution.IsEqualByAdjacentSumReduction(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsEqualByPascalRowCoefficients_LeetCodeExamples_ReturnsWhetherFinalDigitsMatch(
        DigitsMatchExample example) =>
        Assert.Equal(
            example.Expected,
            CheckIfDigitsAreEqualInStringAfterOperationsISolution.IsEqualByPascalRowCoefficients(example.S));

    // One LeetCode example: the digit string and whether the two final digits match. The
    // row names both positions - a bare `bool` argument would read as "true" and say
    // nothing about what is true.
    public readonly record struct DigitsMatchExample(string S, bool Expected);
}
