namespace DSAExperimentation.LeetCode.Solutions.WordsWithinTwoEditsOfDictionary;

// The values LC 2452 fixes, declared once so WordsWithinTwoEditsOfDictionarySolution and its
// benchmark read the same copy instead of each restating it.
internal static class WordsWithinTwoEditsOfDictionaryConstants
{
    // LeetCode's budget: a query matches a word it differs from in at most two positions.
    internal const int MaxEdits = 2;
}
