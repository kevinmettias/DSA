namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogCallAnswersTests
{
    // One script: the constructor, a call to method 0, which returns nothing, then a call to method 1, which
    // answered 7.
    private static readonly CatalogCallAnswers Answers = new([[CatalogScripts.Constructor, 0, 1]], [null, new int[][] { [0, 0, 7] }]);

    [Fact]
    public void NotationOf_AnyCase_WritesNullForTheConstructorAndEachCallReturningNothing() =>
        Assert.Equal("[null,null,7]", Answers.NotationOf(0).ToJsonString());

    [Fact]
    public void Count_AnyAnswers_CountsTheirCases() => Assert.Equal(1, Answers.Count);
}
