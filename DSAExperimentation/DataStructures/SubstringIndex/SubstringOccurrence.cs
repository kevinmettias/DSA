namespace DSAExperimentation.DataStructures.SubstringIndex;

// A substring named by where it sits in the indexed text, not by a copy of its characters: the
// caller already holds the text, and a copy would cost an allocation the index never needs.
/// <summary>One place a substring occurs in an indexed text.</summary>
/// <param name="Start">The 0-based position in the text where the substring begins.</param>
/// <param name="Length">How many characters the substring spans.</param>
public readonly record struct SubstringOccurrence(int Start, int Length)
{
    // Built where it is read rather than kept in a static field: inlined into LC 1044's arm, a read
    // of the field cost a call to the static-base helper, where this folds to two zeros.
    /// <summary>
    /// Gets the empty substring at the text's start, the answer when there is no other.
    /// </summary>
    public static SubstringOccurrence Empty => new(0, 0);
}
