using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class TextKeyTests
{
    [Fact]
    public void Length_IsTheStringsLength() => Assert.Equal(4, new TextKey("tree").Length);

    [Fact]
    public void LetterAt_ReadsTheCharactersFromTheFirst()
    {
        var key = new TextKey("tree");

        Assert.Equal("tree", new string([.. Enumerable.Range(0, key.Length).Select(key.LetterAt)]));
    }

    [Fact]
    public void ImplicitConversion_ReadsTheSameString()
    {
        TextKey key = "ab";

        Assert.Equal((2, 'b'), (key.Length, key.LetterAt(1)));
    }
}
