using System.Reflection;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class LeetCodeShapeTests
{
    // A parameter type and the type a question may declare for it, which it is.
    public static TheoryData<Type, string> DeclaredTypes => new()
    {
        { typeof(int[][]), "integer[][]" },
        { typeof(IEnumerable<string>), "string[]" },
        { typeof(IList<int>), "list<integer>" },
        { typeof(BinaryTreeNode<int>), "TreeNode" },
        { typeof(char), "character" },
    };

    // A parameter type and a type a question may declare, which it is not.
    public static TheoryData<Type, string> OtherTypes => new()
    {
        { typeof((int, int)[]), "integer[][]" },
        { typeof(int[]), "integer[][]" },
        { typeof(string), "character" },
    };

    [Theory]
    [MemberData(nameof(DeclaredTypes))]
    public void IsDeclaredType_TypeOfTheDeclaredType_IsTrue(Type type, string declared)
    {
        var isDeclared = LeetCodeShape.IsDeclaredType(type, declared);

        Assert.True(isDeclared);
    }

    [Theory]
    [MemberData(nameof(OtherTypes))]
    public void IsDeclaredType_TypeOfAnotherShape_IsFalse(Type type, string declared)
    {
        var isDeclared = LeetCodeShape.IsDeclaredType(type, declared);

        Assert.False(isDeclared);
    }

    [Fact]
    public void Narrowed_RowsBesideTuples_KeepsTheRows()
    {
        var narrowed = LeetCodeShape.Narrowed(OverloadsOf(nameof(ShapedProbeSolution.CountByLoop)), ["integer[][]"]);

        Assert.Equal(typeof(int[][]), Assert.Single(narrowed).GetParameters()[0].ParameterType);
    }

    [Fact]
    public void Narrowed_OverloadsOfDifferentCounts_KeepsTheOneOfTheDeclaredCount()
    {
        var narrowed = LeetCodeShape.Narrowed(OverloadsOf(nameof(ShapedProbeSolution.SumByLoop)), ["integer[]"]);

        Assert.Single(Assert.Single(narrowed).GetParameters());
    }

    [Fact]
    public void Narrowed_TwoOverloadsMatchingAlike_KeepsBoth()
    {
        var narrowed = LeetCodeShape.Narrowed(OverloadsOf(nameof(ShapedProbeSolution.PickByLoop)), ["integer[]"]);

        Assert.Equal(2, narrowed.Count);
    }

    [Fact]
    public void Narrowed_TupleSpreadOverTheDeclaredInputs_KeepsTheTuple()
    {
        var narrowed = LeetCodeShape.Narrowed(OverloadsOf(nameof(ShapedProbeSolution.CostByLoop)), ["string", "character[]", "character[]", "integer[]"]);

        Assert.Equal(typeof((char[], char[], int[])), Assert.Single(narrowed).GetParameters()[1].ParameterType);
    }

    [Fact]
    public void Narrowed_NoOverloadOfTheDeclaredCount_KeepsThemAll()
    {
        var narrowed = LeetCodeShape.Narrowed(OverloadsOf(nameof(ShapedProbeSolution.PickByLoop)), ["integer[]", "integer", "integer"]);

        Assert.Equal(2, narrowed.Count);
    }

    private static List<MethodInfo> OverloadsOf(string name) =>
        [.. typeof(ShapedProbeSolution).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(method => method.Name == name)];
}
