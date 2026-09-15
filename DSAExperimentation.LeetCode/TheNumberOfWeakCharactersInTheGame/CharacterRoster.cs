namespace DSAExperimentation.LeetCode.TheNumberOfWeakCharactersInTheGame;

// LC 1996 states its input as int[][], one two-element row per character. Naming the
// two columns once is the whole of this problem's input preparation - every strategy
// reads attack and defense, none reads a row.
//
// It lives beside the solution rather than in Domain/ because a game character's
// attack/defense pair fixes THIS problem's content and nothing else's (ARCHITECTURE
// section 17.3), and it is deliberately not an IEnumerable: it is the parameter type of
// the prepared-input overloads (section 17.4), so it must never be bindable by the
// int[][]-shaped overloads the LeetCode-shaped calls go through.
internal sealed class CharacterRoster
{
    private const int AttackColumn = 0;
    private const int DefenseColumn = 1;

    private readonly (int Attack, int Defense)[] _characters;

    public int Count => _characters.Length;

    private CharacterRoster((int Attack, int Defense)[] characters) => _characters = characters;

    public (int Attack, int Defense) At(int index) => _characters[index];

    // A fresh array every call: the sorting strategy reorders what it is handed, and a
    // benchmark hands it the same roster on every invocation.
    public (int Attack, int Defense)[] ToSortableArray() =>
        ((int Attack, int Defense)[])_characters.Clone();

    public static CharacterRoster Build(int[][] properties)
    {
        var characters = new (int Attack, int Defense)[properties.Length];

        for (var i = 0; i < properties.Length; i++)
        {
            characters[i] = (properties[i][AttackColumn], properties[i][DefenseColumn]);
        }

        return new CharacterRoster(characters);
    }
}
