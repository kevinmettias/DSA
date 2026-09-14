using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumReverseOperations;

// The shared, read-only context a PositionNode carries a reference to - Length and K
// describe the array and reversal-window size, the banned set holds the forbidden
// destinations. The same "node carries a reference to its own adjacency context"
// shape DataStructures.Graph.Grids' Grid already established for GridNode/
// GridChildren, just for a 1-D array of positions instead of a 2-D passable grid.
//
// Doubles as the prepared input of #17.4's hoisted overload: it is not IEnumerable,
// so MinOperationsByReduceGraph's two overloads can never be ambiguous.
internal sealed class ReversalBoard(int length, int k, Set<int> banned)
{
    public int Length { get; } = length;

    public int K { get; } = k;

    public bool IsBanned(int position) => banned.Has(position);
}
