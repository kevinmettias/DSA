using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

// The one linked node's three configurations, each named and reached through one view, so a
// contract suite runs every fact over all of them: what a fact may ask of a view is what the
// forward-linked node, LinkedNode<TNode, TValue>, gives every configuration. Nodes hold ints, and
// a view hands them out as objects so that one test body serves all three types. A view keeps no
// state, so the tests share one per configuration.
internal static class LinkedNodeConfigurations
{
    public const string Singly = nameof(SinglyLinkedListNode<int>);
    public const string Doubly = nameof(DoublyLinkedListNode<int>);
    public const string Random = nameof(RandomLinkedListNode<int>);

    private static readonly Dictionary<string, ILinkedNodeView> Views = new()
    {
        [Singly] = new LinkedNodeView<SinglyLinkedListNode<int>>(value => new(value)),
        [Doubly] = new LinkedNodeView<DoublyLinkedListNode<int>>(value => new() { Value = value }),
        [Random] = new LinkedNodeView<RandomLinkedListNode<int>>(value => new(value)),
    };

    public static ILinkedNodeView Of(string configuration) => Views[configuration];
}
