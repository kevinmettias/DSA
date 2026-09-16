using DSAExperimentation.LeetCode.FruitsIntoBasketsIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FruitsIntoBasketsIII;

// Harness only. The segment tree itself is this repo's own
// SegmentTree<int, MaxOperation<int>>; both search strategies are
// FruitsIntoBasketsIIISolution's - this file just pins them to LeetCode's
// published examples (identical to LC 3477's, at the harder n <= 1e5 bound)
// plus a couple of hand-verified edge cases.
public sealed partial class FruitsIntoBasketsIIITests
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
        var actual = FruitsIntoBasketsIIISolution.CountUnplacedByBruteForce(fruits, baskets);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountUnplacedBySegmentTreeSearch_LeetCodeExamples_ReturnsUnplacedFruitCount(
        int[] fruits, int[] baskets, int expected)
    {
        var actual = FruitsIntoBasketsIIISolution.CountUnplacedBySegmentTreeSearch(fruits, baskets);

        Assert.Equal(expected, actual);
    }
}
