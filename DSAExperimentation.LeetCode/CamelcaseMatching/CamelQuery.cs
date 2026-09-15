namespace DSAExperimentation.LeetCode.CamelcaseMatching;

// One candidate string from LC 1023's queries array, before it is tested against the
// pattern. It exists so the two sides of a match read as distinct roles at the call
// site - a query is searched, a pattern is what it is searched for - rather than as a
// pair of interchangeable strings whose order a caller can silently swap.
internal readonly record struct CamelQuery(string Text);
