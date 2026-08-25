using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;

namespace DSAExperimentation.Graph.Engines.Dags;

// A refinement of IGraphTopology, weaker than ITreeTopology: promises acyclicity
// but not unique ancestry - shared descendants are expected and fine, cycles are
// not. This is exactly what a fold needs and exactly what bare IGraphTopology
// doesn't promise, which is why CheckedFold has to accept any IGraphTopology and
// defend itself at runtime. DagFold is the payoff: constrained on this tier, it can
// skip that defense entirely.
internal interface IDagTopology<TNode, TChildren> : IGraphTopology<TNode, TChildren>
    where TNode : class
    where TChildren : struct, IChildren<TNode>
{
}
