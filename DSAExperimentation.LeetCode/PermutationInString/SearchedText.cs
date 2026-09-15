namespace DSAExperimentation.LeetCode.PermutationInString;

// The string a permutation of the pattern has to be found inside. Its own type for the
// same reason PermutationPattern has one - the containment is one-directional, and a
// pair of `string` positions would let a caller hand the two over the wrong way round
// with the compiler none the wiser.
internal readonly record struct SearchedText(string Text);
