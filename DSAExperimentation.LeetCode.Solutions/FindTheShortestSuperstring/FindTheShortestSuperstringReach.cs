namespace DSAExperimentation.LeetCode.Solutions.FindTheShortestSuperstring;

// The reach of FindTheShortestSuperstringSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. ShortestSuperstringByPermutations tries every order of the
// words, and takes 181 ms over the 57 of the catalog's 88 cases with up to 9 words (measured 2026-11-05,
// Release); a 10-word case takes it about 90 ms alone, and an 11-word one over a second.
internal static class FindTheShortestSuperstringReach
{
    private const int LargestWordCount = 9;

    public static bool IsWithin(string[] words) =>
        words.Length <= LargestWordCount;
}
