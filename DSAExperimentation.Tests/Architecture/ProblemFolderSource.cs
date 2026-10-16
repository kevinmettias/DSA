using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// One LeetCode problem folder read as text, the way BaselineIsolationTests traces a baseline
// through it: every method, field or property, and type the folder declares, each with the type
// it sits in. A name the code spells resolves roughly as C# would find it - to a member of the
// nearest enclosing type that declares it, else to a type the folder declares, else to a static
// member of a static class or of the <Problem>Solution class. A strategy class's instance
// members are therefore reached only through the class itself, by constructing or naming it, so
// two strategies that both declare `Collect` are not linked by that name alone.
internal sealed class ProblemFolderSource
{
    private const string SolutionSuffix = "Solution";

    private static readonly Regex TypeDeclaration = new(
        """
        ^[ \t]*(?<modifiers>(?:(?:public|private|internal|protected|static|readonly|sealed|abstract|partial|file|ref|unsafe|new)\s+)*)
        (?:class|struct|record\s+struct|record\s+class|record|interface|enum)\s+(?<name>[A-Z]\w*)
        """,
        RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
    private static readonly Regex MethodDeclaration = new(
        """
        ^[ \t]*(?<modifiers>(?:(?:public|private|internal|protected|static|readonly|override|virtual|sealed|async|unsafe|new|extern|abstract|partial)\s+)+)
        [\w<>\[\],.?() ]+?\s+(?<name>\w+)\s*(?:<[^<>()]*(?:<[^<>()]*>[^<>()]*)*>)?\s*\(
        """,
        RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
    private static readonly Regex MemberDeclaration = new(
        """
        ^[ \t]*(?<modifiers>(?:(?:public|private|internal|protected|static|readonly|const|required|volatile|new|override|virtual|abstract)\s+)+)
        [\w<>\[\],.?() ]+?\s+(?<name>\w+)\s*(?==>|=|;|\{)
        """,
        RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
    private static readonly Regex StaticModifier = new(@"\b(?:static|const)\b");
    private static readonly Regex NamespaceDeclaration = new(@"^namespace\s+(?<name>[\w.]+)", RegexOptions.Multiline);

    private readonly List<SourceDeclaration> _declarations = [];
    private readonly Dictionary<string, string> _codeByFile = new(StringComparer.Ordinal);
    private readonly Dictionary<SourceDeclaration, Dictionary<string, List<SourceDeclaration>>> _membersByOwner = [];
    private readonly Dictionary<string, List<SourceDeclaration>> _typesByName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<SourceDeclaration>> _staticMembersByName = new(StringComparer.Ordinal);

    // The folder's own namespace, which its types are declared in without a using.
    public string Namespace { get; private set; } = string.Empty;

    public IReadOnlyList<SourceDeclaration> Declarations => _declarations;

    // Each (file, code) pair is one source file's path and its code with comments and string
    // contents blanked, as RepositoryFiles.CodeOf leaves it.
    public static ProblemFolderSource Of(IEnumerable<(string File, string Code)> files)
    {
        var source = new ProblemFolderSource();

        foreach (var (file, code) in files)
        {
            source.Add(file, code);
        }

        source.Index();

        return source;
    }

    public string CodeOf(SourceDeclaration declaration) => _codeByFile[declaration.File];

    public bool HasTypeNamed(string name) => _typesByName.ContainsKey(name);

    // The declarations a benchmark arm can mean by `name`: the Solution class's methods of that
    // name, narrowed to the overloads its argument types fit when those are known, or the type it
    // constructs. With no overload fitting, every one is kept - a trace that over-reaches is safer
    // than one that skips what runs.
    public IReadOnlyList<SourceDeclaration> EntriesNamed(string name, IReadOnlyList<string?>? argumentTypes)
    {
        var named = _declarations.Where(declaration => IsEntry(declaration, name)).ToList();

        if (argumentTypes is null)
        {
            return named;
        }

        var fitting = named.Where(declaration => OverloadFit.CanTake(declaration, argumentTypes)).ToList();

        if (fitting.Count == 0)
        {
            return named;
        }

        return fitting;
    }

    // Every declaration `from` reaches in one step: what each name its code spells resolves to.
    // A type named only as Type.Member reaches just those members, not the whole type - a static
    // call runs that member and nothing else the type holds.
    public IEnumerable<SourceDeclaration> LinkedFrom(SourceDeclaration from, IReadOnlySet<string> inputTypes)
    {
        var mentions = MentionsOf(from);
        var accesses = QualifiedAccesses.Of(from.Text);

        foreach (var name in mentions)
        {
            var targets = Resolve(from, name, inputTypes);
            var isOnlyAccessed = accesses.IsOnlyAccessed(name, from.Text);

            foreach (var target in targets)
            {
                var reached = ReachedThrough(target, name, accesses, isOnlyAccessed);

                foreach (var declaration in reached)
                {
                    yield return declaration;
                }
            }
        }
    }

    private void Add(string file, string code)
    {
        _codeByFile[file] = code;
        Namespace = NamespaceOf(code, Namespace);

        var source = new SourceFileCode(file, code);
        var types = TypesIn(source);
        _declarations.AddRange(types);

        var methods = MethodsIn(source, types);
        _declarations.AddRange(methods);

        var taken = methods.Select(method => method.Start).ToHashSet();
        _declarations.AddRange(MembersIn(source, types, taken));
    }

    private static string NamespaceOf(string code, string known)
    {
        var declared = NamespaceDeclaration.Match(code);

        if (!declared.Success)
        {
            return known;
        }

        return declared.Groups["name"].Value;
    }

    // Types in source order, so every type's owner is created before it.
    private static List<SourceDeclaration> TypesIn(SourceFileCode source)
    {
        var types = new List<SourceDeclaration>();

        foreach (Match declared in TypeDeclaration.Matches(source.Code))
        {
            types.Add(TypeAt(declared, source, types));
        }

        return types;
    }

    private static SourceDeclaration TypeAt(Match declared, SourceFileCode source, List<SourceDeclaration> outerTypes)
    {
        var (end, primaryParameters) = SourceSpans.TypeEnd(source.Code, declared.Index + declared.Length);

        return new SourceDeclaration
        {
            Kind = DeclarationKind.Type,
            Name = declared.Groups["name"].Value,
            File = source.File,
            Start = declared.Index,
            End = end,
            Text = source.Code[declared.Index..end],
            Signature = primaryParameters,
            Owner = InnermostOwner(outerTypes, declared.Index),
            IsStatic = StaticModifier.IsMatch(declared.Groups["modifiers"].Value),
        };
    }

    // The innermost type whose body holds `position`, or null at file level.
    private static SourceDeclaration? InnermostOwner(List<SourceDeclaration> types, int position)
    {
        SourceDeclaration? owner = null;

        foreach (var type in types)
        {
            if (type.Start < position && position < type.End)
            {
                owner = type;
            }
        }

        return owner;
    }

    private static List<SourceDeclaration> MethodsIn(SourceFileCode source, List<SourceDeclaration> types)
    {
        var methods = new List<SourceDeclaration>();

        foreach (Match declared in MethodDeclaration.Matches(source.Code))
        {
            methods.Add(MethodAt(declared, source, types));
        }

        return methods;
    }

    private static SourceDeclaration MethodAt(Match declared, SourceFileCode source, List<SourceDeclaration> types)
    {
        var parameters = declared.Index + declared.Length - 1;
        var end = SourceSpans.BodyEnd(source.Code, parameters);
        var signatureEnd = SourceSpans.MatchingClose(source.Code, parameters) + 1;

        return new SourceDeclaration
        {
            Kind = DeclarationKind.Method,
            Name = declared.Groups["name"].Value,
            File = source.File,
            Start = declared.Index,
            End = end,
            Text = source.Code[declared.Index..end],
            Signature = source.Code[declared.Index..signatureEnd],
            Owner = InnermostOwner(types, declared.Index),
            IsStatic = StaticModifier.IsMatch(declared.Groups["modifiers"].Value),
        };
    }

    // Fields and properties: only inside a type, and never a line a method already claimed.
    private static List<SourceDeclaration> MembersIn(SourceFileCode source, List<SourceDeclaration> types, HashSet<int> taken)
    {
        var members = new List<SourceDeclaration>();

        foreach (Match declared in MemberDeclaration.Matches(source.Code))
        {
            var owner = InnermostOwner(types, declared.Index);

            if (owner is not null && !taken.Contains(declared.Index))
            {
                members.Add(MemberAt(declared, source, owner));
            }
        }

        return members;
    }

    private static SourceDeclaration MemberAt(Match declared, SourceFileCode source, SourceDeclaration owner)
    {
        var end = SourceSpans.MemberEnd(source.Code, declared.Index + declared.Length);

        return new SourceDeclaration
        {
            Kind = DeclarationKind.Member,
            Name = declared.Groups["name"].Value,
            File = source.File,
            Start = declared.Index,
            End = end,
            Text = source.Code[declared.Index..end],
            Signature = string.Empty,
            Owner = owner,
            IsStatic = StaticModifier.IsMatch(declared.Groups["modifiers"].Value),
        };
    }

    private void Index()
    {
        foreach (var declaration in _declarations)
        {
            IndexByOwner(declaration);
            IndexType(declaration);
            IndexStaticMember(declaration);
        }
    }

    private void IndexByOwner(SourceDeclaration declaration)
    {
        if (declaration.Owner is null)
        {
            return;
        }

        if (!_membersByOwner.TryGetValue(declaration.Owner, out var members))
        {
            members = new Dictionary<string, List<SourceDeclaration>>(StringComparer.Ordinal);
            _membersByOwner[declaration.Owner] = members;
        }

        AddTo(members, declaration);
    }

    // The Solution class is the folder itself rather than a type its code names, so it is never
    // a name's target: a delegation into another problem names it, and is followed there instead.
    private void IndexType(SourceDeclaration declaration)
    {
        if (declaration.Kind != DeclarationKind.Type || IsSolutionClass(declaration))
        {
            return;
        }

        AddTo(_typesByName, declaration);
    }

    // A static member of a type that is never instantiated - a static class, or the Solution
    // class - can only be called, qualified or not, so a name reaches it without its type.
    private void IndexStaticMember(SourceDeclaration declaration)
    {
        if (!IsCallableWithoutItsType(declaration))
        {
            return;
        }

        AddTo(_staticMembersByName, declaration);
    }

    private static bool IsCallableWithoutItsType(SourceDeclaration declaration) =>
        declaration.Kind != DeclarationKind.Type
            && declaration.IsStatic
            && declaration.Owner is not null
            && (declaration.Owner.IsStatic || IsSolutionClass(declaration.Owner));

    private static void AddTo(Dictionary<string, List<SourceDeclaration>> index, SourceDeclaration declaration)
    {
        if (!index.TryGetValue(declaration.Name, out var named))
        {
            named = [];
            index[declaration.Name] = named;
        }

        named.Add(declaration);
    }

    private static bool IsSolutionClass(SourceDeclaration type) =>
        type.Owner is null && type.Name.EndsWith(SolutionSuffix, StringComparison.Ordinal);

    private static bool IsEntry(SourceDeclaration declaration, string name)
    {
        if (declaration.Name != name)
        {
            return false;
        }

        if (declaration.Kind == DeclarationKind.Type)
        {
            return true;
        }

        return declaration.Kind == DeclarationKind.Method && declaration.Owner is not null && IsSolutionClass(declaration.Owner);
    }

    // The names a declaration's code spells. A method's own name in its header is not a call to
    // its overloads, but its return and parameter types are mentions: a target-typed `new()`
    // names its type nowhere else.
    private static HashSet<string> MentionsOf(SourceDeclaration declaration)
    {
        if (declaration.Kind != DeclarationKind.Method)
        {
            return SourceSpans.WordsIn(declaration.Text);
        }

        var mentions = SourceSpans.WordsIn(declaration.Signature);
        mentions.Remove(declaration.Name);

        var body = declaration.Text[declaration.Signature.Length..];
        mentions.UnionWith(SourceSpans.WordsIn(body));

        return mentions;
    }

    private IReadOnlyList<SourceDeclaration> Resolve(SourceDeclaration from, string name, IReadOnlySet<string> inputTypes)
    {
        foreach (var scope in ScopesOf(from))
        {
            var members = MembersNamed(scope, name);

            if (members.Count > 0)
            {
                return WithoutInputTypes(members, name, inputTypes);
            }
        }

        if (!inputTypes.Contains(name) && _typesByName.TryGetValue(name, out var types))
        {
            return types;
        }

        return _staticMembersByName.GetValueOrDefault(name) ?? [];
    }

    // An input type the harness hands the baseline is the caller's, so its body is not traced.
    private static IReadOnlyList<SourceDeclaration> WithoutInputTypes(
        IReadOnlyList<SourceDeclaration> members, string name, IReadOnlySet<string> inputTypes)
    {
        if (!inputTypes.Contains(name))
        {
            return members;
        }

        return [.. members.Where(member => member.Kind != DeclarationKind.Type)];
    }

    // The types whose members a name inside `from` can mean, nearest first: a type's own body,
    // then every type it is nested in.
    private static IEnumerable<SourceDeclaration> ScopesOf(SourceDeclaration from)
    {
        var scope = from.Owner;

        if (from.Kind == DeclarationKind.Type)
        {
            scope = from;
        }

        for (; scope is not null; scope = scope.Owner)
        {
            yield return scope;
        }
    }

    private IReadOnlyList<SourceDeclaration> MembersNamed(SourceDeclaration type, string name)
    {
        if (!_membersByOwner.TryGetValue(type, out var members))
        {
            return [];
        }

        return members.GetValueOrDefault(name) ?? [];
    }

    // A type reached only as Type.Member gives up the members named after it, not its body.
    private IEnumerable<SourceDeclaration> ReachedThrough(
        SourceDeclaration target, string name, QualifiedAccesses accesses, bool isOnlyAccessed)
    {
        if (target.Kind != DeclarationKind.Type || !isOnlyAccessed)
        {
            return [target];
        }

        return accesses.MembersOf(name).SelectMany(member => MembersNamed(target, member));
    }

    // One source file being read: its path, and its code with comments and string contents blanked.
    private readonly record struct SourceFileCode(string File, string Code);

    // Every Type.Member a stretch of code spells, by type name: which members it names, and how
    // many of the type name's mentions are such accesses.
    private sealed class QualifiedAccesses
    {
        private static readonly Regex QualifiedAccess = new(@"\b(?<type>[A-Z]\w*)\s*\.\s*(?<member>\w+)");

        private readonly Dictionary<string, HashSet<string>> _membersByType = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _countByType = new(StringComparer.Ordinal);

        public static QualifiedAccesses Of(string code)
        {
            var accesses = new QualifiedAccesses();

            foreach (Match access in QualifiedAccess.Matches(code))
            {
                accesses.Add(access.Groups["type"].Value, access.Groups["member"].Value);
            }

            return accesses;
        }

        public IEnumerable<string> MembersOf(string type) => _membersByType.GetValueOrDefault(type) ?? [];

        // Whether every mention of `name` in `code` is the left side of a member access.
        public bool IsOnlyAccessed(string name, string code)
        {
            if (!_countByType.TryGetValue(name, out var accessed))
            {
                return false;
            }

            return SourceSpans.CountOf(name, code) == accessed;
        }

        private void Add(string type, string member)
        {
            if (!_membersByType.TryGetValue(type, out var members))
            {
                members = new HashSet<string>(StringComparer.Ordinal);
                _membersByType[type] = members;
            }

            members.Add(member);
            _countByType[type] = _countByType.GetValueOrDefault(type) + 1;
        }
    }
}
