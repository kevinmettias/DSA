using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class FenwickTreeTests
{
    private static readonly int[] AscendingFive = [1, 2, 3, 4, 5];
    private static readonly int[] AscendingThree = [1, 2, 3];

    [Fact]
    public void Constructor_FromInitialValues_PrefixQueryMatchesRunningSum()
    {
        const int PrefixSumThroughIndex1 = 3;
        const int LastIndex = 4;
        const int PrefixSumThroughLastIndex = 15;

        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        Assert.Equal(1, tree.PrefixQuery(0));
        Assert.Equal(PrefixSumThroughIndex1, tree.PrefixQuery(1));
        Assert.Equal(PrefixSumThroughLastIndex, tree.PrefixQuery(LastIndex));
    }

    [Fact]
    public void Query_SumOperation_ReturnsRangeSum()
    {
        const int RangeEnd = 3;
        const int SumThroughRangeEnd = 9;
        const int LastIndex = 4;
        const int SumOfAllElements = 15;
        const int MiddleIndex = 2;
        const int ValueAtMiddleIndex = 3;

        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        var sumThroughRangeEnd = tree.Query(1, RangeEnd);
        Assert.Equal(SumThroughRangeEnd, sumThroughRangeEnd);

        var sumOfAllElements = tree.Query(0, LastIndex);
        Assert.Equal(SumOfAllElements, sumOfAllElements);

        var valueAtMiddleIndex = tree.Query(MiddleIndex, MiddleIndex);
        Assert.Equal(ValueAtMiddleIndex, valueAtMiddleIndex);
    }

    [Fact]
    public void Add_AppliesDeltaToEveryQueryCoveringThatIndex()
    {
        const int UpdateIndex = 2;
        const int UpdateDelta = 10;
        const int ValueAtUpdateIndex = 13;
        const int SumOfFirstTwoElements = 3;
        const int LastIndex = 4;
        const int SumOfAllElementsAfterUpdate = 25;

        var tree = new FenwickTree<int, SumOperation<int>>(AscendingFive);

        tree.Add(UpdateIndex, UpdateDelta);

        var valueAtUpdateIndex = tree.Query(UpdateIndex, UpdateIndex);
        Assert.Equal(ValueAtUpdateIndex, valueAtUpdateIndex);

        var sumOfFirstTwoElements = tree.Query(0, 1);
        Assert.Equal(SumOfFirstTwoElements, sumOfFirstTwoElements);

        var sumOfAllElementsAfterUpdate = tree.Query(0, LastIndex);
        Assert.Equal(SumOfAllElementsAfterUpdate, sumOfAllElementsAfterUpdate);
    }

    [Fact]
    public void Query_XorOperation_ReturnsRangeXor()
    {
        const int FirstThreeRangeEnd = 2;
        const int XorOfFirstThreeElements = 1 ^ 2 ^ 3;
        const int LastTwoRangeStart = 3;
        const int LastTwoRangeEnd = 4;
        const int XorOfLastTwoElements = 4 ^ 5;

        var tree = new FenwickTree<int, XorOperation<int>>(AscendingFive);

        var xorOfFirstThreeElements = tree.Query(0, FirstThreeRangeEnd);
        Assert.Equal(XorOfFirstThreeElements, xorOfFirstThreeElements);

        var xorOfLastTwoElements = tree.Query(LastTwoRangeStart, LastTwoRangeEnd);
        Assert.Equal(XorOfLastTwoElements, xorOfLastTwoElements);
    }

    [Fact]
    public void Constructor_SizeOnly_StartsAtIdentityEverywhere()
    {
        const int TreeSize = 5;
        const int LastIndex = 4;

        var tree = new FenwickTree<int, SumOperation<int>>(TreeSize);

        var sumOfAllElements = tree.Query(0, LastIndex);
        Assert.Equal(0, sumOfAllElements);
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        const int TreeSize = 7;

        var tree = new FenwickTree<int, SumOperation<int>>(TreeSize);

        Assert.Equal(TreeSize, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        const int OutOfRangeIndex = 3;

        var tree = new FenwickTree<int, SumOperation<int>>(AscendingThree);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, OutOfRangeIndex));
    }

    [Fact]
    public void Add_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        const int OutOfRangeIndex = 3;
        const int Delta = 10;

        var tree = new FenwickTree<int, SumOperation<int>>(AscendingThree);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Add(OutOfRangeIndex, Delta));
    }
}
