using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignSpreadsheet;

// LeetCode 3484. Design Spreadsheet: an instance API (setCell/resetCell/getValue)
// over a sparse grid of 26 columns and up to `rows` numbered rows, every unset
// cell reading as 0. Like DesignTaskManager, a Design problem's whole point is a
// sequence of mutating calls against one instance, so "every strategy for the
// problem" (ARCHITECTURE.md 17.3) takes the form of two classes implementing the
// shared ISpreadsheetStrategy surface below, differing only in which key/value
// container backs cell storage - a sparse map is the right representation
// regardless of container, since rows can run to 1e5 while only a handful of
// cells are ever actually set.
internal static class DesignSpreadsheetSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ISpreadsheetStrategy
    {
        void SetCell(string cell, int value);

        void ResetCell(string cell);

        int GetValue(string formula);
    }

    // The textbook answer: a BCL Dictionary<string, int> keyed by the cell
    // reference itself, deliberately without this repo's HashMap - the arm
    // SpreadsheetByHashMap has to justify itself against.
    internal sealed class SpreadsheetByDictionary : ISpreadsheetStrategy
    {
        private readonly Dictionary<string, int> _cells = new();

        // `rows` mirrors LC's Spreadsheet(int rows) constructor for fidelity; a
        // sparse map needs no capacity hint from it.
        public SpreadsheetByDictionary(int rows)
        {
        }

        public void SetCell(string cell, int value) => _cells[cell] = value;

        public void ResetCell(string cell) => _cells.Remove(cell);

        public int GetValue(string formula)
        {
            var (left, right) = SplitFormula(formula);
            return ResolveOperand(left) + ResolveOperand(right);
        }

        private int ResolveOperand(string operand)
            => char.IsDigit(operand[0]) ? int.Parse(operand) : _cells.GetValueOrDefault(operand);
    }

    // This repo's own HashMap<TKey, TValue> as the cell store, otherwise identical
    // to SpreadsheetByDictionary - proving the repo's hashmap is a drop-in
    // replacement for a BCL Dictionary here.
    internal sealed class SpreadsheetByHashMap : ISpreadsheetStrategy
    {
        private readonly HashMap<string, int> _cells = new();

        public SpreadsheetByHashMap(int rows)
        {
        }

        public void SetCell(string cell, int value) => _cells.Set(cell, value);

        public void ResetCell(string cell) => _cells.TryRemove(cell);

        public int GetValue(string formula)
        {
            var (left, right) = SplitFormula(formula);
            return ResolveOperand(left) + ResolveOperand(right);
        }

        private int ResolveOperand(string operand)
            => char.IsDigit(operand[0]) ? int.Parse(operand) : CellValueOrDefault(operand);

        // The cell's current value, or 0 where it has never been set - HashMap's
        // counterpart to the BCL Dictionary's own GetValueOrDefault above.
        private int CellValueOrDefault(string cell) => _cells.TryGetValue(cell, out var value) ? value : 0;
    }

    // Shared formula parsing: strip the leading '=' and split on the single '+' -
    // LC guarantees the "=X+Y" shape, so no operator precedence or nesting to
    // handle.
    private static (string Left, string Right) SplitFormula(string formula)
    {
        var body = formula.AsSpan(1);
        var plusIndex = body.IndexOf('+');
        return (body[..plusIndex].ToString(), body[(plusIndex + 1)..].ToString());
    }
}
