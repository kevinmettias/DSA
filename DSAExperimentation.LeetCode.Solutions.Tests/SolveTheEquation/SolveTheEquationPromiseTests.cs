using DSAExperimentation.LeetCode.Solutions.SolveTheEquation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SolveTheEquation;

public sealed partial class SolveTheEquationPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "x+5-3+x=6+x-2", true },
            { "x=x", true },
            { "2x=x", true },
            { "x=x+2", true },
            { "2x-3=5x+7", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenAUniqueSolutionIsAnInteger(string equation, bool expected)
        => Assert.Equal(expected, SolveTheEquationPromise.IsKeptBy(equation));
}
