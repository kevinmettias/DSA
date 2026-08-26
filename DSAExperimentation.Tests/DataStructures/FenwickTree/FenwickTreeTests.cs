using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class FenwickTreeTests
{
    [Fact]
    public void Constructor_FromInitialValues_PrefixQueryMatchesRunningSum()
    {
        var tree = new FenwickTree<int, SumOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(1, tree.PrefixQuery(0));
        Assert.Equal(3, tree.PrefixQuery(1));
        Assert.Equal(15, tree.PrefixQuery(4));
    }

    [Fact]
    public void Query_SumOperation_ReturnsRangeSum()
    {
        var tree = new FenwickTree<int, SumOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(9, tree.Query(1, 3));
        Assert.Equal(15, tree.Query(0, 4));
        Assert.Equal(3, tree.Query(2, 2));
    }

    [Fact]
    public void Add_AppliesDeltaToEveryQueryCoveringThatIndex()
    {
        var tree = new FenwickTree<int, SumOperation<int>>([1, 2, 3, 4, 5]);

        tree.Add(2, 10);

        Assert.Equal(13, tree.Query(2, 2));
        Assert.Equal(3, tree.Query(0, 1));
        Assert.Equal(25, tree.Query(0, 4));
    }

    [Fact]
    public void Query_XorOperation_ReturnsRangeXor()
    {
        var tree = new FenwickTree<int, XorOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(1 ^ 2 ^ 3, tree.Query(0, 2));
        Assert.Equal(4 ^ 5, tree.Query(3, 4));
    }

    [Fact]
    public void Constructor_SizeOnly_StartsAtIdentityEverywhere()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(5);

        Assert.Equal(0, tree.Query(0, 4));
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(7);

        Assert.Equal(7, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new FenwickTree<int, SumOperation<int>>([1, 2, 3]);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, 3));
    }

    [Fact]
    public void Add_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new FenwickTree<int, SumOperation<int>>([1, 2, 3]);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Add(3, 10));
    }
}
