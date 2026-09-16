using DSAExperimentation.LeetCode.DesignSpreadsheet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignSpreadsheet;

// Harness only. Both strategies are DesignSpreadsheetSolution's - this file replays
// LeetCode's published call sequence against each ISpreadsheetStrategy
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a sequence of mutating calls
// rather than a single argument tuple. SpreadsheetOp.Apply is pure dispatch - no
// formula-evaluation logic of its own.
public sealed class DesignSpreadsheetTests
{
    public static TheoryData<int, SpreadsheetOp[], int?[]> Examples =>
        new()
        {
            {
                3,
                [
                    SpreadsheetOp.GetValue("=5+7"),
                    SpreadsheetOp.SetCell("A1", 10),
                    SpreadsheetOp.GetValue("=A1+6"),
                    SpreadsheetOp.SetCell("B2", 15),
                    SpreadsheetOp.GetValue("=A1+B2"),
                    SpreadsheetOp.ResetCell("A1"),
                    SpreadsheetOp.GetValue("=A1+B2"),
                ],
                [12, null, 16, null, 25, null, 15]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpreadsheetByDictionary_LeetCodeExample_EvaluatesFormulasAgainstStoredCells(
        int rows, SpreadsheetOp[] operations, int?[] expected) =>
        RunScript(new DesignSpreadsheetSolution.SpreadsheetByDictionary(rows), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpreadsheetByHashMap_LeetCodeExample_EvaluatesFormulasAgainstStoredCells(
        int rows, SpreadsheetOp[] operations, int?[] expected) =>
        RunScript(new DesignSpreadsheetSolution.SpreadsheetByHashMap(rows), operations, expected);

    private static void RunScript(
        DesignSpreadsheetSolution.ISpreadsheetStrategy strategy, SpreadsheetOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }

    // One call in a Spreadsheet script: which method to invoke and with what
    // arguments. Pure dispatch, built via the named factories below so a script (like
    // Examples above) reads like the LeetCode call sequence it replays. Nested because
    // it is only ever used inside this test class and has no independent identity: it
    // is this harness's own vocabulary, not a type another file would import.
    public readonly record struct SpreadsheetOp(SpreadsheetOp.OpKind kind, string cellOrFormula, int value)
    {
        public static SpreadsheetOp SetCell(string cell, int value) => new(OpKind.SetCell, cell, value);

        public static SpreadsheetOp ResetCell(string cell) => new(OpKind.ResetCell, cell, 0);

        public static SpreadsheetOp GetValue(string formula) => new(OpKind.GetValue, formula, 0);

        // null for the two void calls, the computed sum for GetValue - so a script
        // runner can assert against one expected value per operation uniformly.
        // Internal, not public: ISpreadsheetStrategy is internal to
        // DesignSpreadsheetSolution, and only this same assembly's RunScript ever
        // calls Apply.
        internal int? Apply(DesignSpreadsheetSolution.ISpreadsheetStrategy strategy)
        {
            switch (kind)
            {
                case OpKind.SetCell:
                    strategy.SetCell(cellOrFormula, value);
                    return null;
                case OpKind.ResetCell:
                    strategy.ResetCell(cellOrFormula);
                    return null;
                default:
                    return strategy.GetValue(cellOrFormula);
            }
        }

        public enum OpKind
        {
            SetCell,
            ResetCell,
            GetValue,
        }
    }
}
