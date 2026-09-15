namespace DSAExperimentation.LeetCode.CamelcaseMatching;

// The camelcase pattern a query is tested against: the letters the query must contain
// in order, and the rule that every other character the query contributes is lowercase.
// Distinct in type from CamelQuery because the match is one-directional - the pattern is
// never scanned for the query - so the two arguments cannot honestly be exchanged.
internal readonly record struct CamelPattern(string Text);
