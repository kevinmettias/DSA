namespace DSAExperimentation.LeetCode.SubsequencesWithAUniqueMiddleModeI;

// The two sides' frequency maps ordered by how many distinct values each holds, so the
// pairwise walk over the values they share always iterates the smaller of the two. The
// names carry the ordering the maps arrive in: `Smaller` is the map that is cheaper to
// walk, `Larger` the map it is matched against. A bare pair of dictionaries left the
// caller to tell them apart by their types - which are the same - or by position.
internal readonly record struct FrequencyMapsByAscendingCount(
    Dictionary<int, int> Smaller,
    Dictionary<int, int> Larger);
