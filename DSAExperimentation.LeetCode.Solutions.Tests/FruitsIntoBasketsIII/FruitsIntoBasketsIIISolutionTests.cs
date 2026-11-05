using DSAExperimentation.LeetCode.Solutions.FruitsIntoBasketsIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FruitsIntoBasketsIII;

// Harness only. The segment tree itself is this repo's own
// SegmentTree<int, Maximum<int>>; both search strategies are
// FruitsIntoBasketsIIISolution's - this file just pins them to a couple of
// hand-verified edge cases: a single fruit that exactly fills the single basket, and
// a single fruit too large for it. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class FruitsIntoBasketsIIISolutionTests
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
        var actual = FruitsIntoBasketsIIISolution.CountUnplacedByBruteForce(fruits, baskets);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountUnplacedBySegmentTreeSearch_Cases_ReturnsUnplacedFruitCount(
        int[] fruits, int[] baskets, int expected)
    {
        var actual = FruitsIntoBasketsIIISolution.CountUnplacedBySegmentTreeSearch(fruits, baskets);

        Assert.Equal(expected, actual);
    }
}
