namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// What a walk does to each node on a key's path, the root first, for a payload kept along the path
// rather than at a key's end: a count of the values passing through each node goes up by one as a
// value goes in and down by one as it comes out. A step is a struct with a static method, so the
// walk is compiled for each step and calls it directly, and it is written for one node class, so
// the call is exact even where the walk's own code is shared across node classes.
/// <summary>What a trie walk does to each node on a key's path.</summary>
/// <typeparam name="TNode">The node class the step changes.</typeparam>
public interface ITrieStep<TNode>
    where TNode : class
{
    /// <summary>Applies the step to one node on the path.</summary>
    /// <param name="node">The node the walk has reached.</param>
    static abstract void Apply(TNode node);
}
