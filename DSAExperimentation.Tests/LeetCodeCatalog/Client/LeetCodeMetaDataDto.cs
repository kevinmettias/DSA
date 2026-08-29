using System.Text.Json.Serialization;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// The parsed form of LeetCodeQuestionDto.MetaData's nested JSON string.
// Parameters' COUNT (not their types - this catalog doesn't attempt to
// generically type raw test-case text, see ILeetCodeTestCaseAdapter's own doc
// comment for why that's left to each problem) is what LeetCodeQuestionMapper
// needs: exampleTestcases is one flat, newline-delimited list of every parameter
// of every example concatenated together, and the only way to know where one
// example ends and the next begins is this array's length.
//
// Parameters is OPTIONAL, not required: a "systemdesign" problem (Min Stack,
// Implement Trie - a class with a constructor plus several methods, not one
// function) has a completely different shape here (classname/constructor/methods,
// no top-level params array at all), and exampleTestcases for those encodes a
// call SEQUENCE (method names + per-call argument arrays), not one function's
// positional arguments - a shape ILeetCodeTestCaseAdapter's single
// Func<TInput,TOutput> contract doesn't fit either. LeetCodeQuestionMapper
// detects a null Parameters here and leaves ExampleTestCases empty for that
// question rather than guessing at a parse - see its own doc comment.
internal sealed record LeetCodeMetaDataDto
{
    [JsonPropertyName("params")]
    public List<LeetCodeParameterDto>? Parameters { get; init; }
}
