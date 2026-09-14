namespace DSAExperimentation.LeetCode.DesignGraphWithShortestPathCalculator;

// LC 2642's own directed, non-negative-weight vertex, kept local to this problem
// folder for the same reason NetworkDelayTime's NetworkNode is: the general
// "weighted graph node" fixture role is already filled several times over for
// still-unmigrated problems, and reaching across into another problem's folder
// would couple two solutions that share nothing but a shape.
//
// The edge list is mutable because AddEdge is part of LeetCode's own API here -
// the graph grows between queries, which is exactly why no query result may be
// cached.
internal sealed class ShortestPathCalculatorNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, ShortestPathCalculatorNode Target)> Edges { get; } = [];
}
