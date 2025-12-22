namespace eVybir.Infra
{
    public record Candidate(string Name, DateTime? Date, string? Description, Candidate.EntryType EntryKind)
    {
        public enum EntryType
        {
            Unknown = 0,
            Person = 1,
            Group = 2,
            Spoil = 3
        }

        public static string HumanizeEntryType(EntryType code)
        {
            return code switch
            {
                EntryType.Person => "Фізична особа",
                EntryType.Group => "Партія чи об'єднання",
                EntryType.Spoil => "Зіпсувати бюлетень",
                _ => "Невизначено",
            };
        }

        public string Kind => HumanizeEntryType(EntryKind);

    }
}
