using DSAExperimentation.DataStructures.SinglyLinkedList;
namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteNodeInALinkedList;
public sealed partial class DeleteNodeInALinkedListTests { [Fact] public void DeleteNode_CopiesNextValueAndSkipsNext(){var node=new SinglyLinkedListNode<int>(5){Next=new(1){Next=new(9)}};Delete(node);Assert.Equal(1,node.Value);Assert.Equal(9,node.Next!.Value);} private static void Delete(SinglyLinkedListNode<int> node){node.Value=node.Next!.Value;node.Next=node.Next.Next;} }
