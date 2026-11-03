using DSAExperimentation.Algorithms.Reducing;
using PrefixNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrieNode<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    int>;

namespace DSAExperimentation.LeetCode.Solutions.LongestCommonPrefixOfKStringsAfterRemoval;

// LC 3485 alone: "how many words share this exact prefix" is a post-order subtree
// sum over a trie of every word's own multiplicity (the node's Value,
// set to how many times that exact string occurs - see
// LongestCommonPrefixOfKStringsAfterRemovalSolution.BuildTrie) plus every child's
// already-computed sum. Exit fires after all children (DepthFirstWalk.cs), so by
// the time a node's own Exit runs, every child already has an entry in state - the
// same post-order-accumulate-into-a-threaded-dictionary shape
// DistanceMapReduceAlgebra uses for depth, just combining children instead of only
// recording the node itself.
internal readonly struct SubtreeWordCountAlgebra
    : IReduceAlgebra<PrefixNode, Dictionary<PrefixNode, int>>
{
    public static Dictionary<PrefixNode, int> Seed => [];

    public static Dictionary<PrefixNode, int> Exit(
        Dictionary<PrefixNode, int> state, PrefixNode node, int depth)
    {
        var count = node.HasValue ? node.Value : 0;

        for (var rank = 0; rank < node.ChildCount; rank++)
        {
            count += state[node.ChildOfRank(rank)];
        }

        state[node] = count;
        return state;
    }
}
