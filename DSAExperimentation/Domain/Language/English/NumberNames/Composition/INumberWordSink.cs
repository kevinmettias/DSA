namespace DSAExperimentation.Domain.Language.English.NumberNames.Composition;

// Where the composer writes a number's words, in reading order, so a caller with a notation of its
// own (capitals, no hyphens) writes them straight into its answer. A struct sink passed by ref
// compiles into the composer for each caller, and keeps the state it gathers.
/// <summary>Receives a number's words in reading order.</summary>
public interface INumberWordSink
{
    /// <summary>Takes the next word.</summary>
    /// <param name="word">The word, with the number it names.</param>
    void Append(NumberWord word);
}
