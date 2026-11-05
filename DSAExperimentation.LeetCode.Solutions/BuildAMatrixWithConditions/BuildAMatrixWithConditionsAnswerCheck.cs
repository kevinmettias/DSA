namespace DSAExperimentation.LeetCode.Solutions.BuildAMatrixWithConditions;

// LC 2392 accepts any k x k matrix holding each number from 1 to k once, zeros elsewhere, with every
// condition's first number in a row above (or a column left of) its second, and the empty matrix only
// when no matrix meets the conditions. The catalog records one, so an answer stands when it is empty
// exactly where the recorded one is, or else such a matrix. Kahn's sort and the naive rescan break ties
// between ready numbers differently.
internal static class BuildAMatrixWithConditionsAnswerCheck
{
    public static bool IsAccepted(int valueCount, int[][] rowConditions, int[][] colConditions, int[][] answer, int[][] recorded)
    {
        if (answer.Length == 0 || recorded.Length == 0)
        {
            return answer.Length == recorded.Length;
        }

        if (PlacesOf(valueCount, answer) is not { } places)
        {
            return false;
        }

        return rowConditions.All(condition => places[condition[0]].Row < places[condition[1]].Row)
            && colConditions.All(condition => places[condition[0]].Column < places[condition[1]].Column);
    }

    // Where each number from 1 to k stands, indexed by the number; null unless the matrix is k x k and holds
    // each of them exactly once, with zeros in every other cell.
    private static (int Row, int Column)[]? PlacesOf(int valueCount, int[][] matrix)
    {
        if (matrix.Length != valueCount || matrix.Any(row => row.Length != valueCount))
        {
            return null;
        }

        var cells = Enumerable.Range(0, valueCount).SelectMany(row => Enumerable.Range(0, valueCount).Select(column => (Row: row, Column: column)));
        var filled = cells.Where(cell => matrix[cell.Row][cell.Column] != 0).ToList();
        var numbers = filled.Select(cell => matrix[cell.Row][cell.Column]).Order();
        var everyNumber = Enumerable.Range(1, valueCount);

        if (!numbers.SequenceEqual(everyNumber))
        {
            return null;
        }

        var places = new (int Row, int Column)[valueCount + 1];
        filled.ForEach(cell => places[matrix[cell.Row][cell.Column]] = cell);
        return places;
    }
}
