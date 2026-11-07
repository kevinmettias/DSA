using DSAExperimentation.LeetCode.Solutions.FindPositiveIntegerSolutionForAGivenEquation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindPositiveIntegerSolutionForAGivenEquation;

// StatedCustomFunctions holds the two formulas LC 1237's statement states, function_id 1
// as x + y and 2 as x * y, and refuses the seven it leaves unstated.
public sealed partial class StatedCustomFunctionsTests
{
    // function_id, x, y, and f(x, y) by the statement's formula for that id.
    public static TheoryData<int, int, int, int> StatedValues =>
        new()
        {
            { StatedCustomFunctions.SumId, 1, 4, 5 },
            { StatedCustomFunctions.SumId, 1000, 1000, 2000 },
            { StatedCustomFunctions.ProductId, 1, 5, 5 },
            { StatedCustomFunctions.ProductId, 7, 9, 63 },
        };

    public static TheoryData<int> StatedIds =>
        new()
        {
            StatedCustomFunctions.SumId,
            StatedCustomFunctions.ProductId,
        };

    // LeetCode's ids the statement states no formula for, and ids outside 1 to 9.
    public static TheoryData<int> UnstatedIds =>
        new()
        {
            3,
            9,
            0,
            10,
        };

    [Theory]
    [MemberData(nameof(StatedValues))]
    public void Of_StatedFunctionId_EvaluatesTheStatedFormula(int functionId, int xValue, int yValue, int expected)
    {
        var function = StatedCustomFunctions.Of(functionId);

        Assert.Equal(expected, function.Evaluate(xValue, yValue));
    }

    [Theory]
    [MemberData(nameof(UnstatedIds))]
    public void Of_UnstatedFunctionId_Throws(int functionId)
        => Assert.Throws<ArgumentOutOfRangeException>(() => StatedCustomFunctions.Of(functionId));

    [Theory]
    [MemberData(nameof(StatedIds))]
    public void IsStated_StatedFunctionId_IsTrue(int functionId)
        => Assert.True(StatedCustomFunctions.IsStated(functionId));

    [Theory]
    [MemberData(nameof(UnstatedIds))]
    public void IsStated_UnstatedFunctionId_IsFalse(int functionId)
        => Assert.False(StatedCustomFunctions.IsStated(functionId));
}
