using DSAExperimentation.LeetCode.Solutions.NumberOfWaysOfCuttingAPizza;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysOfCuttingAPizza;

public sealed partial class NumberOfWaysOfCuttingAPizzaPromiseTests
{
    public static TheoryData<string[], int, bool> Inputs =>
        new()
        {
            { ["A..", "AAA", "..."], 3, true },
            { ["A.A.A.A", ".A.A.", "A.A.A.A"], 6, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForARectangularPizza(string[] pizza, int pieceCount, bool expected)
        => Assert.Equal(expected, NumberOfWaysOfCuttingAPizzaPromise.IsKeptBy(pizza, pieceCount));
}
