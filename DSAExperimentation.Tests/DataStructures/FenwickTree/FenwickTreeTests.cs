using DSAExperimentation.Domain.Mathematics.Algebra;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

// The one Fenwick tree's contract, run over its configurations: the point tree under addition and
// under exclusive or, and the range configuration under addition of ints and of longs. A seeded
// script of updates must leave every range query and every prefix equal to the group's fold of a
// plain array that took the same updates. A point update reaches the range configuration as a
// range of one, and ranges go only to the range configuration, which is the one with RangeAdd. That
// the point tree keeps no second component is read off what its constructor allocates. The facts
// after the contract are the point tree's own worked examples.
public sealed partial class FenwickTreeTests
{
    private const string Sum = nameof(Addition<int>);
    private const string Xor = nameof(BitwiseXor<int>);
    private const string RangeSum = nameof(RangeFenwickTree<int, Addition<int>>);
    private const string RangeSumOfLongs = $"{RangeSum}OfLongs";

    private const int ScriptLength = 600;
    private const int ScriptSeed = 2_029;
    private const int ValueBound = 50;

    // Not a power of two, so the update tree's walks leave the array at several heights.
    private const int ScriptElementCount = 37;

    // One update in this many is a point update in the range script, the rest ranges.
    private const int PointUpdateOdds = 3;

    private static readonly int[] AscendingFive = [1, 2, 3, 4, 5];

    // Each configuration by name, built from its initial elements; the range configurations' views
    // also take ranges.
    private static readonly Dictionary<string, Func<int[], ITreeView>> Trees = new()
    {
        [Sum] = initial => new PointTreeView<Addition<int>>(
            new FenwickTree<int, Addition<int>>(initial)),
        [Xor] = initial => new PointTreeView<BitwiseXor<int>>(
            new FenwickTree<int, BitwiseXor<int>>(initial)),
        [RangeSum] = initial => new RangeTreeView(
            new RangeFenwickTree<int, Addition<int>>(initial)),
        [RangeSumOfLongs] = initial => new LongRangeTreeView(
            new RangeFenwickTree<long, Addition<long>>([.. initial.Select(value => (long)value)])),
    };

    public static TheoryData<string> Configurations => [Sum, Xor, RangeSum, RangeSumOfLongs];

    public static TheoryData<string> RangeConfigurations => [RangeSum, RangeSumOfLongs];

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Count_IsTheInitialLength(string configuration) =>
        Assert.Equal(AscendingFive.Length, TreeOf(configuration, AscendingFive).Count);

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Query_EveryRangeOfAFreshTree_IsTheFoldOfItsElements(string configuration)
    {
        var elements = ScriptElements();
        var tree = TreeOf(configuration, elements);
        var ranges = EveryRange(elements.Length);

        Assert.Equal(
            ranges.Select(range => FoldOf(tree, elements, range.Left, range.Right)),
            ranges.Select(range => tree.Query(range.Left, range.Right)));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void PrefixQuery_EveryIndexOfAFreshTree_IsTheFoldThroughIt(string configuration)
    {
        var elements = ScriptElements();
        var tree = TreeOf(configuration, elements);
        var indices = Enumerable.Range(0, elements.Length).ToList();

        Assert.Equal(
            indices.Select(index => FoldOf(tree, elements, 0, index)),
            indices.Select(tree.PrefixQuery));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Add_SeededScript_EveryQueryAndPrefixAgreesWithTheElements(string configuration)
    {
        var elements = ScriptElements();
        var observations = PointScript(TreeOf(configuration, elements), elements);

        Assert.Equal(observations.Expected, observations.Actual);
    }

    [Theory]
    [MemberData(nameof(RangeConfigurations))]
    public void Query_SeededRangeAddsMixedWithPointAdds_AgreesWithTheElements(string configuration)
    {
        var elements = ScriptElements();
        var observations = RangeScript(RangeTreeOf(configuration, elements), elements);

        Assert.Equal(observations.Expected, observations.Actual);
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Query_OutOfRangeRightInAnyConfiguration_ThrowsArgumentOutOfRangeException(
        string configuration) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TreeOf(configuration, AscendingFive).Query(0, AscendingFive.Length));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void PrefixQuery_IndexAtCount_ThrowsArgumentOutOfRangeException(string configuration) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TreeOf(configuration, AscendingFive).PrefixQuery(AscendingFive.Length));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void PrefixQuery_NegativeIndex_ThrowsArgumentOutOfRangeException(string configuration) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TreeOf(configuration, AscendingFive).PrefixQuery(-1));

