namespace DSAExperimentation.LeetCode.FindAllAnagramsInAString;

// The string whose anagrams are being located: the frequency shape every window of the
// scanned text is compared against. Its own type for the same reason ScannedText has
// one, and it carries a role ScannedText does not - it is the one that must fit inside
// the other, so the length guard is stated against it.
internal readonly record struct AnagramPattern(string Text);
