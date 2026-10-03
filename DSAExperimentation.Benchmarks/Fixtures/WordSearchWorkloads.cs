namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 79: a side x side board of letters drawn from a
// seeded Random over "AB", and a word whose letters but the last are read off a
// seeded self-avoiding walk across the board, while its last letter, 'C', appears
// nowhere on it. The word is therefore not traceable, but a tracer only learns that
// at the last letter: every partial match - the planted walk among them, followed
// to its end - has to be tried and abandoned first. Two letters rather than LC's 52
// make most neighbours match the next letter, so those partial matches are many.
// Path is the planted walk, one board cell per letter of Word but the last.
internal static class WordSearchWorkloads
{
    public const char AbsentLetter = 'C';

    private const string Alphabet = "AB";

    private static readonly (int Row, int Column)[] Steps = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    public static (char[][] Board, string Word, (int Row, int Column)[] Path) Build(int side, int wordLength, Random random)
    {
        var board = Enumerable.Range(0, side)
            .Select(_ => SeededDraws.Values(side, 0, Alphabet.Length, random).Select(index => Alphabet[index]).ToArray())
            .ToArray();
        var path = SelfAvoidingWalk(side, wordLength - 1, random);
        var word = new string([.. path.Select(cell => board[cell.Row][cell.Column]), AbsentLetter]);

        return (board, word, path);
    }

    // A walk can trap itself in a corner of its own making before it is long enough; it is then
    // abandoned and a new one drawn from a fresh start, so the loop stops at the first walk that
    // reaches the asked length.
    private static (int Row, int Column)[] SelfAvoidingWalk(int side, int length, Random random)
    {
        while (true)
        {
            var path = new List<(int Row, int Column)> { (random.Next(side), random.Next(side)) };

            while (path.Count < length && NextSteps(path, side) is { Count: > 0 } open)
            {
                path.Add(open[random.Next(open.Count)]);
            }

            if (path.Count == length)
            {
                return [.. path];
            }
        }
    }

    private static List<(int Row, int Column)> NextSteps(List<(int Row, int Column)> path, int side) =>
        [.. Steps
            .Select(step => (Row: path[^1].Row + step.Row, Column: path[^1].Column + step.Column))
            .Where(cell => cell.Row >= 0 && cell.Row < side && cell.Column >= 0 && cell.Column < side)
            .Where(cell => !path.Contains(cell))];
}
