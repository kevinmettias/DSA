namespace DSAExperimentation.DataStructures.AhoCorasick;

// The [Start, Start + patterns[PatternIndex].Length) span in the scanned text where pattern
// patterns[PatternIndex] (the caller's own input sequence, 0-based) occurs. No Length field: the
// caller already has it from their own pattern list, the same anti-redundancy shape
// RollingHashSearch.FindAll's bare int starts already use.
/// <summary>One occurrence of a pattern in a text, as <see cref="AhoCorasick.FindAll"/> reports it.</summary>
/// <param name="Start">The 0-based position in the text where the occurrence begins.</param>
/// <param name="PatternIndex">
/// The matched pattern's 0-based position in the sequence the automaton was built from.
/// </param>
public readonly record struct AhoCorasickMatch(int Start, int PatternIndex);
