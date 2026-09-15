namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

// The string a conversion starts from, before any registered rule is applied to it.
// It exists so the two strings a solve takes read as distinct roles at the call site -
// a conversion runs from a source into a target, never the reverse - rather than as a
// pair of interchangeable `string` positions whose order a caller can silently swap.
// The relation is one-directional: the DP walk only ever reads source[i..i+L) to find
// a rule's left-hand side and target[i..i+L) to find its right-hand side.
internal readonly record struct SourceText(string Text);
