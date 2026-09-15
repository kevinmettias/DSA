namespace DSAExperimentation.LeetCode.SubstringMatchingPattern;

// The pattern being matched, carrying the single '*' this problem is built around. Its
// own type for the same reason SubjectText has one, and it carries a role SubjectText
// does not: it is the side that is split on the wildcard and whose halves are sought
// separately, so it is never the one scanned.
internal readonly record struct WildcardPattern(string Text);
