using DSAExperimentation.LeetCode.FlattenNestedListIterator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenNestedListIterator;

// Harness only: both strategies live in FlattenNestedListIteratorSolution and are
// replayed over the same LeetCode examples, so a disagreement between them fails
// here rather than surfacing only as a benchmark/test mismatch. NestedInteger is
// internal, so a List<NestedInteger> cannot appear in a public TheoryData<...>
// member (CS0053) - the constraint BinarySearchTreeIteratorTests answers with one
// [Fact] per tree. Here each example is instead named by a nested NestedListShape
// the theory data carries, and BuildList turns that shape into the structure inside
// the test.
public sealed class FlattenNestedListIteratorTests
{
    public static TheoryData<NestedListShape, List<int>> Examples =>
        new()
        {
            { NestedListShape.Classic, [1, 1, 2, 1, 1] },
            { NestedListShape.DeeplyNested, [1, 4, 6] },
            { NestedListShape.EmptyNestedListsInterspersed, [1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLazyStack_LeetCodeExamples_FlattensNestedListInOrder(NestedListShape shape, List<int> expected)
    {
        var flattened = Flatten(FlattenNestedListIteratorSolution.CreateByLazyStack(BuildList(shape)));

        Assert.Equal(expected, flattened);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByEagerFlatten_LeetCodeExamples_FlattensNestedListInOrder(NestedListShape shape, List<int> expected)
    {
        var flattened = Flatten(FlattenNestedListIteratorSolution.CreateByEagerFlatten(BuildList(shape)));

        Assert.Equal(expected, flattened);
    }

    private static List<int> Flatten(FlattenNestedListIteratorSolution.IFlattenIterator iterator)
    {
        var flattened = new List<int>();

        while (iterator.HasNext())
        {
            flattened.Add(iterator.Next());
        }

        return flattened;
    }

    // The three LeetCode examples, keyed by shape. NestedInteger is internal, so the
    // shape is the public stand-in the theory data can carry and BuildList does the
    // reconstruction the theories cannot spell out themselves.
    private static List<NestedInteger> BuildList(NestedListShape shape) => shape switch
    {
        NestedListShape.Classic => ClassicExample(),
        NestedListShape.DeeplyNested => DeeplyNestedList(),
        _ => EmptyNestedListsInterspersed(),
    };

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

    // Which of LeetCode 341's own nested-list examples a theory row replays, named
    // rather than spelled out in the theory data: a List<NestedInteger> is internal,
    // so each example is selected by name and rebuilt by BuildList inside the test.
    // Public because it appears in the theories' own signatures.
    public enum NestedListShape
    {
        Classic,
        DeeplyNested,
        EmptyNestedListsInterspersed,
    }
}
