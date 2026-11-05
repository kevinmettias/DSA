using DSAExperimentation.LeetCode.Solutions.DesignBitset;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DesignBitset;

public sealed partial class DesignBitsetCallsTests
{
    // Five bits with the fourth fixed, written from the first.
    private const string FourthOfFiveSet = "00010";

    [Fact]
    public void ToString_BitsetWithOneBitFixed_WritesEachBitInOrder()
    {
        var bitset = DesignBitsetSolution.CreateByEagerFlip(5);
        bitset.Fix(3);

        Assert.Equal(FourthOfFiveSet, DesignBitsetCalls.ToString(bitset));
    }
}
