using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

// The parsed form of LeetCodeQuestionDto.MetaData's nested JSON string, which has two shapes and
// no documentation: a function question's {name, params, return, output?, manual?} and a design
// question's {classname, constructor, methods, systemdesign}. Every member is optional so one
// type reads both, and LeetCodeQuestionMapper builds a LeetCodeSignature from whichever half is
// present.
internal sealed record LeetCodeMetaDataDto
{
    public string? Name { get; init; }

    [JsonPropertyName("params")]
    public List<LeetCodeParameterDto>? Parameters { get; init; }

    public LeetCodeReturnDto? Return { get; init; }

    public LeetCodeOutputDto? Output { get; init; }

    public bool Manual { get; init; }

    [JsonPropertyName("classname")]
    public string? ClassName { get; init; }

    public LeetCodeConstructorDto? Constructor { get; init; }

    public List<LeetCodeMethodDto>? Methods { get; init; }
}
