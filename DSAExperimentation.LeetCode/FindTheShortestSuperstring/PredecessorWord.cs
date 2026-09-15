namespace DSAExperimentation.LeetCode.FindTheShortestSuperstring;

// A word in the earlier half of a concatenation: the one whose TAIL is measured against
// the head of the word that follows it. LC 943 assembles a superstring by appending each
// word after its predecessor and dropping the prefix that predecessor already ends with,
// so the two ends of that equation are not interchangeable - this type says which end.
internal readonly record struct PredecessorWord(string Text);
