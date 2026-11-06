using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Questions;

public sealed partial class LeetCodeSignatureTests
{
    [Fact]
    public void IsDesign_SignatureNamingAClass_IsTrue()
    {
        var signature = new LeetCodeSignature { ClassName = "MinStack", Parameters = [], ConstructorParameters = [], Methods = [] };

        Assert.True(signature.IsDesign);
    }

    [Fact]
    public void IsDesign_FunctionSignature_IsFalse()
    {
        var signature = new LeetCodeSignature { FunctionName = "twoSum", Parameters = [], ConstructorParameters = [], Methods = [] };

        Assert.False(signature.IsDesign);
    }

    [Fact]
    public void ScriptedConstructorParameters_ArraysWithTheirLengths_DropsTheLengths()
    {
        LeetCodeParameter[] declared =
        [
            new("matrix", "integer[][]"), new("matrixRowSize", "integer"), new("matrixColSize", "integer"),
            new("nums", "integer[]"), new("numsSize", "integer"), new("k", "integer"),
        ];
        var signature = new LeetCodeSignature { ClassName = "Probe", Parameters = [], ConstructorParameters = declared, Methods = [] };
        var names = signature.ScriptedConstructorParameters.Select(parameter => parameter.Name);

        Assert.Equal(["matrix", "nums", "k"], names);
    }

    [Fact]
    public void ScriptedConstructorParameters_IntegerNamedForNoArray_KeepsIt()
    {
        var signature = new LeetCodeSignature { ClassName = "Probe", Parameters = [], ConstructorParameters = [new("windowSize", "integer")], Methods = [] };

        Assert.Single(signature.ScriptedConstructorParameters);
    }
}
