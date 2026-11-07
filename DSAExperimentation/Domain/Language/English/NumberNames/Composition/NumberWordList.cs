namespace DSAExperimentation.Domain.Language.English.NumberNames.Composition;

// A sink that keeps a number's words in a list, for a caller that reads them after composing. Its
// list is made with it, so a copy of the sink adds to the same list, which is what a caller reading
// the words afterwards relies on.
/// <summary>Gathers a number's words into a list, in reading order.</summary>
public readonly struct NumberWordList : INumberWordSink
{
    /// <summary>Gets the words gathered so far.</summary>
    public List<NumberWord> Words { get; }

    /// <summary>A sink with an empty list.</summary>
    public NumberWordList() => Words = [];

    /// <inheritdoc/>
    public void Append(NumberWord word) => Words.Add(word);
}
