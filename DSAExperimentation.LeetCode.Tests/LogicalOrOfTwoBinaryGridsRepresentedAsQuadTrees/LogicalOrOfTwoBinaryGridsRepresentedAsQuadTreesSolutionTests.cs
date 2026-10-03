using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructQuadTree;
using DSAExperimentation.LeetCode.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

namespace DSAExperimentation.LeetCode.Tests.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

// LeetCode 558. Logical Or of Two Binary Grids Represented as Quad-Trees: harness
// only. All three strategies are
// LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution's. LeetCode's published
// examples are stated in its own serialized quad-tree format and the result is
// compared with the published output tree. The added grid examples build the two
// input quad-trees from grids via ConstructQuadTreeSolution (LC 427), run one OR
// strategy, and check that decoding the result tree back to a grid reproduces the
// expected elementwise OR exactly.
public sealed partial class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolutionTests
{
    // Every node present in LeetCode's quad-tree format owns this many child slots.
    private const int QuadrantCount = 4;

    private const string MissingQuadrant = "a non-leaf QuadTreeNode carries all four children";

    // LeetCode's examples, verbatim. The two grid-materializing strategies also need the
    // grid's side, which LeetCode's input leaves implicit: example 1's quadTree2 is two
    // levels deep, so 4 x 4 is the smallest grid it fits (any larger power of two
    // describes the same trees), and example 2 says each tree is a 1 x 1 matrix.
    public static TheoryData<SerializedExample> PublishedExamples =>
        new()
        {
            {
                new SerializedExample(
                    QuadTree1: [[0, 1], [1, 1], [1, 1], [1, 0], [1, 0]],
                    QuadTree2: [[0, 1], [1, 1], [0, 1], [1, 1], [1, 0], null, null, null, null, [1, 0], [1, 0], [1, 1], [1, 1]],
                    Size: 4,
                    Expected: [[0, 0], [1, 1], [1, 1], [1, 1], [1, 0]])
            },
            { new SerializedExample(QuadTree1: [[1, 0]], QuadTree2: [[1, 0]], Size: 1, Expected: [[1, 0]]) },
        };

    public static TheoryData<int[][], int[][], int[][]> Examples =>
        new()
        {
            { [[1, 1], [1, 1]], [[0, 0], [0, 0]], [[1, 1], [1, 1]] },
            { [[1, 0], [0, 1]], [[0, 1], [1, 0]], [[1, 1], [1, 1]] },
            {
                [
                    [1, 1, 0, 0],
                    [1, 1, 0, 0],
                    [0, 0, 0, 0],
                    [0, 0, 0, 0],
                ],
                [
                    [0, 0, 0, 0],
                    [0, 0, 0, 0],
                    [0, 0, 1, 0],
                    [0, 0, 0, 1],
                ],
                [
                    [1, 1, 0, 0],
                    [1, 1, 0, 0],
                    [0, 0, 1, 0],
                    [0, 0, 0, 1],
                ]
            },
        };

    [Theory]
    [MemberData(nameof(PublishedExamples))]
    public void OrByDirectRecursiveMerge_PublishedExamples_ReturnsThePublishedTree(SerializedExample example)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByDirectRecursiveMerge(
            Decode(example.QuadTree1), Decode(example.QuadTree2));

