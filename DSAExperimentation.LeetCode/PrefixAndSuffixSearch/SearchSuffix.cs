namespace DSAExperimentation.LeetCode.PrefixAndSuffixSearch;

// The suffix half of one f(prefix, suffix) query: the part a matching word must end
// with. Its own type for the same reason SearchPrefix has one - swapping a haystack
// head and tail is a silent wrong answer, and a pair of `string` positions would let
// the compiler accept it.
internal readonly record struct SearchSuffix(string Text);
