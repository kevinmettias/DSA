namespace DSAExperimentation.LeetCode.LongestSubstringOfOneRepeatingCharacter;

// One contiguous range of LC 2213's string, summarized with exactly what
// RunAggregate needs to merge two adjacent ranges: the character sitting on each
// edge, how long the range is, how far the run touching each edge reaches inward,
// and the longest run seen anywhere inside. Len == 0 marks the empty range -
// RunAggregate.Identity - and is the one case where Left/Right carry no real
// character. This answers LC 2213 alone, which is why it lives beside the solution
// rather than in Domain (ARCHITECTURE.md section 17.3), the same placement
// PeakGapNode and LisAggregate already have.
internal readonly record struct RunSegment(char Left, char Right, int Len, int PrefixLen, int SuffixLen, int MaxLen)
{
    // The neutral element: an empty range contributes nothing to either side it is
    // combined with, the same shape MaxOperation<Element>.Identity has.
    public static RunSegment Empty => new('\0', '\0', Len: 0, PrefixLen: 0, SuffixLen: 0, MaxLen: 0);

    // A single character is a range of length one whose prefix run, suffix run and
    // best run are all that one character.
    public static RunSegment Leaf(char character) =>
        new(character, character, Len: 1, PrefixLen: 1, SuffixLen: 1, MaxLen: 1);
}
