using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Composition;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Composition;

public sealed partial class NumberWordListTests
{
    [Fact]
    public void Words_New_IsEmpty() => Assert.Empty(new NumberWordList().Words);

    [Fact]
    public void Append_KeepsTheWordsInOrder()
    {
        var list = new NumberWordList();
        list.Append(EnglishNumberWords.Two);
        list.Append(EnglishNumberWords.Hundred);
        Assert.Equal([EnglishNumberWords.Two, EnglishNumberWords.Hundred], list.Words);
    }
}
