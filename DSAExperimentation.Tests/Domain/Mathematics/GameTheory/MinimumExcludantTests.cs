using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Domain.Mathematics.GameTheory;

public sealed partial class MinimumExcludantTests
{
    public static TheoryData<int[], int> Lists => new()
    {
        { [], 0 },
        { [1, 2], 0 },
        { [0, 1, 3], 2 },
        { [2, 0, 1, 1, 0], 3 },
        { [0, 5, 100], 1 },
    };

    [Theory]
    [MemberData(nameof(Lists))]
    public void Of_List_IsTheLeastNaturalItLacks(int[] numbers, int expected) => Assert.Equal(expected, MinimumExcludant.Of(numbers));

    // A list longer than the stack buffer still finds its mex.
    [Fact]
    public void Of_LongList_IsTheLeastNaturalItLacks() =>
        Assert.Equal(1_000, MinimumExcludant.Of([.. Enumerable.Range(0, 1_000)]));
}
