namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A store whose children have an order, their letters', so a child can be fetched by its rank in it.
// That order is what a tree engine needs to sweep a node's children from Get(0) to Get(Count - 1), so
// only a trie over an ordered store has a tree-topology witness (TrieTopology). A hashed store keeps no
// order, and the compiler refuses it one rather than letting an engine sweep an order that is not there.
/// <summary>A trie child store whose children are ordered, so each can be fetched by its rank.</summary>
/// <typeparam name="TSelf">The store type itself.</typeparam>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
public interface IOrderedTrieChildStore<TSelf, TValue> : ITrieChildStore<TSelf, TValue>
    where TSelf : struct, IOrderedTrieChildStore<TSelf, TValue>
{
    /// <summary>Gets how many children the node has.</summary>
    int Count { get; }

    /// <summary>Fetches the child of a given rank in the store's order.</summary>
    /// <param name="rank">The child's rank, from 0 below <see cref="Count"/>.</param>
    /// <returns>The child of that rank.</returns>
    TrieNode<TSelf, TValue> ChildOfRank(int rank);
}
