using DSAExperimentation.LeetCode.Solutions.FruitsIntoBasketsII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FruitsIntoBasketsII;

// Harness only. FruitsIntoBasketsIISolution owns the one strategy this
// problem's n <= 100 bound calls for; this file just pins it to a couple of
// hand-verified edge cases: a single fruit that fits its single basket and one that
// does not. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class FruitsIntoBasketsIISolutionTests
{
    public static TheoryData<int[], int[], int> Cases =>
        new()
        {
            { [1], [1], 0 },
            { [5], [1], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountUnplacedByBruteForce_Cases_ReturnsUnplacedFruitCount(
        int[] fruits, int[] baskets, int expected)
    {
        var unplaced = FruitsIntoBasketsIISolution.CountUnplacedByBruteForce(fruits, baskets);

        Assert.Equal(expected, unplaced);
    }
}