    // The point tree keeps one element per node and the range configuration a pair, so what the
    // point tree allocates must come to less than the range configuration's second components plus
    // the few small objects around the arrays.
    [Fact]
    public void Constructor_PointConfiguration_AllocatesNoSecondComponent()
    {
        var pointBytes = Allocations.BytesOf(
            () => new FenwickTree<long, Addition<long>>(Allocation.ElementCount));
        var rangeBytes = Allocations.BytesOf(
            () => new RangeFenwickTree<long, Addition<long>>(Allocation.ElementCount));

        Assert.InRange(pointBytes, 0, rangeBytes - pointBytes + Allocation.NonArrayAllowance);
    }

    [Fact]
    public void Constructor_FromInitialValues_PrefixQueryMatchesRunningSum()
    {
        var tree = new FenwickTree<int, Addition<int>>(AscendingFive);

        Assert.Equal(1, tree.PrefixQuery(0));
        Assert.Equal(Fixtures.PrefixSumThroughIndex1, tree.PrefixQuery(1));
        Assert.Equal(Fixtures.PrefixSumThroughLastIndex, tree.PrefixQuery(Fixtures.LastIndex));
    }

    [Fact]
    public void Query_SumOperation_ReturnsRangeSum()
    {
        var tree = new FenwickTree<int, Addition<int>>(AscendingFive);

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
        var tree = new FenwickTree<int, Addition<int>>(AscendingFive);

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
        var tree = new FenwickTree<int, BitwiseXor<int>>(AscendingFive);

        var xorOfFirstThreeElements = tree.Query(0, Fixtures.FirstThreeRangeEnd);
        Assert.Equal(Fixtures.XorOfFirstThreeElements, xorOfFirstThreeElements);

        var xorOfLastTwoElements = tree.Query(Fixtures.LastTwoRangeStart, Fixtures.LastTwoRangeEnd);
        Assert.Equal(Fixtures.XorOfLastTwoElements, xorOfLastTwoElements);
    }

