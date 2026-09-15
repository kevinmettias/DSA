namespace DSAExperimentation.DataStructures;

// Shared tuning knobs for algorithms that halve a range or branch a complete binary tree
// (BinarySearch, MergeSort, Heap, SegmentTree) - not a shared Representation type, just the bare
// numeric constants each would otherwise redeclare identically.
//
// HalvingFactor is read by everything that splits at the middle, not only by those four: the
// bisections inside the LeetCode solutions (FindInMountainArray, FruitsIntoBasketsIII,
// KokoEatingBananas, SplitArrayLargestSum and the rest) and the benchmark fixtures that seed a
// generated input at half its length all divide by the same 2 for the same reason, and each
// used to hold a private copy of it under a name of its own choosing.
//
// Lives in DataStructures/, the LOWEST tier that needs it, even though the name says "algorithm":
// HeapArrayIndex and SegmentTree read these, and a tier may only depend downward (ARCHITECTURE.md
// section 17.2, enforced by LayeringTests). Filing it under Algorithms/ made three data structures
// reach up a tier for two integers. HalvingFactor and BranchingFactor
// happen to share a value (2) but not a meaning - one is the divisor a range is split by, the
// other is how many children a node has - so they stay two named constants, not one reused under
// two names.
internal static class AlgorithmConstants
{
    public const int HalvingFactor = 2;
    public const int BranchingFactor = 2;
}
