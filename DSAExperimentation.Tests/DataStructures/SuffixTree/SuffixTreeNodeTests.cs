using DSAExperimentation.DataStructures.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SuffixTree;

public sealed partial class SuffixTreeNodeTests
{
    // Zero is the boundary case: the whole text is the suffix starting at index 0, so a
    // node ending it is a suffix end like any other.
    public static TheoryData<int> RealSuffixStarts => new() { 0, 5 };

    // A fresh node marks no suffix: SuffixStart starts at -1, the "not a suffix end"
    // sentinel, rather than at 0, which is a real starting index.
    [Fact]
    public void IsSuffixEnd_NewNode_IsFalse() => Assert.False(new SuffixTreeNode().IsSuffixEnd);

    // The build sets the count when the node's subtree is complete; until then it counts nothing.
    [Fact]
    public void SuffixCount_NewNode_IsZero() => Assert.Equal(0, new SuffixTreeNode().SuffixCount);

    [Theory]
    [MemberData(nameof(RealSuffixStarts))]
    public void IsSuffixEnd_AnySuffixStartFromZeroUp_IsTrue(int suffixStart)
        => Assert.True(new SuffixTreeNode { SuffixStart = suffixStart }.IsSuffixEnd);
}
