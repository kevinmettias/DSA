using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Dags;

namespace DSAExperimentation.Graph.Engines.Dags.Trees;

// A refinement of IDagTopology, not a restatement: implementing this additionally
// promises unique ancestry (no sharing) on top of the acyclicity IDagTopology
// already promises - exactly what lets the tree-only walkers (DepthFirstWalk,
// RecursiveFoldEvaluation, etc.) skip the visited-tracking/memoization that
// IDagTopology/IGraphTopology-based algorithms need, and stay zero-cost. Any
// ITreeTopology is automatically usable wherever an IDagTopology or IGraphTopology
// is expected; never the reverse.
internal interface ITreeTopology<TNode, TChildren> : IDagTopology<TNode, TChildren>
    where TNode : class
    where TChildren : struct, IChildren<TNode>
{
}
