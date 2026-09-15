namespace DSAExperimentation.LeetCode.UglyNumberIII;

// The four LCMs the counting rule's inclusion-exclusion divides by - the three
// pairwise ones and the three-way one. They are one concept derived from the three
// factors, and the count reads all four together and nothing else, so they travel
// as a unit rather than as four loose fields beside the factors themselves.
internal readonly record struct SubsetCommonMultiples(long LcmAb, long LcmAc, long LcmBc, long LcmAbc);
