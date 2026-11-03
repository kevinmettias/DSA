namespace DSAExperimentation.LeetCode.Catalog.Tests;

public sealed partial class LeetCodeCaseTests
{
    private static readonly LeetCodeCase TwoSumCase = new() { Inputs = ["[2,7,11,15]", "9"], ExpectedOutput = "[0,1]" };

    private static readonly LeetCodeCase InPlaceCase = new() { Inputs = ["[1,2,3]", "1"] };

    [Fact]
    public void Input_FirstPosition_ParsesTheArray()
        => Assert.Equal([2, 7, 11, 15], TwoSumCase.Input<int[]>(0));

    [Fact]
    public void Input_SecondPosition_ParsesTheTarget()
        => Assert.Equal(9, TwoSumCase.Input<int>(1));

    [Fact]
    public void Expected_CaseWithAnAnswer_ParsesIt()
        => Assert.Equal([0, 1], TwoSumCase.Expected<int[]>());

    [Fact]
    public void Expected_CaseWithoutAnAnswer_Throws()
        => Assert.Throws<InvalidOperationException>(() => InPlaceCase.Expected<int[]>());

    [Fact]
    public void HasExpectedOutput_ReportsWhetherAnAnswerIsCarried()
        => Assert.Equal((true, false), (TwoSumCase.HasExpectedOutput, InPlaceCase.HasExpectedOutput));
}
