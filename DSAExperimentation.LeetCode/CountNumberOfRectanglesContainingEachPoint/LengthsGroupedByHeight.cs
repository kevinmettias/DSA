using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountNumberOfRectanglesContainingEachPoint;

// One rectangle set's sorting work, handed back whole: every distinct height in
// ascending order, and beside it that height's rectangle lengths, each list sorted.
// GroupSortedLengthsByHeight returns this so its caller names the two halves - both
// members are collections, and nothing else in the signature would say which is which.
internal readonly record struct LengthsGroupedByHeight(int[] Heights, HashMap<int, int[]> LengthsByHeight);
