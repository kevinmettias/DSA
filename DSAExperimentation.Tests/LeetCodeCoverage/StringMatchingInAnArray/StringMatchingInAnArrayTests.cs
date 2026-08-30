using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StringMatchingInAnArray;

// LeetCode 1408. String Matching in an Array: for each word, KMP substring
// search (this repo's own PrefixFunctionSearch) against every other word - a
// word is kept whenever some other word's text contains it as a substring.
public sealed partial class StringMatchingInAnArrayTests
{
    [Fact]
    public void StringMatching_LeetCodeExampleOne_ReturnsContainedWords()
    {
        string[] words = ["mass", "as", "hero", "superhero"];

        var result = StringMatching(words);

        Assert.Equal(["as", "hero"], result);
    }

    [Fact]
    public void StringMatching_LeetCodeExampleTwo_ReturnsContainedWords()
    {
        string[] words = ["leetcode", "et", "code"];

        var result = StringMatching(words);

        Assert.Equal(["et", "code"], result);
    }

    [Fact]
    public void StringMatching_NoWordIsASubstringOfAnother_ReturnsEmpty()
    {
        string[] words = ["blue", "green", "bu"];

        var result = StringMatching(words);

        Assert.Equal([], result);
    }

    private static List<string> StringMatching(string[] words)
    {
        var result = new List<string>();

        for (var i = 0; i < words.Length; i++)
        {
            if (IsSubstringOfAnotherWord(words, i))
            {
                result.Add(words[i]);
            }
        }

        return result;
    }

    private static bool IsSubstringOfAnotherWord(string[] words, int index)
    {
        for (var j = 0; j < words.Length; j++)
        {
            if (j != index && PrefixFunctionSearch.FindAll(words[j], words[index]).Count > 0)
            {
                return true;
            }
        }

        return false;
    }
}
