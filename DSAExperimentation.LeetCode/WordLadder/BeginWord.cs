namespace DSAExperimentation.LeetCode.WordLadder;

// The word a ladder starts from. It exists so the two endpoint strings a solve takes
// read as distinct roles at the call site - a ladder runs from a begin word to an end
// word, never the reverse - rather than as a pair of interchangeable `string`
// positions whose order a caller can silently swap.
internal readonly record struct BeginWord(string Text);
