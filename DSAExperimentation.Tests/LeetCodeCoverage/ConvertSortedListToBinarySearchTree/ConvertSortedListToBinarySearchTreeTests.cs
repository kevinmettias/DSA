using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertSortedListToBinarySearchTree;

public sealed partial class ConvertSortedListToBinarySearchTreeTests
{
    [Fact]
    public void SortedListToBST_ClassicExample_PreservesInOrderValues()
    {
        var root = Build(BuildList([-10, -3, 0, 5, 9]));
        Assert.Equal([-10, -3, 0, 5, 9], InOrder(root));
    }

    private static BinaryTreeNode<int>? Build(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>(); for (var node = head; node is not null; node = node.Next) values.Add(node.Value);
        return BuildRange(values, 0, values.Count - 1);
    }
    private static BinaryTreeNode<int>? BuildRange(List<int> values, int low, int high) { if (low > high) return null; var mid = low + ((high - low) / 2); return new BinaryTreeNode<int>(values[mid]) { Left = BuildRange(values, low, mid - 1), Right = BuildRange(values, mid + 1, high) }; }
    private static SinglyLinkedListNode<int>? BuildList(int[] values) { var d = new SinglyLinkedListNode<int>(0); var t = d; foreach (var v in values) { t.Next = new SinglyLinkedListNode<int>(v); t = t.Next; } return d.Next; }
    private static int[] InOrder(BinaryTreeNode<int>? node) => node is null ? [] : [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];
}
