using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeSubarrays;

// The prepared input MinOperationsByMergeSortTree's hoisted overload takes: one
// merge-sort tree over the whole array, plus the run-id every query's [l, r] gets
// checked against. The two members are unrelated types, so only their names say
// which half of the index each one is.
internal readonly record struct EqualizeIndex(SegmentTree<int[], SortedMergeOperation> Tree, int[] RunId);
