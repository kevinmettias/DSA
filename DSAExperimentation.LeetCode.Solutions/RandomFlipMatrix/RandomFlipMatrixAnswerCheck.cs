using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.RandomFlipMatrix;

// LC 519 accepts, for each flip, any cell [i, j] of the m x n matrix still 0, chosen at random, so no script's
// answer can match a recording. A script's answer stands when it answers every call as LeetCode would: the
// constructor and each reset with null, and each flip with a cell inside the matrix that no flip since the
// last reset named. The check replays the script, so a cell named twice before a reset is refused; the
// recorded answer is one of the scripts that stand, and does not take part.
internal static class RandomFlipMatrixAnswerCheck
{
    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        var constructorInputs = arguments[0] as JsonArray;

        if (answer.Count != calls.Length || constructorInputs is not [JsonValue rows, JsonValue columns])
        {
            return false;
        }

        var rowCount = rows.GetValue<int>();
        var columnCount = columns.GetValue<int>();
        var matrix = new FlippedCells(rowCount, columnCount);
        var isAccepted = answer[0] is null;

        for (var call = 1; isAccepted && call < calls.Length; call++)
        {
            isAccepted = matrix.IsAnsweredBy(calls[call], answer[call]);
        }

        return isAccepted;
    }

    // The matrix as the script leaves it after each call: which of its cells are 1.
    private sealed class FlippedCells(int rows, int columns)
    {
        // LeetCode's names for the two calls after the constructor.
        private const string Flip = "flip";
        private const string Reset = "reset";

        private readonly HashSet<(int Row, int Column)> _flipped = [];

        // Whether a call's answer is one LeetCode accepts, the call then applied: a flip names a cell still 0,
        // which becomes 1; a reset answers null, and every cell becomes 0.
        public bool IsAnsweredBy(string call, JsonNode? answered) => call switch
        {
            Flip => IsFlipOfZeroCell(answered),
            Reset => IsReset(answered),
            _ => false,
        };

        private bool IsFlipOfZeroCell(JsonNode? answered)
        {
            var cell = answered as JsonArray;

            if (cell is not [JsonValue row, JsonValue column])
            {
                return false;
            }

            if (IndexAmong(rows, row) is not { } rowIndex || IndexAmong(columns, column) is not { } columnIndex)
            {
                return false;
            }

            return _flipped.Add((rowIndex, columnIndex));
        }

        // The index a coordinate names among count rows or columns; null when it names none - not a whole
        // number, or out of range.
        private static int? IndexAmong(int count, JsonValue coordinate)
        {
            if (!coordinate.TryGetValue<int>(out var index))
            {
                return null;
            }

            var isInRange = index >= 0 && index < count;
            return isInRange ? index : null;
        }

        private bool IsReset(JsonNode? answered)
        {
            _flipped.Clear();
            return answered is null;
        }
    }
}
