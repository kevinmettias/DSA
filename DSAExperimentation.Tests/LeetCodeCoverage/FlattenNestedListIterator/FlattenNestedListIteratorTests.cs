using DSAExperimentation.LeetCode.FlattenNestedListIterator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenNestedListIterator;

// Harness only: both strategies live in FlattenNestedListIteratorSolution. Each
// example is its own [Fact] per strategy rather than a [Theory] - NestedInteger is
// internal, so a List<NestedInteger> cannot appear in a public TheoryData<...>
// member (CS0053), the same reason BinarySearchTreeIteratorTests uses one [Fact]
// per tree rather than [Theory].
public sealed class FlattenNestedListIteratorTests
{
    [Fact]
    public void CreateByLazyStack_ClassicExample_FlattensNestedListInOrder() =>
        Assert.Equal([1, 1, 2, 1, 1], Flatten(FlattenNestedListIteratorSolution.CreateByLazyStack(ClassicExample())));

    [Fact]
    public void CreateByLazyStack_DeeplyNestedList_FlattensEveryDepthInOrder() =>
        Assert.Equal([1, 4, 6], Flatten(FlattenNestedListIteratorSolution.CreateByLazyStack(DeeplyNestedList())));

    [Fact]
    public void CreateByLazyStack_EmptyNestedListsInterspersed_SkipsThemEntirely() =>
        Assert.Equal(
            [1, 2], Flatten(FlattenNestedListIteratorSolution.CreateByLazyStack(EmptyNestedListsInterspersed())));

    [Fact]
    public void CreateByEagerFlatten_ClassicExample_FlattensNestedListInOrder() =>
        Assert.Equal(
            [1, 1, 2, 1, 1], Flatten(FlattenNestedListIteratorSolution.CreateByEagerFlatten(ClassicExample())));

    [Fact]
    public void CreateByEagerFlatten_DeeplyNestedList_FlattensEveryDepthInOrder() =>
        Assert.Equal([1, 4, 6], Flatten(FlattenNestedListIteratorSolution.CreateByEagerFlatten(DeeplyNestedList())));

    [Fact]
    public void CreateByEagerFlatten_EmptyNestedListsInterspersed_SkipsThemEntirely() =>
        Assert.Equal(
            [1, 2], Flatten(FlattenNestedListIteratorSolution.CreateByEagerFlatten(EmptyNestedListsInterspersed())));

    private static List<int> Flatten(FlattenNestedListIteratorSolution.IFlattenIterator iterator)
    {
        var flattened = new List<int>();

        while (iterator.HasNext())
        {
            flattened.Add(iterator.Next());
        }

        return flattened;
    }

    // [[1,1],2,[1,1]]
    private static List<NestedInteger> ClassicExample() =>
        [
            NestedInteger.OfList(NestedInteger.OfInteger(1), NestedInteger.OfInteger(1)),
            NestedInteger.OfInteger(2),
            NestedInteger.OfList(NestedInteger.OfInteger(1), NestedInteger.OfInteger(1)),
        ];

    // [1,[4,[6]]]
    private static List<NestedInteger> DeeplyNestedList() =>
        [
            NestedInteger.OfInteger(1),
            NestedInteger.OfList(NestedInteger.OfInteger(4), NestedInteger.OfList(NestedInteger.OfInteger(6))),
        ];

    // [[],1,[],2,[]]
    private static List<NestedInteger> EmptyNestedListsInterspersed() =>
        [
            NestedInteger.OfList(),
            NestedInteger.OfInteger(1),
            NestedInteger.OfList(),
            NestedInteger.OfInteger(2),
            NestedInteger.OfList(),
        ];
}
