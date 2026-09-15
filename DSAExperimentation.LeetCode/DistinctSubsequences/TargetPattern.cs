namespace DSAExperimentation.LeetCode.DistinctSubsequences;

// The other half of LC 115's operand pair: the sequence that some subsequence of
// SourceText has to spell exactly. It gets its own type for the same reason
// SourceText does, and it is named for what it is at the call site - the pattern
// being matched - rather than for its position in the parameter list.
internal readonly record struct TargetPattern(string Pattern);
