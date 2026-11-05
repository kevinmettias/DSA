using DSAExperimentation.LeetCode.Solutions.CheckIfThereIsAValidParenthesesStringPath;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfThereIsAValidParenthesesStringPath;

public sealed partial class CheckIfThereIsAValidParenthesesStringPathPromiseTests
{
    public static TheoryData<char[][], bool> Inputs =>
        new()
        {
            { [['(', '(', '('], [')', '(', ')'], ['(', '(', ')'], ['(', '(', ')']], true },
            { [[')', ')'], ['(', '(']], true },
            { [['(', '('], [')', ')', ')']], false },
            { [['(', 'x'], [')', ')']], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForARectangleOfParentheses(char[][] grid, bool expected)
        => Assert.Equal(expected, CheckIfThereIsAValidParenthesesStringPathPromise.IsKeptBy(grid));
}
