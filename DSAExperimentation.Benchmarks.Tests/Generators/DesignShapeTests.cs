using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

public sealed partial class DesignShapeTests
{
    // Range Sum Query - Mutable's signature as the snapshot writes it: declared types with brackets inside
    // their strings, and an example after the signature whose text names the class again.
    private const string DesignQuestion = """
        { "Signature": { "FunctionName": null, "Parameters": [ { "Name": "inputs", "Type": "integer[]" } ], "ReturnType": null,
          "ClassName": "NumArray", "ConstructorParameters": [ { "Name": "nums", "Type": "integer[]" } ],
          "Methods": [
            { "Name": "update", "Parameters": [ { "Name": "index", "Type": "integer" }, { "Name": "val", "Type": "integer" } ], "ReturnType": "void" },
            { "Name": "sumRange", "Parameters": [ { "Name": "left", "Type": "integer" }, { "Name": "right", "Type": "integer" } ], "ReturnType": "integer" } ] },
          "Examples": [ { "Inputs": [ "[\"NumArray\",\"sumRange\"]", "[[[1,3]],[0,1]]" ] } ] }
        """;

    // Random Pick Index's constructor as the snapshot writes it, C's length of nums listed after it.
    private const string QuestionWithALength = """
        { "Signature": { "ClassName": "Solution", "ConstructorParameters": [ { "Name": "nums", "Type": "integer[]" }, { "Name": "numsSize", "Type": "integer" } ],
          "Methods": [ { "Name": "pick", "Parameters": [ { "Name": "target", "Type": "integer" } ], "ReturnType": "integer" } ] } }
        """;

    private const string FunctionQuestion = """
        { "Signature": { "FunctionName": "count", "Parameters": [ { "Name": "nums", "Type": "integer[]" } ], "ReturnType": "integer", "ClassName": null } }
        """;

    private static readonly (string Name, int Inputs)[] DesignMethods = [("update", 2), ("sumRange", 2)];

    [Fact]
    public void In_DesignQuestion_CountsTheConstructorsInputs() => Assert.Equal(1, DesignShape.In(DesignQuestion)?.ConstructorInputs);

    [Fact]
    public void In_DesignQuestion_ReadsEachMethodsNameAndInputsInOrder() =>
        Assert.Equal(DesignMethods, DesignShape.In(DesignQuestion)?.Methods ?? []);

    [Fact]
    public void In_FunctionQuestion_FindsNoDesign() => Assert.Null(DesignShape.In(FunctionQuestion));

    [Fact]
    public void In_ConstructorListingTheLengthOfAnArray_CountsOnlyTheArray() => Assert.Equal(1, DesignShape.In(QuestionWithALength)?.ConstructorInputs);
}
