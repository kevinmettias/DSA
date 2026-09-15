namespace DSAExperimentation.LeetCode.PermutationInString;

// The string whose permutation is being sought: the shape every fixed-size window of
// the searched text is compared against. It exists so the two strings a solve takes
// read as distinct roles at the call site - one must fit inside the other, never the
// reverse - rather than as a pair of interchangeable `string` positions whose order a
// caller can silently swap.
internal readonly record struct PermutationPattern(string Text);
