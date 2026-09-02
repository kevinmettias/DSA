namespace DSAExperimentation.LeetCode.MinimizeTheMaximumEdgeWeightOfGraph;

// A reference type, not a value type - every topology/walk/reduce contract in this
// library constrains TNode : class (identity-based, matching TrackedVisitGuard's
// HashSet<TNode>), the same reason GridNode is a class despite being small enough
// to want value semantics. MaxWeight travels with the node exactly the way
// GridNode carries its Grid: it is the runtime context GetChildren needs to decide
// which edges currently count as passable.
internal sealed record EdgeWeightNode(int Id, EdgeWeightGraph Graph, int MaxWeight);
