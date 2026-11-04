using DSAExperimentation.LeetCode.Solutions.ReverseSubstringsBetweenEachPairOfParentheses;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReverseSubstringsBetweenEachPairOfParentheses;

public sealed partial class ReverseSubstringsBetweenEachPairOfParenthesesPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "(u(love)i)", true },
            { "a(bcdefghijkl(mno)p)q", true },
            { "(a(b(c)d)e(f(g)h)i", false },
            { "a)b(", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryParenthesisCloses(string text, bool expected)
        => Assert.Equal(expected, ReverseSubstringsBetweenEachPairOfParenthesesPromise.IsKeptBy(text));
}
