using DSAExperimentation.LeetCode.Solutions.ImplementTrie;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ImplementTrie;

// LeetCode's example: "apple" inserted.
public sealed partial class ImplementTrieCallsTests
{
    private const string Word = "apple";
    private const string Prefix = "app";

    [Fact]
    public void Search_InsertedWord_IsTrue() => Assert.True(ImplementTrieCalls.Search(Words(), Word));

    [Fact]
    public void Search_PrefixOfAnInsertedWord_IsFalse() => Assert.False(ImplementTrieCalls.Search(Words(), Prefix));

    [Fact]
    public void StartsWith_PrefixOfAnInsertedWord_IsTrue() => Assert.True(ImplementTrieCalls.StartsWith(Words(), Prefix));

    [Fact]
    public void Insert_Word_AddsIt()
    {
        var words = ImplementTrieSolution.CreateByArrayNodes();

        ImplementTrieCalls.Insert(words, Word);

        Assert.True(words.HasKey(Word));
    }

    private static ImplementTrieSolution.ArrayNodeTrie Words()
    {
        var words = ImplementTrieSolution.CreateByArrayNodes();
        ImplementTrieCalls.Insert(words, Word);

        return words;
    }
}
