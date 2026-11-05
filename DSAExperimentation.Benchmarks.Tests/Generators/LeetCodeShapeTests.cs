using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

// The compile-time half of the harness's LeetCodeShape: each row names a field or a method of Holder below.
public sealed partial class LeetCodeShapeTests
{
    // Holder's overload pairs: rows beside tuples, a declared input beside one more, and two that match alike.
    private const string Rows = "CountByRows";
    private const string Pairs = "CountByPairs";
    private const string OneInput = "SumByLoop";
    private const string TwoInputs = "SumByHalf";
    private const string ByArray = "PickByArray";
    private const string ByList = "PickByList";
    private const string IntegerRows = "integer[][]";
    private const string Integers = "integer[]";
    private const int BothOverloads = 2;

    private static readonly string[] QuestionsTypes = [IntegerRows, "integer"];

    private const string Holder = """
        using System.Collections.Generic;
        using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

        internal static class Holder
        {
            public static int[][] Grid;
            public static (int Start, int End)[] Intervals;
            public static IEnumerable<string> Words;
            public static BinaryTreeNode<int> Tree;
            public static string Text;

            public static int CountByRows(int[][] intervals) => 0;
            public static int CountByPairs((int Start, int End)[] intervals) => 0;
            public static int SumByLoop(int[] values) => 0;
            public static int SumByHalf(int[] values, int half) => 0;
            public static int PickByArray(int[] values) => 0;
            public static int PickByList(IList<int> values) => 0;
        }
        """;

    // A question file: its signature declares an integer[][] and an integer, after a title and before a
    // design-shaped constructor list whose types are not the function's.
    private const string QuestionFile = """
        { "Title": "Probe", "Signature": { "FunctionName": "count", "Parameters": [ { "Name": "intervals", "Type": "integer[][]" }, { "Name": "k", "Type": "integer" } ], "ReturnType": "integer", "ConstructorParameters": [ { "Name": "x", "Type": "string" } ] } }
        """;

    private const string DesignFile = """
        { "Signature": { "FunctionName": null, "Parameters": [], "ReturnType": null, "ConstructorParameters": [ { "Name": "capacity", "Type": "integer" } ] } }
        """;

    // A field and the type a question may declare for it, which it is.
    public static TheoryData<string, string> DeclaredFields => new()
    {
        { "Grid", "integer[][]" },
        { "Words", "string[]" },
        { "Words", "list<string>" },
        { "Tree", "TreeNode" },
        { "Text", "string" },
    };

    // A field and a type a question may declare, which it is not.
    public static TheoryData<string, string> OtherFields => new()
    {
        { "Intervals", "integer[][]" },
        { "Grid", "integer[]" },
        { "Text", "character" },
    };

    [Fact]
    public void DeclaredTypesIn_QuestionFile_ReadsTheFunctionsParameterTypesInOrder() =>
        Assert.Equal(QuestionsTypes, LeetCodeShape.DeclaredTypesIn(QuestionFile));

    [Fact]
    public void DeclaredTypesIn_DesignQuestionFile_ReadsNone() => Assert.Empty(LeetCodeShape.DeclaredTypesIn(DesignFile));

    [Theory]
    [MemberData(nameof(DeclaredFields))]
    public void IsDeclaredType_FieldOfTheDeclaredType_IsTrue(string field, string declared)
    {
        var isDeclared = LeetCodeShape.IsDeclaredType(GeneratorProbe.FieldType(Holder, field), declared);

        Assert.True(isDeclared);
    }

    [Theory]
    [MemberData(nameof(OtherFields))]
    public void IsDeclaredType_FieldOfAnotherType_IsFalse(string field, string declared)
    {
        var isDeclared = LeetCodeShape.IsDeclaredType(GeneratorProbe.FieldType(Holder, field), declared);

        Assert.False(isDeclared);
    }

    [Fact]
    public void Narrowed_RowsBesideTuples_KeepsTheRows()
    {
        var narrowed = LeetCodeShape.Narrowed([GeneratorProbe.Method(Holder, Rows), GeneratorProbe.Method(Holder, Pairs)], [IntegerRows]);

        Assert.Equal(Rows, Assert.Single(narrowed).Name);
    }

    [Fact]
    public void Narrowed_OverloadsOfDifferentCounts_KeepsTheOneOfTheDeclaredCount()
    {
        var narrowed = LeetCodeShape.Narrowed([GeneratorProbe.Method(Holder, OneInput), GeneratorProbe.Method(Holder, TwoInputs)], [Integers]);

        Assert.Equal(OneInput, Assert.Single(narrowed).Name);
    }

    [Fact]
    public void Narrowed_TwoOverloadsMatchingAlike_KeepsBoth()
    {
        var narrowed = LeetCodeShape.Narrowed([GeneratorProbe.Method(Holder, ByArray), GeneratorProbe.Method(Holder, ByList)], [Integers]);

        Assert.Equal(BothOverloads, narrowed.Count);
    }
}
