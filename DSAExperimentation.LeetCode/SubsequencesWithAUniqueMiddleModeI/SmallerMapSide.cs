namespace DSAExperimentation.LeetCode.SubsequencesWithAUniqueMiddleModeI;

// Which side of the middle index the smaller of the two frequency maps turned out to be
// on. FrequencyMapsByAscendingCount orders the maps by how many distinct values they
// hold, so a count read from the smaller one has to be handed back in (left, right)
// order - a choice that reads as a name here and as a bare `true` at the call site.
internal enum SmallerMapSide
{
    // The smaller map is the left side, so its count is the left count.
    Left,

    // The smaller map is the right side, so its count is the right count.
    Right,
}
