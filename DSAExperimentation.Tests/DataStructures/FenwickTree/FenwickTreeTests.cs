using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class FenwickTreeTests
{
    private static readonly int[] AscendingFive = [1, 2, 3, 4, 5];

    [Fact]
    public void Constructor_FromInitialValues_PrefixQueryMatchesRunningSum()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        Assert.Equal(1, tree.PrefixQuery(0));
        Assert.Equal(Fixtures.PrefixSumThroughIndex1, tree.PrefixQuery(1));
        Assert.Equal(Fixtures.PrefixSumThroughLastIndex, tree.PrefixQuery(Fixtures.LastIndex));
    }

    [Fact]
    public void Query_SumOperation_ReturnsRangeSum()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        var sumThroughRangeEnd = tree.Query(1, Fixtures.RangeEnd);
        Assert.Equal(Fixtures.SumThroughRangeEnd, sumThroughRangeEnd);

        var sumOfAllElements = tree.Query(0, Fixtures.LastIndex);
        Assert.Equal(Fixtures.SumOfAllElements, sumOfAllElements);

        var valueAtMiddleIndex = tree.Query(Fixtures.MiddleIndex, Fixtures.MiddleIndex);
        Assert.Equal(Fixtures.ValueAtMiddleIndex, valueAtMiddleIndex);
    }

    [Fact]
    public void Add_AppliesDeltaToEveryQueryCoveringThatIndex()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        tree.Add(Fixtures.UpdateIndex, Fixtures.UpdateDelta);

        var valueAtUpdateIndex = tree.Query(Fixtures.UpdateIndex, Fixtures.UpdateIndex);
        Assert.Equal(Fixtures.ValueAtUpdateIndex, valueAtUpdateIndex);

        var prefixSumThroughUpdateIndex = tree.PrefixQuery(Fixtures.UpdateIndex);
        Assert.Equal(Fixtures.PrefixSumThroughUpdateIndex, prefixSumThroughUpdateIndex);

        var sumOfFirstTwoElements = tree.Query(0, 1);
        Assert.Equal(Fixtures.SumOfFirstTwoElements, sumOfFirstTwoElements);

        var sumOfAllElementsAfterUpdate = tree.Query(0, Fixtures.LastIndex);
        Assert.Equal(Fixtures.SumOfAllElementsAfterUpdate, sumOfAllElementsAfterUpdate);
    }

    [Fact]
    public void Query_XorOperation_ReturnsRangeXor()
    {
        var tree = new FenwickTree<int, XorOperation<int>>(AscendingFive);

        var xorOfFirstThreeElements = tree.Query(0, Fixtures.FirstThreeRangeEnd);
        Assert.Equal(Fixtures.XorOfFirstThreeElements, xorOfFirstThreeElements);

        var xorOfLastTwoElements = tree.Query(Fixtures.LastTwoRangeStart, Fixtures.LastTwoRangeEnd);
        Assert.Equal(Fixtures.XorOfLastTwoElements, xorOfLastTwoElements);
    }

    [Fact]
    public void Constructor_SizeOnly_StartsAtIdentityEverywhere()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(Fixtures.SizeOnlyTreeSize);

        var sumOfAllElements = tree.Query(0, Fixtures.LastIndex);
        Assert.Equal(0, sumOfAllElements);
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(Fixtures.CountTreeSize);

        Assert.Equal(Fixtures.CountTreeSize, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, Fixtures.OutOfRangeIndex));
    }

    [Fact]
    public void Add_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Add(Fixtures.OutOfRangeIndex, Fixtures.Delta));
    }

    /// <summary>
    /// The indices these tests probe and the values they expect back, named once so a
    /// second test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const int PrefixSumThroughIndex1 = 3;
        public const int LastIndex = 4;
        public const int PrefixSumThroughLastIndex = 15;
        public const int RangeEnd = 3;
        public const int SumThroughRangeEnd = 9;
        public const int SumOfAllElements = 15;
        public const int MiddleIndex = 2;
        public const int ValueAtMiddleIndex = 3;
        public const int UpdateIndex = 2;
        public const int UpdateDelta = 10;
        public const int ValueAtUpdateIndex = 13;
        public const int PrefixSumThroughUpdateIndex = 16;
        public const int SumOfFirstTwoElements = 3;
        public const int SumOfAllElementsAfterUpdate = 25;
        public const int FirstThreeRangeEnd = 2;
        public const int XorOfFirstThreeElements = 1 ^ 2 ^ 3;
        public const int LastTwoRangeStart = 3;
        public const int LastTwoRangeEnd = 4;
        public const int XorOfLastTwoElements = 4 ^ 5;
        public const int SizeOnlyTreeSize = 5;
        public const int CountTreeSize = 7;
        public const int OutOfRangeIndex = 5;
        public const int Delta = 10;
    }
}
