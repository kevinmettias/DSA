using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.LinkedListInBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListInBinaryTree;

// Harness only. Both downward-path searches are LinkedListInBinaryTreeSolution's -
// this file pins them to LeetCode's three published examples plus the two smaller
// trees the original test used. Trees arrive in LeetCode's own level-order-with-null
// array shape and lists as plain value arrays, because BinaryTreeNode<int> and
// SinglyLinkedListNode<int> are internal and cannot appear in a public TheoryData
// signature; LeetCodeWireFormat.ToBinaryTree and BuildList reconstruct them.
public sealed partial class LinkedListInBinaryTreeTests
{
    // LC 1367's own example tree, shared by its three published cases.
    private static readonly int?[] PublishedTree =
        [1, 4, 4, null, 2, 2, null, 1, null, 6, 8, null, null, null, null, 1, 3];

    // The original test's smaller tree: 1 with children 4 and 5, the 4 carrying 2
    // and 6, and that 6 carrying 8 on its right.
    private static readonly int?[] SmallTree = [1, 4, 5, 2, 6, null, null, null, null, null, 8];

    public static TheoryData<SubPathExample> Examples =>
        new()
        {
            { new SubPathExample(HeadValues: [4, 2, 8], TreeValues: PublishedTree, Expected: true) },
            { new SubPathExample(HeadValues: [1, 4, 2, 6], TreeValues: PublishedTree, Expected: true) },
            { new SubPathExample(HeadValues: [1, 4, 2, 6, 8], TreeValues: PublishedTree, Expected: false) },
            { new SubPathExample(HeadValues: [4, 6, 8], TreeValues: SmallTree, Expected: true) },
            { new SubPathExample(HeadValues: [4, 2, 6], TreeValues: SmallTree, Expected: false) },
            { new SubPathExample(HeadValues: [9], TreeValues: SmallTree, Expected: false) },
            { new SubPathExample(HeadValues: [1], TreeValues: SmallTree, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSubPathByArraySliceWalk_LeetCodeExamples_ReturnsWhetherADownwardPathMatches(
        SubPathExample example)
    {
        var matches = LinkedListInBinaryTreeSolution.IsSubPathByArraySliceWalk(
            BuildList(example.HeadValues), LeetCodeWireFormat.ToBinaryTree(example.TreeValues));

        Assert.Equal(example.Expected, matches);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSubPathByLinkedNodeWalk_LeetCodeExamples_ReturnsWhetherADownwardPathMatches(
        SubPathExample example)
    {
        var matches = LinkedListInBinaryTreeSolution.IsSubPathByLinkedNodeWalk(
            BuildList(example.HeadValues), LeetCodeWireFormat.ToBinaryTree(example.TreeValues));

        Assert.Equal(example.Expected, matches);
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }

    // One example as one argument. The expected answer is a bool, and a bare `true` or
    // `false` sitting third in a row does not say what it is a verdict on; naming the
    // field at each row below does.
    public readonly record struct SubPathExample(int[] HeadValues, int?[] TreeValues, bool Expected);
}
