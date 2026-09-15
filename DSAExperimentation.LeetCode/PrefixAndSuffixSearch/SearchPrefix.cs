namespace DSAExperimentation.LeetCode.PrefixAndSuffixSearch;

// The prefix half of one f(prefix, suffix) query: the part a matching word must start
// with. It exists so the two strings a query takes read as distinct roles at the call
// site - a word is tested for a prefix at its head and a suffix at its tail, and the
// two are not interchangeable - rather than as a pair of `string` positions whose
// order a caller can silently swap.
internal readonly record struct SearchPrefix(string Text);
