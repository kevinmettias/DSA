namespace DSAExperimentation.LeetCode.Solutions.FindTheShortestSuperstring;

// LC 943 accepts any shortest string that holds every word ("If there are multiple valid strings of the
// smallest length, return any of them"). The catalog records one, as long as any, so an answer stands when it
// holds every word and is as long as the recorded one. The permutation scan and the memoized bitmask break
// ties between equally short orders of the words differently, and neither always as the catalog did.
internal static class FindTheShortestSuperstringAnswerCheck
{
    public static bool IsAccepted(string[] words, string answer, string recorded)
        => answer.Length == recorded.Length && words.All(word => answer.Contains(word, StringComparison.Ordinal));
}
