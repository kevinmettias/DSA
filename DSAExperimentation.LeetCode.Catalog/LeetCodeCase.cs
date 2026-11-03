using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog;

// A case is kept as LeetCode's own text - one line of notation per judge input, and the answer
// in the same notation - because that is the form every source supplies and the one a reader can
// paste into LeetCode's console. A harness asks for the typed value it needs, so the catalog
// never has to know which C# type a problem's solution takes.
//
// ExpectedOutput is null where no source vouches for an answer: an example whose description the
// scrape could not pair with it, or a dataset case for an in-place question, whose reference
// solution returned nothing. Such a case still serves a harness that compares strategies with one
// another, and a benchmark.
/// <summary>One set of inputs for a question, with the answer to it where a source supplies one.</summary>
public sealed record LeetCodeCase
{
    private const string NoAnswerMessage = "This case carries no expected answer; filter on HasExpectedOutput first.";

    /// <summary>The judge's inputs in LeetCode notation, one per entry of the question's signature.</summary>
    public required IReadOnlyList<string> Inputs { get; init; }

    /// <summary>The expected answer in LeetCode notation, or null where no source supplies one.</summary>
    public string? ExpectedOutput { get; init; }

    /// <summary>Where the case came from; set by the catalog from the file it was read out of.</summary>
    [JsonIgnore]
    public LeetCodeCaseSource Source { get; init; }

    /// <summary>Whether the case carries an expected answer to compare against.</summary>
    [JsonIgnore]
    public bool HasExpectedOutput => ExpectedOutput is not null;

    /// <summary>Parses one input into the type the harness needs.</summary>
    /// <typeparam name="TValue">The type to read the input as.</typeparam>
    /// <param name="position">The input's position in the signature.</param>
    /// <returns>The input as a <typeparamref name="TValue"/>.</returns>
    public TValue Input<TValue>(int position) => LeetCodeNotation.Parse<TValue>(Inputs[position]);

    /// <summary>Parses the expected answer into the type the harness compares with.</summary>
    /// <typeparam name="TValue">The type to read the answer as.</typeparam>
    /// <returns>The expected answer as a <typeparamref name="TValue"/>.</returns>
    /// <exception cref="InvalidOperationException">The case carries no expected answer.</exception>
    public TValue Expected<TValue>()
    {
        if (ExpectedOutput is not { } output)
        {
            throw new InvalidOperationException(NoAnswerMessage);
        }

        return LeetCodeNotation.Parse<TValue>(output);
    }
}
