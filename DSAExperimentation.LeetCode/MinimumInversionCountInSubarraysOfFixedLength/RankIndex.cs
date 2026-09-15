using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumInversionCountInSubarraysOfFixedLength;

// The rank-compressed index the sliding-window sweep works over: the sorted
// distinct values of nums as a sequence to rank-compress against, and a Fenwick
// tree sized to those ranks that tracks which ranks are currently in the window.
// The two were returned as an unnamed pair - the types keep a caller from
// swapping them, but nothing said which one MEANS which.
internal readonly record struct RankIndex(
    FenwickTree<int, SumOperation<int>> Tree, ArraySequence<int> Sequence);