    [Fact]
    public void Constructor_SizeOnly_StartsAtIdentityEverywhere()
    {
        var tree = new FenwickTree<int, Addition<int>>(Fixtures.SizeOnlyTreeSize);

        var sumOfAllElements = tree.Query(0, Fixtures.LastIndex);
        Assert.Equal(0, sumOfAllElements);
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var tree = new FenwickTree<int, Addition<int>>(Fixtures.CountTreeSize);

        Assert.Equal(Fixtures.CountTreeSize, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new FenwickTree<int, Addition<int>>(AscendingFive);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, Fixtures.OutOfRangeIndex));
    }

    [Fact]
    public void Add_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new FenwickTree<int, Addition<int>>(AscendingFive);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Add(Fixtures.OutOfRangeIndex, Fixtures.Delta));
    }

    // Point updates at seeded positions, each followed by a seeded range query and a prefix.
    private static Observations PointScript(ITreeView tree, int[] elements)
    {
        var random = new Random(ScriptSeed);
        var observations = new Observations();

        for (var step = 0; step < ScriptLength; step++)
        {
            var index = random.Next(elements.Length);
            var delta = random.Next(-ValueBound, ValueBound);
            tree.AddAt(index, delta);
            elements[index] = tree.Combine(elements[index], delta);

            observations.Record(tree, elements, random);
        }

        return observations;
    }

    // Range updates mixed with point updates, two to one, each followed by a seeded range query and
    // a prefix.
    private static Observations RangeScript(IRangeTreeView tree, int[] elements)
    {
        var random = new Random(ScriptSeed);
        var observations = new Observations();

        for (var step = 0; step < ScriptLength; step++)
        {
            ApplySeededUpdate(tree, elements, random);

            observations.Record(tree, elements, random);
        }

        return observations;
    }

    private static void ApplySeededUpdate(IRangeTreeView tree, int[] elements, Random random)
    {
        var (left, right) = SeededRange(random, elements.Length);
        var delta = random.Next(-ValueBound, ValueBound);
        var isPointUpdate = random.Next(PointUpdateOdds) == 0;

        if (isPointUpdate)
        {
            tree.AddAt(left, delta);
            elements[left] = tree.Combine(elements[left], delta);
            return;
        }

        tree.RangeAdd(left, right, delta);

        for (var index = left; index <= right; index++)
        {
            elements[index] = tree.Combine(elements[index], delta);
        }
    }

    private static (int Left, int Right) SeededRange(Random random, int length)
    {
        var left = random.Next(length);

        return (left, random.Next(left, length));
    }

    private static List<(int Left, int Right)> EveryRange(int length) =>
        [.. Enumerable.Range(0, length)
            .SelectMany(left => Enumerable.Range(left, length - left)
                .Select(right => (left, right)))];

    private static int[] ScriptElements()
    {
        var random = new Random(ScriptSeed);

        return [.. Enumerable.Range(0, ScriptElementCount)
            .Select(_ => random.Next(-ValueBound, ValueBound))];
    }

    // The group's fold of elements[left..right], in index order: what a query must answer.
    private static int FoldOf(ITreeView tree, int[] elements, int left, int right)
    {
        var combined = tree.Identity;

        for (var index = left; index <= right; index++)
        {
            combined = tree.Combine(combined, elements[index]);
        }

        return combined;
    }

    private static ITreeView TreeOf(string configuration, int[] initial) =>
        Trees[configuration](initial);

    private static IRangeTreeView RangeTreeOf(string configuration, int[] initial) =>
        (IRangeTreeView)TreeOf(configuration, initial);

    // What the contract reads of any configuration: the operations every one has, a point update as
    // each spells it, and the group a query folds with.
    private interface ITreeView
    {
        int Count { get; }

        int Identity { get; }

        int Query(int left, int right);

        int PrefixQuery(int index);

        void AddAt(int index, int delta);

        int Combine(int left, int right);
    }

    // A configuration that also adds a delta to a whole range.
    private interface IRangeTreeView : ITreeView
    {
        void RangeAdd(int left, int right, int delta);
    }

    private sealed class PointTreeView<TOperation>(FenwickTree<int, TOperation> tree) : ITreeView
        where TOperation : struct, IAbelianGroup<int>
    {
        public int Count => tree.Count;

        public int Identity => TOperation.Identity;

        public int Query(int left, int right) => tree.Query(left, right);

        public int PrefixQuery(int index) => tree.PrefixQuery(index);

        public void AddAt(int index, int delta) => tree.Add(index, delta);

        public int Combine(int left, int right) => TOperation.Combine(left, right);
    }

    private sealed class RangeTreeView(RangeFenwickTree<int, Addition<int>> tree) : IRangeTreeView
    {
        public int Count => tree.Count;

        public int Identity => Addition<int>.Identity;

        public int Query(int left, int right) => tree.Query(left, right);

        public int PrefixQuery(int index) => tree.PrefixQuery(index);

        public void AddAt(int index, int delta) => tree.RangeAdd(index, index, delta);

        public void RangeAdd(int left, int right, int delta) => tree.RangeAdd(left, right, delta);

        public int Combine(int left, int right) => Addition<int>.Combine(left, right);
    }

    // The range configuration over longs, read back as ints: every sum the scripts reach fits.
    private sealed class LongRangeTreeView(RangeFenwickTree<long, Addition<long>> tree)
        : IRangeTreeView
    {
        public int Count => tree.Count;

        public int Identity => Addition<int>.Identity;

        public int Query(int left, int right) => checked((int)tree.Query(left, right));

        public int PrefixQuery(int index) => checked((int)tree.PrefixQuery(index));

        public void AddAt(int index, int delta) => tree.RangeAdd(index, index, delta);

        public void RangeAdd(int left, int right, int delta) => tree.RangeAdd(left, right, delta);

        public int Combine(int left, int right) => Addition<int>.Combine(left, right);
    }

    // Each query a script makes, beside the plain array's fold it must equal.
    private sealed class Observations
    {
        public List<int> Expected { get; } = [];

        public List<int> Actual { get; } = [];

        // A seeded range query, and the prefix through that range's right end.
        public void Record(ITreeView tree, int[] elements, Random random)
        {
            var (left, right) = SeededRange(random, elements.Length);

            Expected.Add(FoldOf(tree, elements, left, right));
            Actual.Add(tree.Query(left, right));
            Expected.Add(FoldOf(tree, elements, 0, right));
            Actual.Add(tree.PrefixQuery(right));
        }
    }

    // The allocation fact's sizes.
    private static class Allocation
    {
        // Large enough that a second component per node, eight bytes each, outweighs the allowance
        // below many times over.
        public const int ElementCount = 1_024;

        // Room for the tree objects and their arrays' headers, far below ElementCount longs.
        public const int NonArrayAllowance = 512;
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
