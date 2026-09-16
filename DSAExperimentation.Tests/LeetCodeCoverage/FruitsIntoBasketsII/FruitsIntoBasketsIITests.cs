using DSAExperimentation.LeetCode.FruitsIntoBasketsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FruitsIntoBasketsII;

// Harness only. FruitsIntoBasketsIISolution owns the one strategy this
// problem's n <= 100 bound calls for; this file just pins it to LeetCode's
// published examples plus a couple of hand-verified edge cases.
public sealed class FruitsIntoBasketsIITests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [4, 2, 5], [3, 5, 4], 1 },
            { [3, 6, 1], [6, 4, 7], 0 },
            { [1], [1], 0 },
            { [5], [1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountUnplacedByBruteForce_LeetCodeExamples_ReturnsUnplacedFruitCount(
        int[] fruits, int[] baskets, int expected)
    {
        var unplaced = FruitsIntoBasketsIISolution.CountUnplacedByBruteForce(fruits, baskets);

        Assert.Equal(expected, unplaced);
    }
}
