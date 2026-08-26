namespace DSAExperimentation.Algorithms;

// Shared tuning knobs for algorithms that halve a range or branch a complete binary tree
// (BinarySearch, MergeSort, Heap, SegmentTree) - not a shared Representation type, just the bare
// numeric constants each would otherwise redeclare identically. HalvingFactor and BranchingFactor
// happen to share a value (2) but not a meaning - one is the divisor a range is split by, the
// other is how many children a node has - so they stay two named constants, not one reused under
// two names.
internal static class AlgorithmConstants
{
    public const int HalvingFactor = 2;
    public const int BranchingFactor = 2;
}
