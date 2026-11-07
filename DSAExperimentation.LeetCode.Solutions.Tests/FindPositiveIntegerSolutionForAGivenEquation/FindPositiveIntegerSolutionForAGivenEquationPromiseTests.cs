using DSAExperimentation.LeetCode.Solutions.FindPositiveIntegerSolutionForAGivenEquation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindPositiveIntegerSolutionForAGivenEquation;

public sealed partial class FindPositiveIntegerSolutionForAGivenEquationPromiseTests
{
    public static TheoryData<int, int, bool> Inputs =>
        new()
        {
            { 1, 5, true },
            { 2, 5, true },
            { 2, 100, true },
            { 3, 5, false },
            { 9, 100, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheStatementStatesTheFunction(int functionId, int targetValue, bool expected)
        => Assert.Equal(expected, FindPositiveIntegerSolutionForAGivenEquationPromise.IsKeptBy(functionId, targetValue));
}
