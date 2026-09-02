namespace DSAExperimentation.LeetCode.MinimumCostToBuyApplesII;

// One node per shop. A road contributes to two independent weighted graphs over
// the same vertex set - ForwardEdges (plain cost, for the empty outbound trip)
// and ReturnEdges (cost * tax, for the trip back while carrying apples) - so a
// single IEdgeTopology witness per role, RecoveryNode's own "weight, target" edge
// -list shape, is enough for both.
internal sealed class AppleNode(int id)
{
    public int Id { get; } = id;

    public List<(long Weight, AppleNode Target)> ForwardEdges { get; } = [];

    public List<(long Weight, AppleNode Target)> ReturnEdges { get; } = [];

    public override string ToString() => Id.ToString();
}
