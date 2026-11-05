using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Domain.Mathematics.Algebra;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.Solutions.MinimumInversionCountInSubarraysOfFixedLength;

// The rank-compressed index the sliding-window sweep works over: the compressed
// coordinates of nums to rank values against, and a Fenwick tree sized to those
// ranks that tracks which ranks are currently in the window.
// The two were returned as an unnamed pair - the types keep a caller from
// swapping them, but nothing said which one MEANS which.
internal readonly record struct RankIndex(
    FenwickTree<int, Addition<int>> Tree, CompressedCoordinates<int> Coordinates);
