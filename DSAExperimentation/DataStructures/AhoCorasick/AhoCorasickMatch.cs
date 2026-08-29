namespace DSAExperimentation.DataStructures.AhoCorasick;

// The [Start, Start + patterns[PatternIndex].Length) span in the scanned text where pattern
// patterns[PatternIndex] (the caller's own input sequence, 0-based) occurs. No Length field: the
// caller already has it from their own pattern list, the same anti-redundancy shape
// RollingHashSearch.FindAll's bare int starts already use.
internal readonly record struct AhoCorasickMatch(int Start, int PatternIndex);
