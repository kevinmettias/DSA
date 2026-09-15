namespace DSAExperimentation.LeetCode.DistinctSubsequences;

// LC 115 counts the subsequences of one string that spell another, and hands both
// in as plain strings. `NumDistinct(source, target)` names its two positions only
// by position, so a caller can hand them over the wrong way round and the compiler
// will not object - and the two are not interchangeable, because swapping them asks
// the opposite question. Each position therefore gets the type that says which role
// it plays, and a transposition stops compiling.
//
// "The source" is this problem's own vocabulary and answers nothing else, so per
// ARCHITECTURE.md 17.3 it lives beside the solution rather than in Domain/.
internal readonly record struct SourceText(string Text);
