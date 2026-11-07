namespace DSAExperimentation.DataStructures.LinkedList;

// The one linked-list node: a value and a link to the next node, which every list has, so the
// forward contract is this class. The singly linked node is exactly this, with no other field; a
// configuration that links to more declares the link it adds as a property of its own, the doubly
// linked node Previous and the random-pointer node Random, so the JIT keeps each a field apart
// from Next and each access stays an auto-property's. The configurations derive with TNode fixed to
// themselves, so Next is typed as the configuration, and the constructors only this assembly can
// call keep the set of them to the ones this library defines (ARCHITECTURE.md §13.11).
/// <summary>A node of a linked list: a value and a link to the next node.</summary>
/// <typeparam name="TNode">The configuration deriving from this class.</typeparam>
/// <typeparam name="TValue">The value the node holds.</typeparam>
public abstract class LinkedNode<TNode, TValue>
    where TNode : LinkedNode<TNode, TValue>
{
    // presumption: allow -- a node built without a value (the doubly linked node, filled by an
    // object initializer, and a list's sentinels) starts with the default, as an array slot does;
    // the compiler emits no store for a default initializer.
    /// <summary>Gets or sets the value the node holds.</summary>
    public TValue Value { get; set; } = default!;

    /// <summary>Gets or sets the next node, or null when this node is the last.</summary>
    public TNode? Next { get; set; }

    private protected LinkedNode()
    {
    }

    private protected LinkedNode(TValue value) => Value = value;
}
