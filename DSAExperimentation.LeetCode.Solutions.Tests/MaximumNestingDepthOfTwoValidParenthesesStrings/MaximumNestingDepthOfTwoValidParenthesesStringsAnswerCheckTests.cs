using DSAExperimentation.LeetCode.Solutions.MaximumNestingDepthOfTwoValidParenthesesStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNestingDepthOfTwoValidParenthesesStrings;

public sealed partial class MaximumNestingDepthOfTwoValidParenthesesStringsAnswerCheckTests
{
    public static TheoryData<string, int[], int[], bool> Answers =>
        new()
        {
            { "(()())", [0, 1, 1, 1, 1, 0], [0, 1, 1, 1, 1, 0], true },
            { "(()())", [1, 0, 0, 0, 0, 1], [0, 1, 1, 1, 1, 0], true },
            { "()(())()", [0, 0, 0, 1, 1, 0, 1, 1], [0, 0, 0, 1, 1, 0, 1, 1], true },
            { "(()())", [0, 0, 0, 0, 0, 0], [0, 1, 1, 1, 1, 0], false },
            { "(())", [0, 0, 1, 1], [0, 1, 1, 0], false },
            { "()", [0, 2], [0, 0], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForASplitIntoTwoValidStringsAsShallowAsTheRecordedOne(string seq, int[] answer, int[] recorded, bool expected)
        => Assert.Equal(expected, MaximumNestingDepthOfTwoValidParenthesesStringsAnswerCheck.IsAccepted(seq, answer, recorded));
}
