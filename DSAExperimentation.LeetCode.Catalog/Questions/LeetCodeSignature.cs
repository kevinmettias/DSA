using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Questions;

// LeetCode's metaData, typed. It comes in two shapes: a function question names one function, its
// parameters and its return type; a design question names a class, its constructor and its
// methods, and its cases are call scripts rather than argument lists. One record holds both so a
// question always has a signature, and IsDesign says which half is filled.
//
// Parameters are the JUDGE's inputs, which are not always the function's: Linked List Cycle's
// judge takes `pos` to build the cycle, and the function never sees it. HasCustomJudge is
// LeetCode's own flag for that ("manual"), and a case has one input per entry here either way.
/// <summary>The shape of a question's inputs and answer, as LeetCode's metaData describes it.</summary>
public sealed record LeetCodeSignature
{
    /// <summary>The function's name for a function question; null for a design question.</summary>
    public string? FunctionName { get; init; }

    /// <summary>The judge's inputs for a function question, in the order a case lists them.</summary>
    public required IReadOnlyList<LeetCodeParameter> Parameters { get; init; }

    /// <summary>LeetCode's type name for the function's answer; null for a design question.</summary>
    public string? ReturnType { get; init; }

    /// <summary>
    /// For an in-place question whose function returns nothing, the input the judge reads its answer back
    /// from; null otherwise.
    /// </summary>
    public int? OutputParameterIndex { get; init; }

    /// <summary>Whether LeetCode drives this question with a hand-written judge whose inputs differ from the function's.</summary>
    public bool HasCustomJudge { get; init; }

    /// <summary>The class a design question asks for; null for a function question.</summary>
    public string? ClassName { get; init; }

    /// <summary>The design class's constructor inputs; empty for a function question.</summary>
    public required IReadOnlyList<LeetCodeParameter> ConstructorParameters { get; init; }

    /// <summary>The design class's methods; empty for a function question.</summary>
    public required IReadOnlyList<LeetCodeMethod> Methods { get; init; }

    /// <summary>Whether the question asks for a class driven by a call script rather than one function.</summary>
    [JsonIgnore]
    public bool IsDesign => ClassName is not null;
}
