using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.FrequenciesOfShortestSupersequences;

// A letter-precedence graph is not a DAG in general - that is exactly the question
// being asked (does doubling this subset break every cycle?) - so this only
// promises IGraphTopology, the same reasoning LockTopology's own doc comment gives.
internal readonly struct LetterTopology : IGraphTopology<LetterNode, ListChildren<LetterNode>>
{
    public static ListChildren<LetterNode> GetChildren(LetterNode node) => new(node.Neighbors);
}
