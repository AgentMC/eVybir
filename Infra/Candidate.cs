using System.Text;

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

        public string UfEntryKind => HumanizeEntryType(EntryKind);

        public string ShortName
        {
            get
            {
                if (EntryKind != EntryType.Person) return Name;
                var namings = Name.Split(" ");
                var builder = new StringBuilder(namings[0]);
                for (int i = 1; i < namings.Length; i++)
                {
                    builder.Append(' ').Append(namings[i][0]).Append('.');
                    var dash = namings[i].IndexOf('-');
                    if (dash != -1 && dash + 1 < namings[i].Length)
                    {
                        builder.Append('-').Append(char.ToUpper(namings[i][dash + 1])).Append('.');
                    }
                }
                return builder.ToString();
            }
        }
    }
}
