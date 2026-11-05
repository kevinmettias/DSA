namespace DSAExperimentation.LeetCode.Solutions.SortCharactersByFrequency;

// LC 451 accepts any string that sorts the characters by how often they occur, most often first, and says
// that characters occurring equally often may come in either order. So an answer stands when it holds the
// string's characters, each character's occurrences side by side, and no run longer than the one before
// it. The recorded answer is one of these, so it does not take part.
internal static class SortCharactersByFrequencyAnswerCheck
{
    public static bool IsAccepted(string text, string answer, string recorded)
        => answer.Order().SequenceEqual(text.Order()) && IsInRunsLongestFirst(answer);

    // Each character's occurrences make one run, and the runs never grow from one to the next.
    private static bool IsInRunsLongestFirst(string answer)
    {
        var runs = new List<(char Character, int Length)>();

        foreach (var character in answer)
        {
            if (runs.Count > 0 && runs[^1].Character == character)
            {
                runs[^1] = (character, runs[^1].Length + 1);
            }
            else
            {
                runs.Add((character, 1));
            }
        }

        var isEachCharacterOneRun = runs.Select(run => run.Character).Distinct().Count() == runs.Count;
        return isEachCharacterOneRun && runs.Zip(runs.Skip(1)).All(pair => pair.First.Length >= pair.Second.Length);
    }
}