        AssertSameTree(Decode(example.Expected), merged);
    }

    [Theory]
    [MemberData(nameof(PublishedExamples))]
    public void OrByBruteForceGridMaterialize_PublishedExamples_ReturnsThePublishedTree(SerializedExample example)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByBruteForceGridMaterialize(
            Decode(example.QuadTree1), Decode(example.QuadTree2), example.Size);

        AssertSameTree(Decode(example.Expected), merged);
    }

    [Theory]
    [MemberData(nameof(PublishedExamples))]
    public void OrByFenwickGridMaterialize_PublishedExamples_ReturnsThePublishedTree(SerializedExample example)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByFenwickGridMaterialize(
            Decode(example.QuadTree1), Decode(example.QuadTree2), example.Size);

        AssertSameTree(Decode(example.Expected), merged);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrByDirectRecursiveMerge_LeetCodeExamples_ReconstructsElementwiseOrOfBothGrids(
        int[][] grid1, int[][] grid2, int[][] expected)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByDirectRecursiveMerge(Build(grid1), Build(grid2));

        AssertReconstructsExpectedGrid(merged, expected);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrByBruteForceGridMaterialize_LeetCodeExamples_ReconstructsElementwiseOrOfBothGrids(
        int[][] grid1, int[][] grid2, int[][] expected)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByBruteForceGridMaterialize(
            Build(grid1), Build(grid2), grid1.Length);

        AssertReconstructsExpectedGrid(merged, expected);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrByFenwickGridMaterialize_LeetCodeExamples_ReconstructsElementwiseOrOfBothGrids(
        int[][] grid1, int[][] grid2, int[][] expected)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByFenwickGridMaterialize(
            Build(grid1), Build(grid2), grid1.Length);

        AssertReconstructsExpectedGrid(merged, expected);
    }

    private static QuadTreeNode Build(int[][] grid) => ConstructQuadTreeSolution.BuildByBruteForceCellScan(grid);

    private static void AssertReconstructsExpectedGrid(QuadTreeNode result, int[][] expected) =>
        Assert.Equal(expected, QuadTreeGrid.Materialize(result, expected.Length));

    // LeetCode's quad-tree format: level order, one [isLeaf, val] pair per node, null
    // where no node is. Every node present owns the next four slots for its TopLeft,
    // TopRight, BottomLeft and BottomRight children - four nulls under a leaf - and
    // trailing nulls are trimmed, so the k-th node present has its children at slots
    // 4k + 1 to 4k + 4: the four-way form of the 2k + 1, 2k + 2 rule
    // LeetCodeWireFormat.ToBinaryTree reads. QuadTreeNode's quadrants are init-only, so
    // each node's first child slot is found first and the tree is then built top-down.
    private static QuadTreeNode Decode(int[]?[] levelOrder)
    {
        var firstChildSlots = new int[levelOrder.Length];
        var nodesSeen = 0;

        for (var slot = 0; slot < levelOrder.Length; slot++)
        {
            if (levelOrder[slot] is not null)
            {
                firstChildSlots[slot] = (QuadrantCount * nodesSeen) + 1;
                nodesSeen++;
            }
        }

        return DecodeAt(levelOrder, firstChildSlots, 0);
    }

    private static QuadTreeNode DecodeAt(int[]?[] levelOrder, int[] firstChildSlots, int slot)
    {
        if (levelOrder[slot] is not [var isLeaf, var value])
        {
            throw new InvalidOperationException(
                $"slot {slot} is a child of a non-leaf node, so it must hold an [isLeaf, val] pair");
        }

        var node = new QuadTreeNode(Val: value == 1, IsLeaf: isLeaf == 1);

        return node.IsLeaf ? node : WithQuadrants(node, levelOrder, firstChildSlots, firstChildSlots[slot]);
    }

    private static QuadTreeNode WithQuadrants(
        QuadTreeNode node, int[]?[] levelOrder, int[] firstChildSlots, int firstChildSlot) =>
        node with
        {
            TopLeft = DecodeAt(levelOrder, firstChildSlots, firstChildSlot + (int)QuadrantSlot.TopLeft),
            TopRight = DecodeAt(levelOrder, firstChildSlots, firstChildSlot + (int)QuadrantSlot.TopRight),
            BottomLeft = DecodeAt(levelOrder, firstChildSlots, firstChildSlot + (int)QuadrantSlot.BottomLeft),
            BottomRight = DecodeAt(levelOrder, firstChildSlots, firstChildSlot + (int)QuadrantSlot.BottomRight),
        };

    // LeetCode accepts any val on a node that is not a leaf, so two non-leaves match on
    // their four quadrants alone, and two leaves on their value.
    private static void AssertSameTree(QuadTreeNode expected, QuadTreeNode actual)
    {
        Assert.Equal(expected.IsLeaf, actual.IsLeaf);

        if (expected.IsLeaf)
        {
            Assert.Equal(expected.Val, actual.Val);
            return;
        }

        AssertSameTree(Child(expected.TopLeft), Child(actual.TopLeft));
        AssertSameTree(Child(expected.TopRight), Child(actual.TopRight));
        AssertSameTree(Child(expected.BottomLeft), Child(actual.BottomLeft));
        AssertSameTree(Child(expected.BottomRight), Child(actual.BottomRight));
    }

    // Decode and every OR strategy mark a node IsLeaf false only where they also give it
    // all four quadrants, so a missing child of a non-leaf is a broken tree, reported here
    // rather than promised away.
    private static QuadTreeNode Child(QuadTreeNode? child) =>
        child ?? throw new InvalidOperationException(MissingQuadrant);

    // One published example: both inputs and the expected output in LeetCode's serialized
    // form, plus the grid side the materializing strategies take.
    public readonly record struct SerializedExample(int[]?[] QuadTree1, int[]?[] QuadTree2, int Size, int[]?[] Expected);

    // The order LeetCode's format lists a node's four children in: each one's offset from
    // the node's first child slot.
    private enum QuadrantSlot
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
}
