using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LongestCommonPrefixOfKStringsAfterRemoval;

// LC 3485 alone: "how many words share this exact prefix" is a post-order subtree
// sum over a trie of every word's own multiplicity (LowercaseTrieNode<int>.Value,
// set to how many times that exact string occurs - see
// LongestCommonPrefixOfKStringsAfterRemovalSolution.BuildTrie) plus every child's
// already-computed sum. Exit fires after all children (DepthFirstWalk.cs), so by
// the time a node's own Exit runs, every child already has an entry in state - the
// same post-order-accumulate-into-a-threaded-dictionary shape
// DistanceMapReduceAlgebra uses for depth, just combining children instead of only
// recording the node itself.
internal readonly struct SubtreeWordCountAlgebra
    : IReduceAlgebra<LowercaseTrieNode<int>, Dictionary<LowercaseTrieNode<int>, int>>
{
    public static Dictionary<LowercaseTrieNode<int>, int> Seed => [];

    public static Dictionary<LowercaseTrieNode<int>, int> Exit(
        Dictionary<LowercaseTrieNode<int>, int> state, LowercaseTrieNode<int> node, int depth)
    {
        var count = node.HasValue ? node.Value : 0;

        foreach (var child in node.Children)
        {
            if (child is not null)
            {
                count += state[child];
            }
        }

        state[node] = count;
        return state;
    }
}
