namespace DSAExperimentation.LeetCode.FindAllAnagramsInAString;

// The string a scan reads windows out of. It exists so the two strings a solve takes
// read as distinct roles at the call site - one is searched, the other is the shape
// being searched for - rather than as a pair of interchangeable `string` positions
// whose order a caller can silently swap.
internal readonly record struct ScannedText(string Text);
