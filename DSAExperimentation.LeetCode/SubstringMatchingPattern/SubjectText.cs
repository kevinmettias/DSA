namespace DSAExperimentation.LeetCode.SubstringMatchingPattern;

// The string a wildcard pattern is searched inside. It exists so the two strings a
// solve takes read as distinct roles at the call site - one is scanned for
// occurrences, the other supplies the halves being looked for - rather than as a pair
// of interchangeable `string` positions whose order a caller can silently swap.
internal readonly record struct SubjectText(string Text);
