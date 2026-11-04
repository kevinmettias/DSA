namespace DSAExperimentation.LeetCode.Solutions.SimilarStringGroups;

// LC 839's promise about its input: the words are all one length and all anagrams of each other.
// LeetCode's judge never poses an input that breaks it. Words of different letters are never
// similar however they are grouped, and the strategies compare them differently. Generated cases do
// not always keep the promise, so a harness that runs them asks this first.
internal static class SimilarStringGroupsPromise
{
    public static bool IsKeptBy(string[] strs) => strs.Select(word => new string([.. word.Order()])).Distinct().Count() == 1;
}
