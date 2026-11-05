using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.LeetCode.Solutions.FindSubstringWithGivenHashValue;

// LC 2156's promise about its input: the string consists of lowercase English letters, and an answer always
// exists - some k-long substring hashes to hashValue. LeetCode's judge never poses an input that breaks it.
// The rolling hash reads each character's value from the alphabet and throws on a space, and where no
// window matches, both strategies find nothing. Generated cases do not always keep the promise, so a
// harness that runs them asks this first. It hashes every window by the statement's own definition:
// the sum of each letter's number (a as 1) times power to its offset, modulo the modulus.
internal static class FindSubstringWithGivenHashValuePromise
{
    public static bool IsKeptBy(string text, RollingHashLane lane, (int WindowLength, long HashValue) target)
    {
        var windowCount = Math.Max(text.Length - target.WindowLength + 1, 0);
        return text.All(char.IsAsciiLetterLower) && Enumerable.Range(0, windowCount).Any(start => HashOf(text, start, target.WindowLength, lane) == target.HashValue);
    }

    private static long HashOf(string text, int start, int length, RollingHashLane lane)
    {
        var (hash, power) = (0L, 1L);

        for (var offset = 0; offset < length; offset++)
        {
            hash = (hash + (EnglishLetters.NumberOf(text[start + offset]) * power)) % lane.Modulus;
            power = power * lane.Base % lane.Modulus;
        }

        return hash;
    }
}
