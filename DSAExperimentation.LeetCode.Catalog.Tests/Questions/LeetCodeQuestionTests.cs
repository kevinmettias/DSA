namespace DSAExperimentation.LeetCode.Catalog.Tests.Questions;

public sealed partial class LeetCodeQuestionTests
{
    [Fact]
    public void Url_TwoSum_IsItsLeetCodeProblemPage()
        => Assert.Equal(new Uri("https://leetcode.com/problems/two-sum/"), LeetCodeCatalog.Question("two-sum").Url);
}
