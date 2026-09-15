namespace DSAExperimentation.LeetCode.FindTheShortestSuperstring;

// A word in the later half of a concatenation: the one whose HEAD is dropped by however
// much its predecessor's tail already spells it. The mirror of PredecessorWord, and a
// distinct type for the same reason - an overlap length is not symmetric in its two
// words, so swapping them would answer a different question rather than the same one.
internal readonly record struct SuccessorWord(string Text);
