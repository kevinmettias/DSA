namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

// The string a conversion has to produce. Its own type for the same reason SourceText
// has one, and it is named for what it is at the call site - the shape the conversion
// is aiming at - rather than for a position among two interchangeable strings. Swapping
// a source and a target is the canonical silent-wrong-answer bug this distinguishes.
internal readonly record struct TargetText(string Text);
