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
}
